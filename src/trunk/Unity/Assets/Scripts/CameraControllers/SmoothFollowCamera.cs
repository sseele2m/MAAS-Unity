using UnityEngine;
using System.Collections;

public class SmoothFollowCamera : CameraController
{
	// The distance in the x-z plane to the target
	public float distance = 5f;
	// the height we want the camera to be above the target
	public float height = 3f;
	public float heightDamping = 2f;
	public float rotationDamping = 3f;

	private void Start()
	{
		RegisterWithController();
	}

	private void Update()
	{
		// Get the camera
		Transform viewTransform = Camera.main.camera.transform;
		Transform newTransform = viewTransform;
		
		// Calculate the current rotation angles
		var wantedRotationAngle = transform.eulerAngles.y;
		var wantedHeight = transform.position.y + height;
			
		var currentRotationAngle = viewTransform.eulerAngles.y;
		var currentHeight = viewTransform.position.y;
		
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.fixedDeltaTime);
	
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
	
		// Convert the angle into a rotation
		var currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		newTransform.position = transform.position;
		newTransform.position -= currentRotation * Vector3.forward * distance;
	
		// Set the height of the camera
		newTransform.position = new Vector3(viewTransform.position.x, currentHeight, viewTransform.position.z);
		
		// Always look at the target
		newTransform.LookAt(transform);
		
		// Set the camera.
		viewTransform.position = newTransform.position;
		viewTransform.rotation = newTransform.rotation;
	}
}
