using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FestivalUserBehaviour : MonoBehaviour {

	public List<Vector3> followPoints;
	public List<Vector3> newFollowPoints;

	public FestivalUser _Data;
	public TextMesh nameLabel;
	public float speed;
	public int currentPoint;

	public enum FestivalUserState{FollowingPoints,FollowingDirection};
	public FestivalUserState mState;

	public enum SelectionState{Pressed,Dragging,Released,Cancelled};
	public SelectionState selectionState;

	public Vector3 currentDirection;

	EntranceInfo entrance;

	public float minWaypointDistance = 0.1F;
	public float changeFollowPointDistance = 0.01F;

	// Use this for initialization
	public void Init (EntranceInfo mEntrance,FestivalUser data) {
		currentPoint = 0;
		entrance = mEntrance;
		mState = FestivalUserState.FollowingDirection;
		currentDirection = mEntrance.entranceDirection;
		transform.up = currentDirection;
		_Data = data;
		nameLabel.text = data.name;

		StartCoroutine ("FollowBehaviour");

	}
	
	public IEnumerator FollowBehaviour(){

		while (true) {
			if(mState == FestivalUserState.FollowingPoints){
				Vector3 targetPoint = followPoints[currentPoint];
				currentDirection =  (followPoints[currentPoint] - transform.position).normalized;
				transform.up = currentDirection;
				transform.position += currentDirection * Time.deltaTime * speed;

				if(Vector2.Distance(transform.position,(Vector2)followPoints[currentPoint]) < changeFollowPointDistance){
					Debug.Log("Ha alcanzado el siguiente waypoint");

					if(currentPoint < followPoints.Count-1){
						Debug.Log("Pasar al siguiente waypoint");
						//Next point
						currentPoint++;
					}
					else{
						Debug.Log("Ha llegado al final del camino, continuar con la misma direccion... current point is " + currentPoint.ToString());
						mState = FestivalUserState.FollowingDirection;
					}
				}
			}
			else{
				transform.position += currentDirection * Time.deltaTime * speed;

				//Raycast para detectar cambios de direccion...IA básica

			}

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
			//Inicializar caminos...
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


		if (Vector2.Distance (worldPoint, !newWayStarted?transform.position:followPoints[followPoints.Count - 1]) > minWaypointDistance) {

			if(!newWayStarted){
				followPoints.Clear();
				followPoints.Add(transform.position);
				newWayStarted = true;
				currentPoint = 0;
				
				if(mState == FestivalUserState.FollowingDirection){
					mState = FestivalUserState.FollowingPoints;
				}
				else{

				}

			}

			//Add new point
			Vector3 addPoint = UICamera.lastWorldPosition;
			addPoint.z = transform.position.z;
			followPoints.Add(addPoint);
			pressTime = Time.time;

			if(mState == FestivalUserState.FollowingDirection){
				if(currentPoint > 0 && currentPoint < followPoints.Count-1){
					mState = FestivalUserState.FollowingPoints;
				}
			}

			Debug.Log ("Add waypoint");
		}


	}

	public void OnRelease(){

		Debug.Log ("On Release");
			
	}


}
