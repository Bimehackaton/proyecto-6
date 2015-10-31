using UnityEngine;
using System.Collections;

public class PlayerClient : MonoBehaviour {

	private bool appliedInitialUpdate;
	public PhotonPlayer photonPlayer;

	void Awake(){

	}
	void Start(){

	}

	void Update(){

		ClientManager.instance.photonView.RPC ("SetRotation", photonPlayer, this.transform.eulerAngles.z);
	}

}
