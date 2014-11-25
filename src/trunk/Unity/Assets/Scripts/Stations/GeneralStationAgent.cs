using UnityEngine;
using System.Collections;

public class GeneralStationAgent : StationAgent {
	protected Transform currentOrder = null;

	// Use this for initialization
	void Start () {
		InitStation();
	}

	public override bool StartWorkingOn(Transform order)
	{
		if(!InQueue.Contains(order) && currentOrder == null)
			return false;

		// Indicate that the station is busy using the light indicator.
		lightIndicator.busy = true;
		// Remember which order the station is currently working on.
		currentOrder = order;

		// Remove the order from the queue visualization.
		InQueue.Remove(order);
		UpdateQueues();

		// Update the order's transform.
		order.position = WorkMount.position;
		order.rotation = WorkMount.rotation;
		
		return true;
	}

	public override bool StopWorking()
	{
		if(currentOrder == null)
			return false;
		
		lightIndicator.busy = false;
		OutQueue.Add(currentOrder);
		UpdateQueues();
		currentOrder = null;
		
		return true;
	}
}
