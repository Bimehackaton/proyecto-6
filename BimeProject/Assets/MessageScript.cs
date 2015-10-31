using UnityEngine;
using System.Collections;

public class MessageScript : MonoBehaviour {

	public UILabel messageLabel;

	public void InitMessage(string txt){
		messageLabel.text = txt;
		Destroy (gameObject, 3);
	}
}
