using UnityEngine;
using System.Collections;

public class TargetVisualizer : MonoBehaviour {
	public int segments = 24;
	public float radius = 0.3f;

	protected void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;

		Gizmos.DrawWireSphere(transform.position, 0.01f);

		float angleSize = 2 * Mathf.PI / segments;

		for(int i = 0; i < segments; ++i)
		{
			Gizmos.DrawLine(
				transform.position + radius * new Vector3(Mathf.Cos(i * angleSize), 0.01f, Mathf.Sin(i * angleSize)),
				transform.position + radius * new Vector3(Mathf.Cos((i + 1) * angleSize), 0.01f, Mathf.Sin((i + 1) * angleSize)));
		}
	}
}
