////////////////////////////////////////////
// ChampNet| Written by John Imgrund(c) 2019
////////////////////////////////////////////

#include "networkManager.h"
#include "NetworkingInstance.h"

//Starters
void joinGame()
{
	//YOU ARE A CLIENT

	// temporary buffer to use for the IP address
	const unsigned int bufferSz = 32;
	char ipAddress[bufferSz];

	//Get the Networking Instance
	NetworkingInstance& gpInstance = NetworkingInstance::getInstance();

	//Grabs a free socket to use
	RakNet::SocketDescriptor selectedSocket;
	gpInstance.getPeer()->Startup(1, &selectedSocket, 1);

	//Copies in the IP address to connect too
	strcpy(ipAddress, gpInstance.getServerIP());

	//If no IP address was entered, assume its a local server
	if (ipAddress[0] == '\n' || ipAddress[0] == NULL)
	{
		strcpy(ipAddress, "127.0.0.1");
	}

	//Connect to server
	printf("Joining the game.\n");
	printf("IP Address is: %s\n", ipAddress);
	gpInstance.getPeer()->Connect(ipAddress, gpInstance.getServerPort(), 0, 0);
}


//Handlers
void clientHandleNetworking()
{
	//Runs to read for IDs related to being a client in a game
	clientRead();
	//Runs to write client messages to send to the host relating to a game
	clientWrite();
}


