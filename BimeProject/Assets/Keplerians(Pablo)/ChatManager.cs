using UnityEngine;
using System.Collections;

public class ChatManager : Photon.MonoBehaviour {

	public UIInput input;
	
	public void OnCLick_Send(){
		photonView.RPC ("ReceiveMessage", PhotonTargets.All, input.label.text,PhotonNetwork.playerName);
	}

	[PunRPC]public void ReceiveMessage(string message,string playerName){
		if (GameManager.instance.isDirector) {
			//tratar mensaje
			foreach(GameObject go in GameManager.instance.clients){
				if(go.GetComponent<PlayerClient>().photonPlayer.name.Equals(playerName)){

					go.GetComponent<FestivalUserBehaviour>().CreateChatMessage(message);
				}
			}
		}
	}
}
