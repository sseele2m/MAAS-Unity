using UnityEngine;
using System.Collections;

public class LightIndicator : MonoBehaviour {
	public Color color = Color.red;
	public float frequency = 1f;
	public float maxIntensity = 1f;

	public bool busy = false;
	private bool wasBusy = false;

	private Light lightSrc = null;

	// Use this for initialization
	void Start () {
		lightSrc = transform.FindChild("Light").GetComponent<Light>();
		lightSrc.color = color;

		//transform.FindChild("Base").GetComponent<MeshRenderer>().renderer.material.color = color;
	}
	
	// Update is called once per frame
	void Update () {
		if(busy)
		{
			lightSrc.intensity = Mathf.Sin(Time.time * frequency) * 0.5f + 0.5f;
			lightSrc.intensity *= maxIntensity;
		}

		if(busy != wasBusy)
		{
			lightSrc.intensity = 0f;
			wasBusy = busy;
		}
	}
}