//Readers
void clientRead()
{
	NetworkingInstance& gpInstance = NetworkingInstance::getInstance();

	RakNet::Packet *packet;

	for (
		packet = gpInstance.getPeer()->Receive();
		packet;
		gpInstance.getPeer()->DeallocatePacket(packet), packet = gpInstance.getPeer()->Receive()
		)
	{
		switch (packet->data[0])
		{
		case ID_NO_FREE_INCOMING_CONNECTIONS:
		{
			printf("The server is full.\n");
		}
		break; //End of ID_NO_FREE_INCOMING_CONNECTIONS

		case ID_DISCONNECTION_NOTIFICATION:
		{
			printf("Client Disconnected.\n");
		}
		break; //End of ID_DISCONNECTION_NOTIFICATION

		case ID_CONNECTION_LOST:
		{
			printf("Client lost connection lost.\n");
		}
		break; //End of ID_CONNECTION_LOST

		case ID_CONNECTION_REQUEST_ACCEPTED:
		{
			printf("Client: Connection has been accepted by the server.\n");
			//Gathers user data to send to the server for reference.
			UserData* data = new UserData();
			gpInstance.getUserName(data->userName); //char[] userName		

			//Set the NetworkMessageID
			data->networkMessageID = ID_SEND_USER_DATA;

			//Set the host id equal to the messages
			gpInstance.setHostID(packet->guid);

			//Place the info back into the sender to be sent by the Write() ASAP
			gpInstance.addToSender(data);
		}
		break; //End of ID_CONNECTION_REQUEST_ACCEPTED

		case ID_MESSAGE_DATA:
		{
			//Create incoming BitStream to read
			RakNet::BitStream incomingStream(packet->data, packet->length, false);

			//Ignore the MessageID
			incomingStream.IgnoreBytes(sizeof(RakNet::MessageID));

			printf("ChatMessage Received.\n");

			//Reads the bitstream in as the correct type of NetworkMessage
			ChatMessage* message = new ChatMessage();
			incomingStream.Read(*message); //Must be *message to pass by reference

			//Add the networkMessageID back in case it was lost in transit
			message->networkMessageID = ID_MESSAGE_DATA;

			printf("UserName: %s\n", message->userName);
			printf("Message: %s\n", message->message);

			//Push it back onto the MessageReceiver queue to be handled later in Unity
			gpInstance.addToReceiver(message);
		}
		break; //End of ID_MESSAGE_DATA

		case ID_PLAYER_NUM:
		{
			//Create incoming BitStream to read
			RakNet::BitStream incomingStream(packet->data, packet->length, false);

			//Ignore the MessageID
			incomingStream.IgnoreBytes(sizeof(RakNet::MessageID));

			printf("PlayerNum Received.\n");

			//Reads the bitstream in as the correct type of NetworkMessage
			PlayerNum* message = new PlayerNum();
			incomingStream.Read(*message); //Must be *message to pass by reference

			//Add the networkMessageID back in case it was lost in transit
			message->networkMessageID = ID_PLAYER_NUM;

			printf("PlayerNum = %d", message->playerNum);

			//Push it back on the messageReceiver queue to be handled later in Unity
			gpInstance.addToReceiver(message);
		}
		break; //End of ID_PLAYER_NUM

		case ID_START_GAME:
		{
			//Create incoming BitStream to read
			RakNet::BitStream incomingStream(packet->data, packet->length, false);

			//Ignore the MessageID
			incomingStream.IgnoreBytes(sizeof(RakNet::MessageID));

			printf("Start Game Received.\n");

			//Reads the bitstream in as the correct type of NetworkMessage
			StartGame* message = new StartGame();
			incomingStream.Read(*message); //Must be *message to pass by reference

			//Add the networkMessageID back in case it was lost in transit
			message->networkMessageID = ID_START_GAME;

			//Push it back on the messageReceiver queue to be handled later in Unity
			gpInstance.addToReceiver(message);
		}
		break; //End of ID_START_GAME

		case ID_DRAW_CARD:
		{
			//Create incoming BitStream to read
			RakNet::BitStream incomingStream(packet->data, packet->length, false);

			//Ignore the MessageID
			incomingStream.IgnoreBytes(sizeof(RakNet::MessageID));

			printf("Draw Card Received.\n");

			//Reads the bitstream in as the correct type of NetworkMessage
			DrawCard* message = new DrawCard();
			incomingStream.Read(*message); //Must be *message to pass by reference

			//Add the networkMessageID back in case it was lost in transit
			message->networkMessageID = ID_DRAW_CARD;

			//Push it back on the messageReceiver queue to be handled later in Unity
			gpInstance.addToReceiver(message);
		}
		break; //End of ID_DRAW_CARD

		case ID_BATTLE_PHASE:
		{
			//Create incoming BitStream to read
			RakNet::BitStream incomingStream(packet->data, packet->length, false);

			//Ignore the MessageID
			incomingStream.IgnoreBytes(sizeof(RakNet::MessageID));

			printf("BattlePhase Received.\n");

			//Reads the bitstream in as the correct type of NetworkMessage
			BattlePhase* message = new BattlePhase();
			incomingStream.Read(*message); //Must be *message to pass by reference

			//Add the networkMessageID back in case it was lost in Transit
			message->networkMessageID = ID_BATTLE_PHASE;

			//Push it back on the messageReceiver queue to be handled later in Unity
			gpInstance.addToReceiver(message);
		}
		break; //End of ID_BATTLE_PHASE

		case ID_CHAMPION_DATA:
		{
			//Create incoming BitStream to read
			RakNet::BitStream incomingStream(packet->data, packet->length, false);

			//Ignore the MessageID
			incomingStream.IgnoreBytes(sizeof(RakNet::MessageID));

			printf("ChampionData Received.\n");

			//Reads the bitsream in as the correct type of NetworkMessage
			ChampionData* message = new ChampionData();
			incomingStream.Read(*message); //Must be *message to pass by reference

			//Adds the networkMessageID back in case it was lost in Transit
			message->networkMessageID = ID_CHAMPION_DATA;

			printf("Card Name: %s \n", message->monsterName);
			printf("Monster health: %d\n", message->monsterHealth);
			printf("Monster Speed: %d \n", message->monsterSpeed);
			printf("Monster Element: %d \n", message->element);
			printf("Monster Alloy: %d \n", message->alloy);
			printf("Player Number: %d \n", message->playerNum);

			//Push it back on the messageReceiver queue to be handled later int Unity
			gpInstance.addToReceiver(message);
		}
		break; //End of ID_CHAMPION_DATA

		case ID_ATTACK:
		{
			//Create incoming BitStream to read
			RakNet::BitStream incomingStream(packet->data, packet->length, false);

			//Ignore the MessageID
			incomingStream.IgnoreBytes(sizeof(RakNet::MessageID));

			printf("AttackMessage Received.\n");

			//Reads the bitsream in as the correct type of NetworkMessage
			AttackMessage* message = new AttackMessage();
			incomingStream.Read(*message); //Must be *message to pass by reference

			//Adds the networkMessageID back in case it was lost in Transit
			message->networkMessageID = ID_ATTACK;

			//Push it back on the messageReceiver queue to be handled later int Unity
			gpInstance.addToReceiver(message);
		}
		break; //End of ID_ATTACK

		case ID_END_GAME:
		{
			//Create incoming BitStream to read
			RakNet::BitStream incomingStream(packet->data, packet->length, false);

			//Ignore the MessageID
			incomingStream.IgnoreBytes(sizeof(RakNet::MessageID));

			printf("End Game Received.\n");

			//Reads the bitstream in as the correct type of NetworkMessage
			EndGame* message = new EndGame();
			incomingStream.Read(*message); //Must be *message to pass by reference

			//Add the networkMessageID back in case it was lost in Transit
			message->networkMessageID = ID_END_GAME;

			//Push it back on the messageReceiver queue to be handled later in Unity
			gpInstance.addToReceiver(message);
		}
		break; //End of ID_END_GAME

		default:
			printf("Message with identifier %i has arrived.\n", packet->data[0]);
			break;
		}
	}
}


