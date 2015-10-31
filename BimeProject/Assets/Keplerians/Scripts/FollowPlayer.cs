using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	Camera uiCam;
	Camera cam3D;
	public Transform target;
	public Vector3 offset = Vector3.zero;

	void Start () {
		uiCam = HUDController.instance.cameraUI;
		cam3D = HUDController.instance.camera3D;

	}
	
	void Update () {
		if (target == null || uiCam == null || cam3D == null)
			return;

		Vector3 vpPoint = cam3D.WorldToViewportPoint (target.position);
		Vector3 uiPos = uiCam.ViewportToWorldPoint (vpPoint);
		uiPos.z = 0;

		transform.position = uiPos + offset;
	}
}
