using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class UserController : MonoBehaviour {

	public static UserController instance;
	public enum UserType{Green,Azul,Rojo,Amarillo};

	public List<EntranceInfo> entrances;
	public GameObject userPrefab;
	public List<FestivalUserBehaviour> users;
	public List<User> usersData;

	public DetectCollision detector;

	// Use this for initialization
	void Awake () {
		instance = this;
		users = new List<FestivalUserBehaviour> ();
	}

	void Start(){
		//StartCoroutine ("CreateUsersTesting");

		VectorLine.SetCanvasCamera (Camera.main);
		VectorLine.canvas.renderMode = RenderMode.WorldSpace;
	}

	public void CreateNewUser(string mName,UserType type,int door){
		GameObject newUser = Instantiate (userPrefab) as GameObject;
		newUser.transform.position = entrances [door].transform.position;
		FestivalUser userInfo = new FestivalUser ();
		userInfo.ID = users.Count;
		userInfo.name = mName;
		userInfo.animInfo = usersData.Find (ud => ud.userType == type);
		newUser.GetComponent<FestivalUserBehaviour> ().Init (entrances [door], userInfo);
	}

	public void OnCreateNewUser(string mName,GameObject newUser,UserType type,int door){
		newUser.transform.position = entrances [door].transform.position;
		FestivalUser userInfo = new FestivalUser ();
		userInfo.userType = type;
		userInfo.ID = users.Count;
		userInfo.name = mName;
		userInfo.animInfo = usersData.Find (ud => ud.userType == type);
		newUser.GetComponent<FestivalUserBehaviour> ().Init (entrances [door], userInfo);
	}

	public void OnUserFinish(){

	}

	public IEnumerator CreateUsersTesting(){
		int index = 0;
		string[] names = new string[]{"alex","ion","pablo","pepe"};

		while (true) {
			CreateNewUser(names[index],(UserType)Random.Range(0,4),0);
			index++;
			yield return new WaitForSeconds(25.0F);
		}

	}
	public bool detectorEnabled = false;
	public void SetPosition(Vector3 worldPos){
		detector.transform.position = worldPos;
	}
	public void EnableDetector(){
		detectorEnabled = true;
		detector.EnableDetector ();
	}
	public void DisableDetector(){
		detectorEnabled = false;
		detector.DisableDetector ();
	}

}

[System.Serializable]
public class FestivalUser{
	public string name;
	public int ID;
	public UserController.UserType userType;
	public User animInfo;
}

[System.Serializable]
public class User{
	public UserController.UserType userType;
	public string walk_anim;
	public string idle_anim;
	public string crash_anim;
}
