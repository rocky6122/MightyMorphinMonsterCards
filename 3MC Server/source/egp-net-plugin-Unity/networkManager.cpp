/////////////////////////////////////////////
// ChampNet| Written by John Imgrund (c) 2019
/////////////////////////////////////////////

#include "networkManager.h"
#include "NetworkingInstance.h"

//Starters

void hostGame()
{
	//YOU ARE A SERVER

	//Get the Networking Instance
	NetworkingInstance& gpInstance = NetworkingInstance::getInstance();

	//The socket descriptor allocates the socket to be used while the game is running
	RakNet::SocketDescriptor selectedSocket(gpInstance.getServerPort(), 0);
	gpInstance.getPeer()->Startup(gpInstance.getMaxClients(), &selectedSocket, 1);

	//Let the server accept incoming connections from the clients
	printf("Starting the game.\n");
	gpInstance.getPeer()->SetMaximumIncomingConnections(gpInstance.getMaxClients());
}

//Handlers

void hostHandleNetworking()
{
	//Runs to read for IDs related hosting a game
	hostRead();
	//Runs to write host messages to send to clients relating to a game
	hostWrite();
}

//Reader

void hostRead()
{
	//Get the NetworkingInstance
	NetworkingInstance& gpInstance = NetworkingInstance::getInstance();

	//Create temp packet to handle incoming
	RakNet::Packet *packet;

	for (
		packet = gpInstance.getPeer()->Receive();
		packet;
		gpInstance.getPeer()->DeallocatePacket(packet), packet = gpInstance.getPeer()->Receive()
		)
	{
		switch (packet->data[0])
		{
		case ID_NEW_INCOMING_CONNECTION:
		{
			printf("A new client is connecting.\n");
		}
		break; //End of ID_NEW_INCOMING_CONNECTION

		case ID_DISCONNECTION_NOTIFICATION:
		{
			printf("Server Disconnected.\n");
		}
		break; //End of ID_DISCONNECTION_NOTIFICATION

		case ID_CONNECTION_LOST:
		{
			printf("Server lost connection lost.\n");
		}
		break; //End of ID_CONNECTION_LOST

		case ID_SEND_USER_DATA:
		{
			printf("UserData received\n");

			if (gpInstance.getCurrentClientNum() == 0) //Host (Sets host data into the array on first run through)
			{
				//Creates a new userData for Host
				UserData hostData;

				//Copy host data into the object
				strcpy(hostData.userName, "Host");
				
				//Place the userdata into the list of client data then update the total clientNum
				UserData* tempClientData = gpInstance.getClientData();

				//Place the hostData onto the ClientData
				tempClientData[gpInstance.getCurrentClientNum()] = hostData;
				gpInstance.addToClientNum(1);
			}

			//Create incoming BitStream to read
			RakNet::BitStream incomingStream(packet->data, packet->length, false);

			//Ignore the MessageID
			incomingStream.IgnoreBytes(sizeof(RakNet::MessageID));

			//Reads the bitstream in as the correct type of NetworkMessage
			UserData userData[1];
			incomingStream.Read(userData);

			//Place the userData onto the list
			gpInstance.getClientData()[gpInstance.getCurrentClientNum()] = *userData;
			gpInstance.addToClientNum(1);

			////Send the player their player number
			PlayerNum* numMsg = new PlayerNum();

			numMsg->networkMessageID = ID_PLAYER_NUM;
			numMsg->playerNum = gpInstance.getCurrentClientNum() - 1;


			//-------------------------------------------------
			//Quickly send the playerNum to correct player
			RakNet::BitStream outgoingStream;

			//Write the message ID in
			outgoingStream.Write(numMsg->networkMessageID);

			outgoingStream.Write(*numMsg);

			//Send the message
			gpInstance.getPeer()->Send(&outgoingStream, HIGH_PRIORITY, RELIABLE_ORDERED, 0, packet->guid, false);

			delete numMsg;
			//--------------------------------------------------


			if (gpInstance.getCurrentClientNum() == 3) //Two clients plus host
			{
				StartGame* msg = new StartGame();

				msg->networkMessageID = ID_START_GAME;

				gpInstance.addToSender(msg);

				gpInstance.setGameMode(1);
			}

			//Show whos joined the game via server
			printf("%s has joined the game.\n", userData->userName);
		}
		break; //End of ID_SEND_USER_DATA

		case ID_MESSAGE_DATA:
		{
			printf("ChatMessage Received.\n");

			//Create incoming BitStream to read
			RakNet::BitStream incomingStream(packet->data, packet->length, false);

			//Ignore the MessageID
			incomingStream.IgnoreBytes(sizeof(RakNet::MessageID));

			//Reads the bitstream in as the correct type of NetworkMessage
			ChatMessage* message = new ChatMessage();
			incomingStream.Read(*message); //Must be *message to pass by reference

			//Add the networkMessageID back in case it was lost in transit
			message->networkMessageID = ID_MESSAGE_DATA;

			printf("UserName: %s\n", message->userName);
			printf("Message: %s\n", message->message);

			//Since it is going to be routed directly send it directly to the Sender Queue
			gpInstance.addToSender(message);
		}
		break; //End of ID_MESSAGE_DATA

		case ID_PLAYER_NUM:
		{
			printf("Missing Card Message Received.\n");

			//Create incoming BitStream to read
			RakNet::BitStream incomingStream(packet->data, packet->length, false);

			//Ignore the MessageID
			incomingStream.IgnoreBytes(sizeof(RakNet::MessageID));

			//Reads the bitstream in as the correct type of NetworkMessage
			PlayerNum missingChampion[1];
			incomingStream.Read(missingChampion);

			//-------------------------------------------------
			//Quickly send the missingChampion to the correct player
			ChampionData* message = new ChampionData();

			message->networkMessageID = ID_CHAMPION_DATA;
			gpInstance.getPlayerChampion(missingChampion->playerNum, message); //Fill in all the championData

			RakNet::BitStream outgoingStream;

			//Write the message ID in
			outgoingStream.Write(message->networkMessageID);
			
			outgoingStream.Write(*message);

			//Send the message
			gpInstance.getPeer()->Send(&outgoingStream, IMMEDIATE_PRIORITY, RELIABLE_ORDERED, 0, packet->guid, false);

			delete message;
			//--------------------------------------------------
		}
		break; //End of ID_PLAYER_NUM

		case ID_CHAMPION_DATA:
		{
			printf("ChampionData Received.\n");

			//Create incoming BitStream to read
			RakNet::BitStream incomingStream(packet->data, packet->length, false);

			//Ignore the MessageID
			incomingStream.IgnoreBytes(sizeof(RakNet::MessageID));

			//Read the bitstream in as the correct type of NetworkMessage
			ChampionData* message = new ChampionData();
			incomingStream.Read(*message); //Must be *message to pass by reference

			//Add the networkMessageID back in case it was lost in transit
			message->networkMessageID = ID_CHAMPION_DATA;

			printf("Card Name: %s \n", message->monsterName);
			printf("Monster health: %d\n", message->monsterHealth);
			printf("Monster Speed: %d \n", message->monsterSpeed);
			printf("Monster Element: %d \n", message->element);
			printf("Monster Alloy: %d \n", message->alloy);
			printf("Player Number: %d \n", message->playerNum);

			//Sets championSpeed for correct player for server Attacks
			if (message->playerNum == 1)
			{
				gpInstance.setPlayerOneSpeed((message->monsterSpeed / 100.0f)); //Turn it into a percentage
			}
			else if (message->playerNum == 2)
			{
				gpInstance.setPlayerTwoSpeed((message->monsterSpeed / 100.0f)); //Turn it into a percentage
			}

			//Set the card info inside the networking instance in case of failure
			gpInstance.setPlayerChampion(message->playerNum, *message);

			//Route the message
			gpInstance.addToSender(message);
		}
		break; //End of ID_CHAMPION_DATA

		case ID_END_GAME:
		{
			printf("End Game Message Received.\n");

			//Create incoming BitStream to read
			RakNet::BitStream incomingStream(packet->data, packet->length, false);

			//Ignore the MessageID
			incomingStream.IgnoreBytes(sizeof(RakNet::MessageID));

			//Reads the bitstream in as the correct type of NetworkMessage
			EndGame* message = new EndGame();
			incomingStream.Read(*message); //Must be *message to pass by reference

										   //Add the networkMessageID back in case it was lost in transit
			message->networkMessageID = ID_END_GAME;

			printf("Winner Name: %s\n", message->winnerName);

			//Set Game to end state
			gpInstance.setGameMode(3);

			//-------------------------------------------------
			//Quickly send the EndGame to correct player
			RakNet::BitStream outgoingStream;

			//Write the message ID in
			outgoingStream.Write(message->networkMessageID);

			outgoingStream.Write(*message);

			//Send the message
			gpInstance.getPeer()->Send(&outgoingStream, HIGH_PRIORITY, RELIABLE_ORDERED, 0, packet->guid, true);
			//--------------------------------------------------
		}
		break; //End of ID_END_GAME

		default:
			printf("Message with identifier %i has arrived.\n", packet->data[0]);
			break;
		}
	}
}

