using UnityEngine;
using System.Collections;

public class CameraFrameGUI : MonoBehaviour {
	GUITexture texture;

	// Use this for initialization
	void Start () {
		texture = GetComponent<GUITexture>();
	}
	
	// Update is called once per frame
	protected void OnGUI() {
		// Adjust the camera frame texture to the screen size.
		Rect pixelInset = new Rect(
			Screen.width * -0.5f,
			Screen.height * -0.5f,
			Screen.width,
			Screen.height);

		texture.pixelInset = pixelInset;
	}
}
