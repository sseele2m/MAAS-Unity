using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class StationAgent : MonoBehaviour {
	protected Controller controller;
	protected LightIndicator lightIndicator;
	protected Transform InQueueMount = null;
	protected Transform OutQueueMount = null;
	protected Transform WorkMount = null;

	protected List<Transform> InQueue = new List<Transform>();
	protected List<Transform> OutQueue = new List<Transform>();

	protected void InitStation()
	{
		controller = GameObject.FindObjectOfType(typeof(Controller)) as Controller;
		controller.RegisterActor(transform);
		
		lightIndicator = GetComponentInChildren<LightIndicator>();
		
		InQueueMount = transform.FindChild("InQueue");
		OutQueueMount = transform.FindChild("OutQueue");
		WorkMount = transform.FindChild("WorkMount");
	}

	// So far this only turns on the light indicator.
	// Later animation and/or sounds could be played.
	// Particles could also be emitted.
	public abstract bool StartWorkingOn(Transform order);

	// So far this only turns off the light indicator.
	// Later animation and/or sounds might have to be turned off.
	public abstract bool StopWorking();

	public virtual bool AddOrder(Transform order)
	{
		if(InQueue.Contains(order))
			return false;

		InQueue.Add(order);
		UpdateQueues();

		return true;
	}

	public virtual bool RemoveOrder(Transform order)
	{
		if(!OutQueue.Contains(order))
			return false;

		OutQueue.Remove(order);
		UpdateQueues();

		return true;
	}

	protected void UpdateQueues()
	{
		if(InQueueMount != null)
		{
			float offset = 0f;
			foreach(Transform orderBox in InQueue)
			{
				orderBox.position = InQueueMount.position + offset * Vector3.up;
				orderBox.rotation = InQueueMount.rotation;
				offset += orderBox.renderer.bounds.size.z;
			}
		}

		if(OutQueueMount != null)
		{
			float offset = 0f;
			foreach(Transform orderBox in OutQueue)
			{
				orderBox.position = OutQueueMount.position + offset * Vector3.up;
				orderBox.rotation = OutQueueMount.rotation;
				offset += orderBox.renderer.bounds.size.z;
			}
		}
	}
}
