using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class FestivalUserBehaviour : MonoBehaviour {

	public List<Vector2> followPoints;

	public FestivalUser _Data;
	public float speed;
	public int currentPoint;

	public enum FestivalUserState{FollowingPoints,FollowingDirection,Crashed,Finished};
	public FestivalUserState mState;

	public enum SelectionState{Pressed,Dragging,Released,Cancelled,Completed,WrongCompleted};
	public SelectionState selectionState;

	public enum UserControlState{Stopped,Continue};
	public UserControlState controlState;

	public Vector3 currentDirection;

	EntranceInfo entrance;

	public float minWaypointDistance = 0.1F;
	public float changeFollowPointDistance = 0.01F;

	public LayerMask mCollisionLayer;

	public float lineWidth;
	public SkeletonAnimation mSkeletonAnimation;

	public GameObject nameLabel_prefab;
	public GameObject message_prefab;
	public UILabel nameLabel;

	// Use this for initialization
	public void Init (EntranceInfo mEntrance,FestivalUser data) {
		currentPoint = 0;
		entrance = mEntrance;
		mState = FestivalUserState.FollowingDirection;
		currentDirection = mEntrance.entranceDirection;
		transform.up = currentDirection;
		_Data = data;
		followPoints = new List<Vector2> ();

		InitLine ();
		StartCoroutine ("FollowBehaviour");

		mSkeletonAnimation.state.SetAnimation (0,_Data.animInfo.walk_anim, true);
		nameLabel = (Instantiate (nameLabel_prefab) as GameObject).GetComponent<UILabel> ();
		nameLabel.GetComponent<FollowPlayer> ().target = transform;
		nameLabel.text = _Data.name;

		controlState = UserControlState.Continue;
	}

	public void OnRemove(){
		if (msg != null)
			Destroy (msg);
		Destroy (nameLabel.gameObject);
		Destroy (gameObject);
	}
	
	public IEnumerator FollowBehaviour(){

		while (true) {
			if(controlState == UserControlState.Continue){
				if(mState == FestivalUserState.FollowingPoints){
					Vector3 targetPoint = followPoints[currentPoint];
					currentDirection =  (followPoints[currentPoint] - (Vector2)transform.position).normalized;
					transform.up = currentDirection;
					transform.position += currentDirection * Time.deltaTime * speed;

					if(Vector2.Distance(transform.position,followPoints[currentPoint]) < changeFollowPointDistance){
						Debug.Log("Ha alcanzado el siguiente waypoint");

						if(currentPoint < followPoints.Count-1){
							Debug.Log("Pasar al siguiente waypoint");
							//Next point
							currentPoint++;
						}
						else{
							if(selectionState == SelectionState.Completed){
								Debug.Log("Ha finalizado");
								mSkeletonAnimation.state.SetAnimation (0,_Data.animInfo.idle_anim, true);
								//TODO El usuario se colocará en un sitio libre? Desconectar al usuario...
								mState = FestivalUserState.Finished;

								ScoreManager.instance.OnUserFinished();
								yield break;
							}
							else if(selectionState == SelectionState.WrongCompleted){
								Debug.Log("Ha finalizado incorrectamente");
								mSkeletonAnimation.state.SetAnimation (0,_Data.animInfo.crash_anim, true);
								//TODO El usuario se colocará en un sitio libre? Desconectar al usuario...
								mState = FestivalUserState.Finished;
								
								ScoreManager.instance.OnUserFinishedWrong();
								yield break;
							}
							Debug.Log("Ha llegado al final del camino, continuar con la misma direccion... current point is " + currentPoint.ToString());
							mState = FestivalUserState.FollowingDirection;

							followPoints.Clear();

							if(myLine != null){
								VectorLine.Destroy(ref myLine);
							}
						}
					}
				}
				else{
					transform.position += currentDirection * Time.deltaTime * speed;
					transform.up = currentDirection;

					//Raycast para detectar cambios de direccion...IA básica
					RaycastHit2D hit = Physics2D.Raycast(transform.position,currentDirection,3F,mCollisionLayer);

					if(hit.collider != null){
						if(hit.collider.tag == "no_walkable"){
							Debug.Log("Wall detected");
							currentDirection = Vector3.Reflect(currentDirection,hit.normal);
						}
					}
				}
				UpdateLine();
			}

			yield return null;

			
		}

	}

	public void OnPress(bool pressed){

		Debug.Log("Pressed " + pressed.ToString());

		if (!pressed){
			if(selectionState != SelectionState.Cancelled && selectionState != SelectionState.Completed){
				OnRelease ();	
			}
		} 
		else {
			selectionState = SelectionState.Pressed;
			newWayStarted = false;
		}
	}
	bool newWayStarted;

	public void OnDrag(Vector2 delta){
		if (selectionState == SelectionState.Cancelled || selectionState == SelectionState.Completed)
			return;

		if (selectionState == SelectionState.Pressed) {
			selectionState = SelectionState.Dragging;

			if(myLine!=null){
				followPoints.Clear();
				VectorLine.Destroy(ref myLine);
			}
		}

		Vector3 worldPoint = UICamera.lastWorldPosition;
		worldPoint.z = transform.position.z;
		Vector2 nextPoint = Vector2.zero;

		if (!newWayStarted || (newWayStarted && mState == FestivalUserState.FollowingDirection)) {
			nextPoint = transform.position;
		} 
		else if (newWayStarted && mState == FestivalUserState.FollowingPoints) {
			nextPoint = followPoints [followPoints.Count - 1];
		} 


		if (Vector2.Distance ((Vector2)worldPoint, nextPoint) > minWaypointDistance) {
			bool initLine = false;

			if(!newWayStarted){
				initLine = true;
				newWayStarted = true;
				currentPoint = 0;
				followPoints.Add(transform.position);

				if(mState == FestivalUserState.FollowingDirection){
					mState = FestivalUserState.FollowingPoints;
				}
				else{

				}

			}

			if(newWayStarted && mState == FestivalUserState.FollowingDirection){
				if(followPoints.Count == 0){
					mState = FestivalUserState.FollowingPoints;
					followPoints.Add(transform.position);
					currentPoint = 0;
				}
			}

			//Add new point
			Vector3 addPoint = UICamera.lastWorldPosition;
			addPoint.z = transform.position.z;

			followPoints.Add(addPoint);

			if(initLine){
				InitLine();
			}
		}

		if (newWayStarted && followPoints.Count > 0) {
			RaycastHit2D hit = Physics2D.Raycast (followPoints [followPoints.Count - 1],
			                                      (Vector2)worldPoint - followPoints [followPoints.Count - 1], 10.0F);
			if(hit.collider != null){
				if (hit.collider.tag == "no_walkable") {
					Debug.Log("Way cancelled");
					selectionState = SelectionState.Cancelled;
					return;
				}
				
				if (hit.collider.tag == "targetZone") {
					Debug.Log("target assigned");
					if(hit.collider.gameObject.GetComponent<TargetZone>().type == _Data.userType){
						selectionState = SelectionState.Completed;
					}
					else{
						selectionState = SelectionState.WrongCompleted;
					}
					
					return;
				}
			}
		}


	}

	public void OnRelease(){
		selectionState = SelectionState.Released;
		Debug.Log ("On Release");
			
	}

	VectorLine myLine;

	//Line methods
	public void InitLine(){

		if (myLine != null) {
			VectorLine.Destroy(ref myLine);
		}

		followPoints.Clear ();
		myLine = new VectorLine("Line", followPoints, lineWidth,LineType.Continuous,Joins.Weld); // C#
		myLine.SetColor (Color.blue);
	}

	public void UpdateLine(){
		if (myLine != null) {
			myLine.drawStart = currentPoint;
			//myLine = new VectorLine("Line", followPoints, 2.0f,LineType.Continuous,Joins.Weld); // C#
			myLine.MakeSpline (followPoints.ToArray (), followPoints.Count);

			myLine.Draw ();
		}
			
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player" || col.tag == "Obstacle") {
			if(mState != FestivalUserState.Crashed){
				mSkeletonAnimation.state.SetAnimation (0,_Data.animInfo.crash_anim, false);
				StopCoroutine("FollowBehaviour");
				mState = FestivalUserState.Crashed;
			}
		}

	}
	public GameObject msg;

	public void CreateChatMessage(string txt){
		if (msg != null)
			Destroy (msg);
		msg = (Instantiate (message_prefab) as GameObject);
		msg.GetComponent<FollowPlayer> ().target = transform;
		msg.GetComponent<MessageScript> ().InitMessage (txt);
	}

	public void OnStop(){
		controlState = UserControlState.Stopped;
	}

	public void OnContinue(){
		controlState = UserControlState.Continue;
	}


}
