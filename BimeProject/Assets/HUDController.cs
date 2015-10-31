using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {
	public static HUDController instance;

	public Camera cameraUI;
	public Camera camera3D;

	// Use this for initialization
	void Awake () {
		instance = this;
	}
	

}
