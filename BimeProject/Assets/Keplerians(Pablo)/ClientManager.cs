using UnityEngine;
using System.Collections;

public class ClientManager : Photon.MonoBehaviour {

	public static ClientManager instance;
	public UILabel rotationLabel;
	public GameObject loginPanel;
	public UIInput inputName;
	public Transform arrowTexture;


	public GameObject stopButton;
	public GameObject continueButton;

	void Awake () {
		instance = this;
	}

	[PunRPC]public void SetRotation(float angle){
		NGUITools.SetActive (arrowTexture.gameObject, true);
		arrowTexture.eulerAngles = new Vector3 (arrowTexture.eulerAngles.x, arrowTexture.eulerAngles.y, angle+90);
		//rotationLabel.text = angle.ToString ();
	}

	public void ShowInputPanel(){
		NGUITools.SetActive(loginPanel,true);
	}
	
	public void HideInputPanel(){
		NGUITools.SetActive(loginPanel,false);
	}

	public void OnClickStop(){
		photonView.RPC ("ReceiveStop", PhotonTargets.All, PhotonNetwork.playerName);

	}

	public void OnCLickCOntinue(){
		photonView.RPC ("ReceiveContinue", PhotonTargets.All, PhotonNetwork.playerName);

	}

	[PunRPC]public void ReceiveStop(string playerName){
		if (GameManager.instance.isDirector) {
			//tratar mensaje
			foreach(GameObject go in GameManager.instance.clients){
				if(go.GetComponent<PlayerClient>().photonPlayer.name.Equals(playerName)){
					
					go.GetComponent<FestivalUserBehaviour>().OnStop();
				}
			}
		}
	}

	[PunRPC]public void ReceiveContinue(string playerName){
		if (GameManager.instance.isDirector) {
			//tratar mensaje
			foreach(GameObject go in GameManager.instance.clients){
				if(go.GetComponent<PlayerClient>().photonPlayer.name.Equals(playerName)){
					
					go.GetComponent<FestivalUserBehaviour>().OnContinue();
				}
			}
		}
	}

}
