using UnityEngine;
using System.Collections;

public class OrderPicker : MonoBehaviour {
	Controller controller;
	
	// Use this for initialization
	void Start () {
		controller = GameObject.FindObjectOfType(typeof(Controller)) as Controller;
		controller.RegisterActor(transform);
	}
}
