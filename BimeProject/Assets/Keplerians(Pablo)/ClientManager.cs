using UnityEngine;
using System.Collections;

public class ClientManager : Photon.MonoBehaviour {

	public static ClientManager instance;
	public UILabel rotationLabel;
	public GameObject loginPanel;
	public UIInput inputName;
	public UITexture arrowTexture;

	void Awake () {
		instance = this;
	}

	[PunRPC]public void SetRotation(float angle){
		NGUITools.SetActive (arrowTexture.gameObject, true);
		arrowTexture.transform.eulerAngles = new Vector3 (arrowTexture.transform.eulerAngles.x, arrowTexture.transform.eulerAngles.y, angle+90);
		//rotationLabel.text = angle.ToString ();
	}

	public void ShowInputPanel(){
		NGUITools.SetActive(loginPanel,true);
	}

	public void HideInputPanel(){
		NGUITools.SetActive(loginPanel,false);
	}


}