//Writers
void clientWrite()
{
	NetworkingInstance& gpInstance = NetworkingInstance::getInstance();

	while (!gpInstance.getSenderQueue().empty())
	{
		RakNet::BitStream outgoingStream;

		//Get a pointer to the front of the queue
		ChampNetMessage* messageToSend = gpInstance.getSenderQueue().front();

		//Pop the front of the queue off
		gpInstance.getSenderQueue().pop();

		//Write the ID to the bitstream
		outgoingStream.Write(messageToSend->networkMessageID);

		//Cast NetworkMessage before you send it
		switch (messageToSend->networkMessageID)
		{
		case ID_SEND_USER_DATA:
		{
			printf("Sending User Data!\n");
			UserData* finalMessageToSend = static_cast<UserData*>(messageToSend);
			outgoingStream.Write(*finalMessageToSend);
		}
		break;
		case ID_MESSAGE_DATA:
		{
			printf("Sending ChatMessage!\n");
			ChatMessage* finalMessageToSend = static_cast<ChatMessage*>(messageToSend);
			outgoingStream.Write(*finalMessageToSend);
		}
		break;
		case ID_PLAYER_NUM:
		{
			printf("Sending missingChampionRequest!\n");
			PlayerNum* finalMessageToSend = static_cast<PlayerNum*>(messageToSend);
			outgoingStream.Write(*finalMessageToSend);
		}
		break;
		case ID_CHAMPION_DATA:
		{
			printf("Sending ChampionData!\n");
			ChampionData* finalMessageToSend = static_cast<ChampionData*>(messageToSend);
			outgoingStream.Write(*finalMessageToSend);
		}
		break;
		case ID_END_GAME:
		{
			printf("Sending EndGame Message!\n");
			EndGame* finalMessageToSend = static_cast<EndGame*>(messageToSend);
			outgoingStream.Write(*finalMessageToSend);
		}
		break;

		default:
			printf("Error!");
		}

		//Send the message
		gpInstance.getPeer()->Send(&outgoingStream, HIGH_PRIORITY, RELIABLE_ORDERED, 0, gpInstance.getHostID(), false);

		delete messageToSend;
	}
}