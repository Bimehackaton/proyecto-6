using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {
	public static HUDController instance;

	public Camera cameraUI;
	public Camera camera3D;

	public GameObject menuPanel;

	// Use this for initialization
	void Awake () {
		instance = this;
	}
	

	public void OnStartGame ()
	{
		NGUITools.SetActive (menuPanel, false);
	}
	
}
