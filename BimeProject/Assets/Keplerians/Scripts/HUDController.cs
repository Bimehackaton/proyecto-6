using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {
	public static HUDController instance;

	public Camera cameraUI;
	public Camera camera3D;

	public GameObject menuPanel;
	public GameObject lostWindow;

	// Use this for initialization
	void Awake () {
		instance = this;
	}
	

	public void OnStartGame ()
	{
		NGUITools.SetActive (menuPanel, false);
	}
	
	public void ShowLostWindow ()
	{
		NGUITools.SetActive (lostWindow, true);
	}
}
