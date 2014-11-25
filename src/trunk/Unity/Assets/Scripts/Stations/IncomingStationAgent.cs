using UnityEngine;
using System.Collections;

public class IncomingStationAgent : StationAgent {

	// Use this for initialization
	void Start () {
		InitStation ();
	}
	
	public override bool AddOrder (Transform order)
	{
		if(OutQueue.Contains(order))
			return false;

		OutQueue.Add(order);
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
