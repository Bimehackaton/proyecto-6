﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public bool isDirector;
	// this is a object name (must be in any Resources folder) of the prefab to spawn as player avatar.
	// read the documentation for info how to spawn dynamically loaded game objects at runtime (not using Resources folders)
	public GameObject playerPrefab;
	public GameObject joinButton;
	public GameObject createButton;
	public static GameManager instance;
	public GameObject spawnPoint;
	[SerializeField]public List<GameObject> clients;
	public GameObject connectingLabel;

	void Awake(){
		instance = this;
	}

	void Start(){
		if (PhotonNetwork.connected) {
			if(!isDirector)
				ClientManager.instance.ShowInputPanel();
			else{
				NGUITools.SetActive(connectingLabel,false);
				NGUITools.SetActive (createButton, true);
			}
		}
	}

	void OnJoinedRoom()
	{
		Debug.Log ("OnJoinedRoom");
		if (isDirector) {
			NGUITools.SetActive (createButton, false);
			GameController.instance.OnStartGame();
		} 
		else {
			ClientManager.instance.HideInputPanel();
		}
	}

	void OnCreatedRoom(){
		Debug.Log ("OnCreatedRoom");

		//NGUITools.SetActive (createButton, false);

	}
	
	IEnumerator OnLeftRoom()
	{
		//Easy way to reset the level: Otherwise we'd manually reset the camera
		
		//Wait untill Photon is properly disconnected (empty room, and connected back to main server)
		while(PhotonNetwork.room!=null || PhotonNetwork.connected==false)
			yield return 0;
		
		Application.LoadLevel(Application.loadedLevel);
		
	}

	void OnPhotonPlayerConnected(PhotonPlayer pp){

		if (isDirector) {

			int type = (int)pp.customProperties["type"];
			int door = (int)pp.customProperties["door"];

			Debug.Log("Crear jugador conectado:" + pp.name + "door " + door.ToString());
			GameObject go = Instantiate (playerPrefab);
			UserController.instance.OnCreateNewUser(pp.name,go,(UserController.UserType)type,door);
			go.GetComponent<PlayerClient>().photonPlayer = pp;
			clients.Add(go);
		}
	}

	void OnPhotonPlayerDisconnected(PhotonPlayer pp){
		if (!isDirector) {
			if (pp.isMasterClient) {//Si no es director y se desconecta el master client...
				PhotonNetwork.LeaveRoom();
			}
		} 
		else {
			foreach (GameObject go in clients) {
				if (go.GetComponent<PlayerClient> ().photonPlayer.ID == pp.ID) {
					clients.Remove(go);
					go.GetComponent<FestivalUserBehaviour>().OnRemove();
					return;
				}
			}
		}

	}

	void OnDisconnectedFromPhoton()
	{
		Debug.LogWarning("OnDisconnectedFromPhoton");
		Application.LoadLevel (Application.loadedLevelName);
	}  

	void OnConnectedToPhoton()
	{
		Debug.Log("OnConnectedToPhoton");
		if (isDirector) {
			NGUITools.SetActive(connectingLabel,false);
			NGUITools.SetActive (createButton, true);

		} else {
			ClientManager.instance.ShowInputPanel();
		}
	} 
}
