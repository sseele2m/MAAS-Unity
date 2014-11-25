using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuiMessage
{
	public float creationTime
	{
		get;
		protected set;
	}

	public string message
	{
		get;
		protected set;
	}

	public Color displayColor
	{
		get;
		protected set;
	}

	public float length = 0f;

	public GuiMessage(float creationTime, string message, Color displayColor)
	{
		this.creationTime = creationTime;
		this.message = message;
		this.displayColor = displayColor;
	}
}

public class Controller : MonoBehaviour {
	CommunicationServer server;

	Dictionary<string, Transform> actors = new Dictionary<string, Transform>();
	List<CameraController> cameraControllers = new List<CameraController>();
	int currentCamController = 0;
	Queue<GuiMessage> guiMessageQueue = new Queue<GuiMessage>();
	float guiWidth = 200;

	readonly Color serverMessageColor = Color.red;
	readonly Color clientMessageColor = Color.green;
	readonly Color controllerMessageColor = Color.yellow;
	readonly Color actorMessageColor = Color.blue;
	readonly float displayTime = 20f; // 20 seconds

	public enum GuiMessageType
	{
		GMT_SERVER,
		GMT_CLIENT,
		GMT_CONTROLLER,
		GMT_ACTOR
	}

	// Use this for initialization
	void Start () {
		Application.runInBackground = true;
		server = new CommunicationServer(4711);
	}

	public void RegisterActor(Transform actor)
	{
		actors.Add(actor.name, actor);
	}

	public void RegisterCameraController(CameraController camController)
	{
		cameraControllers.Add(camController);

		// For now, make each new camera the active one.
		currentCamController = cameraControllers.Count - 1;
		if(currentCamController > 0)
		{
			cameraControllers[currentCamController - 1].enabled = false;
			cameraControllers[currentCamController].enabled = true;
		}
	}

	
	// Update is called once per frame
	void Update () {
		server.Update();
//		if(!clientConnected)
//		{
//			clientConnected = server.AcceptConnection();
//		}
//		else
//		{
//			// Read the next message that the server received.
//			string str = server.ReadMessage();
//			if(str == "") // If no message has arrived, check whether the client is still connected. -> TODO: Maybe don't do this as often.
//			{
//				clientConnected = server.ClientConnected();
//			}
//			else
//			{
//				EnqueueMessage(str, GuiMessageType.GMT_CLIENT);
//				ProcessMessage(str);
//			}
//		}

		// Process the client messages.
		if(server.Inbox.Count > 0)
		{
			string msg = server.Inbox.Dequeue();
			EnqueueMessage(msg, GuiMessageType.GMT_CLIENT);
			ProcessMessage(msg);
		}

		// Send all server messages from its outbox.
//		if(server.Outbox.Count > 0)
//		{
//			server.SendMessage();
//		}

		// Read all server messages from its inbox.
		while(server.ServerMessageQueue.Count > 0)
		{
			EnqueueMessage(server.ServerMessageQueue.Dequeue(), GuiMessageType.GMT_SERVER);
		}

		// Process user input.
		if(Input.GetKeyDown(KeyCode.C))
		{
			// Disable the current cam controller.
			cameraControllers[currentCamController].enabled = false;

			// Increment the index.
			++currentCamController;
			if(currentCamController == cameraControllers.Count)
				currentCamController = 0;

			// Activate the new cam controller.
			cameraControllers[currentCamController].enabled = true;
		}
	}

	public void EnqueueMessage(string message, GuiMessageType gmt)
	{
		Color msgColor;
		switch(gmt)
		{
		case GuiMessageType.GMT_SERVER:
			msgColor = serverMessageColor;
			message = "Server: " + message;
			break;
		case GuiMessageType.GMT_CLIENT:
			msgColor = clientMessageColor;
			message = "Client: " + message;
			break;
		case GuiMessageType.GMT_ACTOR:
			msgColor = actorMessageColor;
			server.Outbox.Enqueue(message);
			message = "Actor: " + message;
			break;
		case GuiMessageType.GMT_CONTROLLER:
		default:
			msgColor = controllerMessageColor;
			message = "Controller: " + message;
			break;
		}

		guiMessageQueue.Enqueue(new GuiMessage(Time.time, message, msgColor));
	}

	void OnDestroy()
	{
		server.ShutDown();
	}

	void OnGUI()
	{
		if(guiMessageQueue.Count < 1)
			return;

		GUI.Box(new Rect(5, Screen.height - 10 - guiMessageQueue.Count * 20, guiWidth + 10, guiMessageQueue.Count * 20), "");

		int numOfDequeues = 0;
		// Diplay messages in queue
		int index = 0;
		float maxWidth = 0f;
		Vector2 dim;
		foreach(GuiMessage gm in guiMessageQueue)
		{
			GUI.color = gm.displayColor;

			// Determine the label's width.
			if(gm.length == 0f)
			{
				dim = GUI.skin.label.CalcSize(new GUIContent(gm.message));
				gm.length = dim.x;
				if(gm.length > guiWidth)
				{
					guiWidth = gm.length;
				}
			}

			GUI.Label(new Rect(10,Screen.height - 10 - (guiMessageQueue.Count - index) * 20, guiWidth, 20), gm.message);

			// Determine whether this message has expired.
			if(Time.time - gm.creationTime > displayTime)
			{
				++numOfDequeues;
			}

			// Determine what the maximum required width is.
			if(gm.length > maxWidth)
			{
				maxWidth = gm.length;
			}

			++index;
		}

		// Remove all messages that have expired.
		for(int i = 0; i < numOfDequeues; ++i)
		{
			guiMessageQueue.Dequeue();
		}

		// Adjust the width of the rectangles.
		if(maxWidth < guiWidth)
		{
			guiWidth = maxWidth;
		}
	}

