using UnityEngine;
using System.Collections;

[System.Serializable]
public class RoomSpaces{
	public UserController.UserType uType;
	[SerializeField]public RoomPosition[] positions;


}

[System.Serializable]
public class RoomPosition{
	public Transform positionRef;
	public bool isFree = true; 
}

public class RoomSpaceController : MonoBehaviour {
	public static RoomSpaceController instance;
	public RoomSpaces[] roomsInfo;

	void Awake(){
	
		instance = this;
	}

	public Vector3 FindPosition (UserController.UserType type) {
		foreach (RoomSpaces rs in roomsInfo) {
			if(rs.uType == type){
				foreach(RoomPosition rp in rs.positions){
					if(rp.isFree){
						rp.isFree = false;
						return rp.positionRef.position;
					}
				}
				return rs.positions[0].positionRef.position;
			}
		}
		return Vector3.zero;
	}
}
