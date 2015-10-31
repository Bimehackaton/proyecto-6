using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class FestivalUserBehaviour : MonoBehaviour {

	public List<Vector2> followPoints;

	public FestivalUser _Data;
	public float speed;
	public int currentPoint;

	public enum FestivalUserState{FollowingPoints,FollowingDirection,Crashed};
	public FestivalUserState mState;

	public enum SelectionState{Pressed,Dragging,Released,Cancelled};
	public SelectionState selectionState;

	public Vector3 currentDirection;

	EntranceInfo entrance;

	public float minWaypointDistance = 0.1F;
	public float changeFollowPointDistance = 0.01F;

	public LayerMask mCollisionLayer;

	public float lineWidth;
	public SkeletonAnimation mSkeletonAnimation;

	public GameObject nameLabel_prefab;
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
	}

	public void OnRemove(){
		Destroy (nameLabel.gameObject);
		Destroy (gameObject);
	}
	
	public IEnumerator FollowBehaviour(){

		while (true) {
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
						Debug.Log("Ha llegado al final del camino, continuar con la misma direccion... current point is " + currentPoint.ToString());
						mState = FestivalUserState.FollowingDirection;

						followPoints.Clear();
						VectorLine.Destroy(ref myLine);
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
			yield return null;
		
		}

	}
	float pressTime;
	float minStartDragTime = 1.0F;

	public void OnPress(bool pressed){
		selectionState = SelectionState.Pressed;
		Debug.Log("Pressed " + pressed.ToString());

		if (!pressed){
			if(selectionState != SelectionState.Cancelled){
				OnRelease ();	
			}
		} 
		else {
			pressTime = Time.time;
			newWayStarted = false;
		}
	}
	bool newWayStarted;

	public void OnDrag(Vector2 delta){
		if (selectionState == SelectionState.Cancelled)
			return;

		if (selectionState == SelectionState.Pressed) {
			selectionState = SelectionState.Dragging;

			if(myLine!=null){
				VectorLine.Destroy(ref myLine);
			}
		}


		//El tiempo que se puede tardar en añadir otro punto.. sino se cancela el camino?
//		if (Time.time - pressTime > minStartDragTime) {
//			//Cancelar el camino..
//			mState = FestivalUserState.FollowingDirection;
//			selectionState = SelectionState.Cancelled;
//			Debug.Log("Drawing cancelled");
//		}

		Vector3 worldPoint = UICamera.lastWorldPosition;
		worldPoint.z = transform.position.z;

		if (newWayStarted) {
			RaycastHit2D hit = Physics2D.Raycast (followPoints [followPoints.Count - 1],
			                                      (Vector2)worldPoint - followPoints [followPoints.Count - 1], 10.0F);
	
			if (hit.collider.tag == "no_walkable") {
				Debug.Log("Way cancelled");
				selectionState = SelectionState.Cancelled;
				return;
			}
		}

		if (Vector2.Distance ((Vector2)worldPoint, !newWayStarted?(Vector2)transform.position:followPoints[followPoints.Count - 1]) > minWaypointDistance) {

			if(!newWayStarted){
				followPoints.Clear();
				followPoints.Add(transform.position);
				newWayStarted = true;
				currentPoint = 0;

				InitLine();

				if(mState == FestivalUserState.FollowingDirection){
					mState = FestivalUserState.FollowingPoints;
				}
				else{

				}

			}


			if(mState == FestivalUserState.FollowingDirection){
				if(followPoints.Count == 0){
					InitLine();
					mState = FestivalUserState.FollowingPoints;
					followPoints.Add(transform.position);
					currentPoint = 0;
				}
			}

			//Add new point
			Vector3 addPoint = UICamera.lastWorldPosition;
			addPoint.z = transform.position.z;
			
			followPoints.Add(addPoint);
			pressTime = Time.time;



		}


	}

	public void OnRelease(){

		Debug.Log ("On Release");
			
	}

	VectorLine myLine;

	//Line methods
	public void InitLine(){
		myLine = new VectorLine("Line", followPoints, lineWidth,LineType.Continuous,Joins.Weld); // C#
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
				mSkeletonAnimation.state.SetAnimation (0,_Data.animInfo.crash_anim, true);
				StopCoroutine("FollowBehaviour");
				mState = FestivalUserState.Crashed;
			}
		}

	}


}
