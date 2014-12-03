using UnityEngine;
using System.Collections;

public class RobotNavAgent : MonoBehaviour {
	public Transform target;
	Transform currentTarget;

	NavMeshAgent navMeshAgent;
	Controller controller;
	bool notifiedController = false;

	enum State
	{
		IDLE,
		MOVING,
		POSITIONING,
		ARRIVED
	}
	State currentState = State.IDLE;
	readonly float shelfRadius = 0.8f;
	readonly float robotRadius = 0.3f;
	readonly float correctionTime = 1f;
	float elapsedTime = 0f;

	bool isEmpty = true;

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
			currentState = State.MOVING;
		}

		switch(currentState)
		{
		case State.MOVING:
			Vector3 targetPos = transform.InverseTransformPoint(currentTarget.transform.position);
			targetPos.y = 0f;
			
			if(targetPos.magnitude < navMeshAgent.radius + 0.1f && navMeshAgent.velocity.magnitude < 0.01f)
			{
				currentState = State.POSITIONING;
				navMeshAgent.updatePosition = false;
				navMeshAgent.updateRotation = false;
				elapsedTime = 0f;
			}
			break;
		case State.POSITIONING:
			elapsedTime += Time.deltaTime;
			transform.rotation = Quaternion.Lerp(transform.rotation, currentTarget.rotation, elapsedTime);
			transform.position = Vector3.Lerp(transform.position, currentTarget.position, elapsedTime);
			if(elapsedTime > correctionTime)
			{
				transform.rotation = currentTarget.rotation;
				transform.position = currentTarget.position;
				navMeshAgent.updatePosition = true;
				navMeshAgent.updateRotation = true;
				currentState = State.ARRIVED;
			}
			break;
		case State.ARRIVED:
			controller.EnqueueMessage(
				string.Format("{0} arrived at {1}", name, currentTarget.transform.parent.name),
				Controller.GuiMessageType.GMT_ACTOR
				);

			if(isEmpty)
			{
				// Attach the shelf to the robot.
				currentTarget.parent.GetComponentInChildren<Shelf>().transform.parent = transform;
				isEmpty = false;
				navMeshAgent.radius = shelfRadius;
			}
			else
			{
				if(currentTarget.parent.GetComponent<Slot>() != null)
				{
					GetComponentInChildren<Shelf>().transform.parent = currentTarget.parent;
					isEmpty = true;
					navMeshAgent.radius = robotRadius;
				}
				else if(currentTarget.parent.GetComponent<OrderPicker>() != null)
				{
					GetComponentInChildren<Shelf>().transform.parent = currentTarget.parent;
					isEmpty = true;
					navMeshAgent.radius = robotRadius;
				}
			}

			currentState = State.IDLE;
			target = currentTarget = null;
			break;
		case State.IDLE:
		default:
			break;
		}

//		if(!notifiedController)
//		{
//			Vector3 targetPos = transform.InverseTransformPoint(currentTarget.transform.position);
//			targetPos.y = 0f;
//
//			if(targetPos.magnitude < navMeshAgent.radius + 0.1f && navMeshAgent.velocity.magnitude < 0.01f)
//			{
//				controller.EnqueueMessage(
//					string.Format("{0} arrived at {1}", name, currentTarget.transform.parent.name),
//					Controller.GuiMessageType.GMT_ACTOR
//					);
//
//				//navMeshAgent.
//
//				// Attach the shelf to the robot.
//				currentTarget.parent.GetComponentInChildren<Shelf>().transform.parent = transform;
//
//				notifiedController = true;
//			}
//		}
	}
}
