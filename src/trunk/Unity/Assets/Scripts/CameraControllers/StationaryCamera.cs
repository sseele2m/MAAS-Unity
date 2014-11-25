using UnityEngine;
using System.Collections;

public class StationaryCamera : CameraController {

	// Use this for initialization
	void Start () {
		RegisterWithController();
	}
	
	// Update is called once per frame
	void Update () {
		Transform viewTransform = Camera.main.camera.transform;

		viewTransform.position = transform.position;
		viewTransform.rotation = transform.rotation;
	}
}