//Writer

void hostWrite()
{
	NetworkingInstance& gpInstance = NetworkingInstance::getInstance();
	RakNet::BitStream outgoingStream;

	while (!gpInstance.getSenderQueue().empty())
	{
		//peep the front
		ChampNetMessage* messageToSend = gpInstance.getSenderQueue().front();

		//Pop the front off the queue
		gpInstance.getSenderQueue().pop();

		//Write the message ID in
		outgoingStream.Write(messageToSend->networkMessageID);

		//CANT TELL WHETHER OR NOT U HAVE TO CAST THE MESSAGE TO ITS CORRECT TYPE BEFORE U SEND IT
		switch (messageToSend->networkMessageID)
		{
		case ID_SEND_USER_DATA:
		{
			printf("Sending User Data!\n");
			UserData* finalMessageToSend = static_cast<UserData*>(messageToSend);
			outgoingStream.Write(*finalMessageToSend); //Dereference it for sending
		}
		break;
		case ID_MESSAGE_DATA:
		{
			printf("Sending Chat Message!\n");
			ChatMessage* finalMessageToSend = static_cast<ChatMessage*>(messageToSend);
			outgoingStream.Write(*finalMessageToSend); //Dereference it for sending
		}
		break;
		case ID_PLAYER_NUM:
		{
			printf("Sending PlayerNum Message!\n");
			PlayerNum* finalMessageToSend = static_cast<PlayerNum*>(messageToSend);
			outgoingStream.Write(*finalMessageToSend); //Dereference it for sending
		}
		break;
		case ID_START_GAME:
		{
			printf("Sending Start Game Message!\n");
			StartGame* finalMessageToSend = static_cast<StartGame*>(messageToSend);
			outgoingStream.Write(*finalMessageToSend); //Dereference it for sending
		}
		break;
		case ID_DRAW_CARD:
		{
			printf("Sending Draw Card Message!\n");
			DrawCard* finalMessageToSend = static_cast<DrawCard*>(messageToSend);
			outgoingStream.Write(*finalMessageToSend); //Dereference it for sending
		}
		break;
		case ID_BATTLE_PHASE:
		{
			printf("Sending BattlePhase Message!\n");
			BattlePhase* finalMessageToSend = static_cast<BattlePhase*>(messageToSend);
			outgoingStream.Write(*finalMessageToSend); //Dereference it for sending
		}
		break;
		case ID_CHAMPION_DATA:
		{
			printf("Sending ChampionData Message!\n");
			ChampionData* finalMessageToSend = static_cast<ChampionData*>(messageToSend);

			printf("Card Name: %s \n", finalMessageToSend->monsterName);
			printf("Monster health: %d\n", finalMessageToSend->monsterHealth);
			printf("Monster Speed: %d \n", finalMessageToSend->monsterSpeed);
			printf("Monster Element: %d \n", finalMessageToSend->element);
			printf("Monster Alloy: %d \n", finalMessageToSend->alloy);
			printf("Player Number: %d \n", finalMessageToSend->playerNum);

			outgoingStream.Write(*finalMessageToSend); //Dereference it for sending
		}
		break;
		case ID_ATTACK:
		{
			printf("Sending Attack Message!\n");
			AttackMessage* finalMessageToSend = static_cast<AttackMessage*>(messageToSend);
			outgoingStream.Write(*finalMessageToSend); //Dereference it for sending
		}
		break;
		case ID_END_GAME:
		{
			printf("Sending End Game Message!\n");
			EndGame* finalMessageToSend = static_cast<EndGame*>(messageToSend);
			outgoingStream.Write(*finalMessageToSend); //Dereference it for sending
		}
		break;

		default:
			printf("Error! This GameMessage doesn't exist.");
		}

		//Send the message
		gpInstance.getPeer()->Send(&outgoingStream, IMMEDIATE_PRIORITY, RELIABLE_ORDERED, 0, gpInstance.getPeer()->GetMyGUID(), true);

		delete messageToSend;
	}
}