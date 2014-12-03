using UnityEngine;
using System.Collections;

public class youBotNavAgent : MonoBehaviour {
	public Transform target;
	Transform currentTarget;

	NavMeshAgent navMeshAgent;
	Controller controller;
	bool notifiedController = false;

	// Use this for initialization
	void Start () {
		navMeshAgent = GetComponent<NavMeshAgent>();

		controller = GameObject.FindObjectOfType(typeof(Controller)) as Controller;
		controller.RegisterActor(transform);
	}
	
	// Update is called once per frame
	void Update () {
		if(target == null)
			return;

		if(target != currentTarget)
		{
			currentTarget = target;
			navMeshAgent.destination = currentTarget.position;
			notifiedController = false;
		}

		if(!notifiedController)
		{
			Vector3 targetPos = transform.InverseTransformPoint(currentTarget.transform.position);
			targetPos.y = 0f;

			if(targetPos.magnitude < navMeshAgent.radius + 0.1f)
			{
				controller.EnqueueMessage(
					string.Format("{0} arrived at {1}", name, currentTarget.transform.parent.name),
					Controller.GuiMessageType.GMT_ACTOR
					);
				notifiedController = true;
				target = currentTarget = null;
			}
		}
	}
}
