﻿using System.Net.Sockets;
using UnityEngine;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.Generic;

public class CommunicationServer {
	enum ConnectionState
	{
		WAITING_FOR_CONNECTION,
		WAITING_FOR_ID_RESPONSE,
		CONNECTED
	}

	public enum ClientType
	{
		KIVA,
		ROCKIN
	}

	TcpListener serverSocket;
	TcpClient clientSocket;
	NetworkStream stream;

	public Queue<string> ServerMessageQueue = new Queue<string>();
	public Queue<string> Inbox = new Queue<string>();
	public Queue<string> Outbox = new Queue<string>();

	public bool ClientConnected
	{
		get
		{
			if(connectionState == ConnectionState.CONNECTED)
				return true;

			return false;
		}
	}

	ConnectionState connectionState = ConnectionState.WAITING_FOR_CONNECTION;
	readonly float ID_RESPONSETIMEOUT = 60f;
	readonly float MSG_RESPONSETIMEOUT = 1f;
	float waitTime;

	public ClientType clientType
	{
		get;
		protected set;
	}

	public CommunicationServer(int portNumber)
	{
		try
		{
			serverSocket = new TcpListener(IPAddress.Any, portNumber);
			clientSocket = default(TcpClient);
			serverSocket.Start();
			ServerMessageQueue.Enqueue("Server started");
		}
		catch(SocketException e)
		{
			Debug.LogError(e.Message);
		}
	}

	public void Update()
	{
		switch(connectionState)
		{
		case ConnectionState.WAITING_FOR_CONNECTION:
			AcceptConnection();
			break;
		case ConnectionState.WAITING_FOR_ID_RESPONSE:
			WaitForClientID();
			break;
		case ConnectionState.CONNECTED:
			ReadMessage();
			SendMessage();
			break;
		}
	}

	public bool AcceptConnection()
	{
		if(!serverSocket.Pending())
			return false;

		clientSocket = serverSocket.AcceptTcpClient();
		stream = clientSocket.GetStream();


		ServerMessageQueue.Enqueue("Accepted client connection. Waiting for ID.");

		connectionState = ConnectionState.WAITING_FOR_ID_RESPONSE;
		waitTime = Time.time;

		return true;
	}

	public void WaitForClientID()
	{
		if(stream.DataAvailable)
		{
			int i;
			byte[] bytes = new byte[1024];
			string incoming;
			
			i = stream.Read(bytes, 0, bytes.Length);
			
			incoming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
			string[] splitMsgs = incoming.Split(new char[]{'\n'});

			if(splitMsgs.Length > 0)
			{
				splitMsgs[0] = splitMsgs[0].Replace("\r\n", "").Replace("\r","").Replace("\n","");
				if(splitMsgs[0].ToLower() == "rockin")
				{
					clientType = ClientType.ROCKIN;
					ServerMessageQueue.Enqueue("Connected client is for Rockin setup.");
				}
				else if(splitMsgs[0].ToLower() == "kiva")
				{
					clientType = ClientType.KIVA;
					ServerMessageQueue.Enqueue("Connected client is for Kiva setup.");
				}
			}
			else
			{
				ServerMessageQueue.Enqueue(string.Format("Unrecognized client type: {0}. Disconnecting.", incoming));
				CleanUpClient();
				return;
			}

			// If we received more than just the client type, put all of it into the inbox.
			if(splitMsgs.Length > 1)
			{
				Debug.Log("received more than type");
				string msg;
				for(int index = 1; index < splitMsgs.Length; ++index)
				{
					msg = splitMsgs[index].Replace("\r\n", "").Replace("\r","").Replace("\n","");
					if(msg.Length > 0)
					{
						Inbox.Enqueue(msg);
					}
				}
			}
			
			connectionState = ConnectionState.CONNECTED;
			waitTime = Time.time;
		}
		else if(Time.time - waitTime >= ID_RESPONSETIMEOUT)
		{
			ServerMessageQueue.Enqueue("Failed to receive ID from client. Disconnecting.");
			CleanUpClient();
		}
	}

	public void ReadMessage()
	{
		// Test how much time we have been waiting for a message.
		if(Time.time - waitTime >= MSG_RESPONSETIMEOUT)
		{
			TestClientConnection();
			waitTime = Time.time; // Reset the timer.
		}

		if(clientSocket == null || !stream.DataAvailable)
		{
			return;
		}

		int i;
		byte[] bytes = new byte[1024];
		string incoming;

		i = stream.Read(bytes, 0, bytes.Length);

		incoming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
		string[] splitMsgs = incoming.Split(new char[]{'\n'});
		string msg;
		foreach (string str in splitMsgs)
		{
			msg = str.Replace("\r\n", "").Replace("\r","").Replace("\n","");
			if(msg.Length > 0)
				Inbox.Enqueue(msg);
		}
	}

	public void SendMessage()
	{
		if(Outbox.Count == 0)
			return;

		try
		{
			if(clientSocket == null)
			{
				Outbox.Clear();
				ServerMessageQueue.Enqueue("Cannot send message to client. Client is no longer connected.");
			}

			// In order for the client to receive a message, it needs to be ended with \r\n!
			string str = Outbox.Dequeue() + "\r\n";
			byte[] msg = System.Text.Encoding.ASCII.GetBytes(str);

			stream.Write(msg, 0, msg.Length);
		}
		catch(SocketException)
		{
			CleanUpClient();
		}
	}

	public void TestClientConnection()
	{
		try
		{
			if(clientSocket.Client.Poll(0, SelectMode.SelectRead))
			{
				byte[] buff = new byte[1];
				if(clientSocket.Client.Receive(buff, SocketFlags.Peek) == 0)
				{
					CleanUpClient();
				}
			}
		}
		catch(SocketException)
		{
			CleanUpClient();
		}
	}

	protected void CleanUpClient()
	{
		ServerMessageQueue.Enqueue("Closing socket");
		clientSocket.Close();
		clientSocket = null;
		connectionState = ConnectionState.WAITING_FOR_CONNECTION;
	}

	public void ShutDown()
	{
		if(clientSocket != null)
			clientSocket.Close();
		if(serverSocket != null)
			serverSocket.Stop();
		ServerMessageQueue.Enqueue("Server stopped");
	}
}
