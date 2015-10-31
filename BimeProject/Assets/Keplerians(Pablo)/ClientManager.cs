using UnityEngine;
using System.Collections;

[System.Serializable]
public class MusicTypeData{
	public string name;
	public Color color;
	public UserController.UserType uType;
	public AudioClip clip;
}

[System.Serializable]
public class DoorType{
	public string name;
	public int ID;
}

public class ClientManager : Photon.MonoBehaviour {

	public static ClientManager instance;
	public UILabel rotationLabel;
	public GameObject loginPanel;
	public UIInput inputName;
	public Transform arrowTexture;
	[SerializeField]public MusicTypeData[] types;
	[SerializeField]public DoorType[] doors;

	public GameObject stopButton;
	public GameObject continueButton;
	public GameObject loadingWindow;
	public GameObject completedWindow;
	public GameObject lostWindow;

	public UserController.UserType currentUType;
	public int currentDoor;

	public UIPopupList popupList;
	public UIPopupList popupListDoors;

	void Awake () {
		instance = this;
	}

	[PunRPC]public void SetRotation(float angle){
		NGUITools.SetActive (arrowTexture.gameObject, true);
		arrowTexture.eulerAngles = Vector3.Lerp(arrowTexture.eulerAngles,new Vector3 (arrowTexture.eulerAngles.x, arrowTexture.eulerAngles.y, angle+0),Time.deltaTime + 10);
		//rotationLabel.text = angle.ToString ();
	}

	public void OnPopUpSelectionChange(){
		foreach (MusicTypeData mtd in types) {
			if(mtd.name.Equals(popupList.value.ToString())){
				popupList.GetComponent<UISprite>().color = mtd.color;
				currentUType = mtd.uType;
				return;
			}

		}

	}

	public void OnPopUpDoorChange(){
		foreach (DoorType dt in doors) {
			if(dt.name.Equals(popupListDoors.value.ToString())){

				currentDoor = dt.ID;
				return;
			}
			
		}
		
	}



	public void ShowInputPanel(){
		NGUITools.SetActive (loadingWindow, false);
		NGUITools.SetActive(loginPanel,true);

		AudioManager.instance.source.Stop ();
	}
	public UILabel playernameLbl;

	public void HideInputPanel(){
		playernameLbl.text = PhotonNetwork.playerName;
		NGUITools.SetActive(loginPanel,false);

	}

	public void OnClickStop(){
		photonView.RPC ("ReceiveStop", PhotonTargets.All, PhotonNetwork.playerName);
		NGUITools.SetActive(stopButton,false);
		NGUITools.SetActive(continueButton,true);
	}

	public void OnCLickCOntinue(){
		photonView.RPC ("ReceiveContinue", PhotonTargets.All, PhotonNetwork.playerName);
		NGUITools.SetActive(stopButton,true);
		NGUITools.SetActive(continueButton,false);
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

	public void OnClick_LeaveRoom(){
		PhotonNetwork.LeaveRoom ();
	}

	[PunRPC]public void WayCompleted(){
		if (!GameManager.instance.isDirector) {
			//tratar mensaje
			NGUITools.SetActive(completedWindow,true);
			foreach(MusicTypeData mtd in types){
				if(mtd.uType == currentUType){
					AudioManager.instance.PlayClip(mtd.clip);
				}
			}
		}
	}

	[PunRPC]public void PlayerLost(){
		if (!GameManager.instance.isDirector) {
			//tratar mensaje
			NGUITools.SetActive(lostWindow,true);
//			foreach(MusicTypeData mtd in types){
//				if(mtd.uType == currentUType){
//					AudioManager.instance.PlayClip(mtd.clip);
//				}
//			}
		}
	}


}