	void ProcessMessage(string str)
	{
		string[] split = str.Split(new char[]{' '});
		
		Transform actor = null;
		Transform target = null;
		
		// Determine the actor.
		if(actors.ContainsKey(split[0]))
		{
			actor = actors[split[0]];
		}
		else
		{
			// It might be a new actor.
			// So far only orders can appear.
			if(split[0].StartsWith("Order"))
			{
				GameObject newOrder = Instantiate(Resources.Load<GameObject>("Order")) as GameObject;
				newOrder.name = split[0];
				actor = newOrder.transform;
			}
			else
			{
				EnqueueMessage( 
				               string.Format("Cannot perform action, '{0}' could not be found", split[0]),
				               GuiMessageType.GMT_CONTROLLER);
				return;
			}
		}
		
		switch(split[1])
		{
		case "moves":
			if(actors.ContainsKey(split[3]))
			{
				target = actors[split[3]];
				MovesTo(youBot: actor, station: target);
			}
			else
			{
				EnqueueMessage( 
	               string.Format("Cannot move to '{0}', there is no such target", split[3]),
	               GuiMessageType.GMT_CONTROLLER);
			}
			break;
		case "started":
		case "starts":
			// Message will either be "started working on" or "started welding/soldering/press_fitting/etc." <ordername>.
			string strTarget = split[2] == "working" ? split[4] : split[3];
			if(!actors.ContainsKey(strTarget))
			{
				EnqueueMessage( 
	               string.Format("{0} cannot work on '{1}', there is no such order.", actor.name, target.name),
	               GuiMessageType.GMT_CONTROLLER);
				return;
			}
			target = actors[strTarget];
			WorksOn(station: actor, order: target);
			break;
		case "stopped":
		case "stops":
			Stops(station: actor);
			break;
		case "arrived":
		case "arrives":
			if(!actors.ContainsKey(actor.name))
				Arrives(order: actor);
			else
			{
				EnqueueMessage( 
	               string.Format("{0} cannot arrive, it is already there.", actor.name),
	               GuiMessageType.GMT_CONTROLLER);
			}
			break;
		case "is":if(actors.ContainsKey(split[3]))
			{
				target = actors[split[3]];
				if(split[2] == "on")
					IsOn(order: actor, youBot: target);
				else if (split[2] == "at")
					IsAt(order: actor, station: target);
			}
			else
			{
				EnqueueMessage( 
	               string.Format("Cannot place {0} at/on '{1}', there is no such target", actor, split[3]),
	               GuiMessageType.GMT_CONTROLLER);
			}
			
			break;
		case "shipped":
		case "ships":
			Ships(order: actor);
			break;
		default:
			EnqueueMessage("No action to be performed", GuiMessageType.GMT_CONTROLLER);
			break;
		}
	}

#region Actions
	void MovesTo(Transform youBot, Transform station)
	{
		youBot.GetComponent<youBotNavAgent>().target = station.FindChild("Target");
	}

	void WorksOn(Transform station, Transform order)
	{
		if(!station.GetComponent<StationAgent>().StartWorkingOn(order))
		{
			EnqueueMessage( 
	               string.Format("{0} cannot work on '{1}', it is not in its queue, not allowed, or another order is being worked on.", station.name, order.name),
	               GuiMessageType.GMT_CONTROLLER);
		}
	}

	void Stops(Transform station)
	{
		if(!station.GetComponent<StationAgent>().StopWorking())
		{
			EnqueueMessage( 
		       string.Format("{0} cannot stop working, it is not working.", station.name),
		       GuiMessageType.GMT_CONTROLLER);
		}
	}

	void Arrives(Transform order)
	{
		actors.Add(order.name, order); // Add the actor to the dictionary.
		actors["IncomingStation"].GetComponent<IncomingStationAgent>().AddOrder(order);
		order.parent = actors["IncomingStation"];
	}

	void IsOn(Transform order, Transform youBot)
	{
		if(youBot.childCount > 2)
		{
			EnqueueMessage( 
               string.Format("Cannot place {0} on {1}, it already carries another order", order.name, youBot.name),
               GuiMessageType.GMT_CONTROLLER);
		}
		StationAgent station = order.parent.GetComponent<StationAgent>();
		if(station != null && station.RemoveOrder(order))
		{
			Transform mount = youBot.FindChild("BoxMount");
			order.position = mount.position;
			order.rotation = mount.rotation;
			order.parent = youBot;
		}
		else
		{
			EnqueueMessage( 
               string.Format("Cannot place {0} on {1}, the order has not been processed by or is not on {2} ", order.name, youBot.name, station.name),
               GuiMessageType.GMT_CONTROLLER);
		}
	}

	void IsAt(Transform order, Transform station)
	{
		if(station.GetComponent<StationAgent>().AddOrder(order))
		{
			order.parent = station;
		}
		else
		{
			EnqueueMessage( 
               string.Format("{0} cannot be placed on '{1}', it is already on it.", order.name, station.name),
               GuiMessageType.GMT_CONTROLLER);
		}
	}

	void Ships(Transform order)
	{
		if(actors["OutgoingStation"].GetComponent<OutgoingStationAgent>().RemoveOrder(order))
		{
			actors.Remove(order.name);
			Destroy(order.gameObject);
		}
		else
		{
			EnqueueMessage( 
               string.Format("{0} cannot be shipped, it is not at the outgoing station.", order.name),
               GuiMessageType.GMT_CONTROLLER);
		}
	}
#endregion
}
