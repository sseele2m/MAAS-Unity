using UnityEngine;
using System.Collections;

public class OutgoingStationAgent : StationAgent {

	// Use this for initialization
	void Start () {
		InitStation();
	}

	public override bool RemoveOrder (Transform order)
	{
		if(!InQueue.Contains(order))
			return false;

		InQueue.Remove(order);
		UpdateQueues();

		return true;
	}

	public override bool StartWorkingOn (Transform order)
	{
		// TODO: Maybe do sth. like activate the light indicator for a certain time.
		return false;
	}

	public override bool StopWorking ()
	{
		return false;
	}
}
