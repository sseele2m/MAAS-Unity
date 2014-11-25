using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	protected void RegisterWithController()
	{
		((Controller) GameObject.FindObjectOfType(typeof(Controller))).RegisterCameraController(this);
	}

	void OnGUI()
	{
		GUI.Label(new Rect(50,50,200,20), gameObject.name);
	}
}
