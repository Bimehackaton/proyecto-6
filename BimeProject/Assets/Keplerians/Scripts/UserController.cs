using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserController : MonoBehaviour {

	public static UserController instance;
	public enum UserType{Green,Azul,Rojo,Amarillo};

	public List<EntranceInfo> entrances;
	public GameObject userPrefab;
	public List<FestivalUserBehaviour> users;

	// Use this for initialization
	void Awake () {
		instance = this;
		users = new List<FestivalUserBehaviour> ();
	}

	void Start(){
		StartCoroutine ("CreateUsersTesting");
	}

	public void CreateNewUser(string mName){
		GameObject newUser = Instantiate (userPrefab) as GameObject;
		newUser.transform.position = entrances [0].transform.position;
		FestivalUser userInfo = new FestivalUser ();
		userInfo.ID = users.Count;
		userInfo.name = mName;
		newUser.GetComponent<FestivalUserBehaviour> ().Init (entrances [0], userInfo);
	}

	public void OnUserFinish(){

	}

	public IEnumerator CreateUsersTesting(){
		int index = 0;
		string[] names = new string[]{"alex","ion","pablo","pepe"};

		while (true) {
			CreateNewUser(names[index]);
			index++;
			yield return new WaitForSeconds(25.0F);
		}

	}

}

[System.Serializable]
public class FestivalUser{
	public string name;
	public int ID;
	public UserController.UserType userType;
}
