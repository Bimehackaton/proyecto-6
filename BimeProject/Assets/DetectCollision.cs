using UnityEngine;
using System.Collections;

public class DetectCollision : MonoBehaviour {

	CircleCollider2D mCollider;

	public enum CollisionType{None,Wall,Obstacle,Target};
	public CollisionType collisionType;
	public GameObject lastObject;

	void Awake(){
		mCollider = GetComponent<CircleCollider2D> ();
	}

	public void EnableDetector(){
		mCollider.enabled = true;
	}
	public void DisableDetector(){
		mCollider.enabled = false;
		collisionType = CollisionType.None;
		//lastObject = null;
	}

	void OnTriggerStay2D(Collider2D col){
		if (col.tag == "no_walkable") {
			Debug.Log("Colision muro");
			collisionType = CollisionType.Wall;
		} 
		else if (col.tag == "targetZone") {
			collisionType = CollisionType.Target;
			lastObject = col.gameObject;
			Debug.Log("Colision target");
		}	



	}
	void OnTriggerExit2D(Collider2D col){
		if (col.tag == "no_walkable") {
			if(collisionType == CollisionType.Wall){
				collisionType = CollisionType.None;
			}
		} 
		else if (col.tag == "targetZone") {
			if(collisionType == CollisionType.Target){
				collisionType = CollisionType.None;
			}
		}	

	}
}
