using UnityEngine;
using System.Collections;

public class ConnectionManager : MonoBehaviour {

	private string roomName = "myRoom";
	public static ConnectionManager instance;
	
	void Awake(){
		instance = this;
		if (!PhotonNetwork.connected)
			PhotonNetwork.ConnectUsingSettings("v1.0"); // version of the game/demo. used to separate older clients from newer ones (e.g. if incompatible)
		
		//PhotonNetwork.playerName = PlayerPrefs.GetString("playerName", "Guest" + Random.Range(1, 9999));
	}

	void Start () {
		
	}

	public void CreateRoom(){
		if (PhotonNetwork.connected){
			Debug.Log ("Create room");
			if(GameManager.instance.isDirector){
				PhotonNetwork.CreateRoom ("MainRoom" + Random.value.ToString(), new RoomOptions () { maxPlayers = 20 }, TypedLobby.Default);
			}
			else{
				PhotonNetwork.playerName = ClientManager.instance.inputName.label.text;
				ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
				props.Add("type", (int)ClientManager.instance.currentUType);
				props.Add("door", (int)ClientManager.instance.currentDoor);
				PhotonNetwork.player.SetCustomProperties(props);
				PhotonNetwork.JoinRandomRoom();
			}

		}
	}


}
