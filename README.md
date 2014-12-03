MAAS-Unity
==========

Visualization for the MAAS winter term 2014/15 projects ((Rockin/Kiva) based on the Unity Game Engine.
-----------------------------------------------------------------------------------------------------
Usage:
-----------------------------------------------------------------------------------------------------

- To connect to the server use port 4711 and the IP 127.0.0.1, if you are running it locally. I have not tested it on a network, but theoretically it should work.
- The general communication with the server is exemplified in the provided Java program.
- Once connected, you need to send "Rockin" or "Kiva" to identify which project setup you need.
- After identifying the setup you can send instructions to the server as string messages. The general structure of the messages is "<actor> <action> <target>". Actor and Target always need to be without spaces, e.g., Order123, youBot2, WeldingStation1, and there needs to be a space between each part.
So far existing actors/targets for the Rockin setup are:
youBot1
youBot2
IncomingStation1
OutgoingStation1
WeldingStation1
SolderingStation1
PressFittingStation1

The actors/targets for the Kiva setup are:
OrderPicker1
Robot1
Slot1-9
Shelf1-9

The available actions for Rockin are:
"moves to"
"is on" (to put orders on youBots)
is at (to put orders from youBots onto stations)
"started working on" (here the target needs to be an order that is in the incoming queue of the station)
"started welding/soldering/pressFitting" (here the target also needs to be an order in the incoming queue)
"stopped working" (no order required, b/c there can only be one being processed)
"arrived" (here the actor needs to be a new order, the name/string needs to be unique)
"shipped" (here the actor also needs to be an order, but it must be an existing on in the incoming queue of the OutgoingStation1)

The available actions for Kiva are:
"moves to"
"arrived" (so far only 3 orders can arrive)

-----------------------------------------------------------------------------------------------------
Further notes:
-----------------------------------------------------------------------------------------------------

- the Rockin setup is complete, i.e., an order can be completely processed from arrival to shipment (although it has not been thoroughly tested for bugs)
- the Kiva setup is by no means complete (not even partially), I just started setting it up
- to test the Visualization without having to type messages via the TcpClientExample program, you can bring up a menu using F1 and create a message using it (although I filter the actors/targets, the target list is still very long and I haven't gotten to reposition buttons on screen - so you might not be able to select the target you want, sorry...)