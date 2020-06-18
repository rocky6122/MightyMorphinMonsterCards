/*
EGP Networking: Plugin Interface/Wrapper
Dan Buckstein
October 2018

Main implementation of Unity plugin wrapper.
*/

/////////////////////////////////////////////////////////////////
// Heavily Modified by John Imgrund, Parker Staszkiewicz (c) 2019
/////////////////////////////////////////////////////////////////

#include "egp-net-plugin.h"
#include "NetworkingInstance.h"

NetworkingInstance& gpInstance = NetworkingInstance::getInstance();

///<summary>Intializes the Networking Instance for the client and starts the thread it will run on.</summary>
void InitializeClientNetworking(char* userName, char* serverIP)
{
	//Credit: Dan Buckstein Animal3D Console Allocation
	HANDLE handle = GetConsoleWindow();
	if (!handle)
	{
		int status = AllocConsole();
		freopen("CONOUT$", "w", stdout);
	}

	//testing
	printf("UserName: %s\n", userName);
	printf("ServerIP: %s\n", serverIP);
	gpInstance.initializeClientInstance(userName, serverIP);
}

///<summary>Ends the Networking instance and closes the thread it ran on.</summary>
void StopNetworking()
{
	gpInstance.cleanUpInstance();
}

///<summary>Polls the Receiver queue to see if it has a message to be handled. If it does, it returns the messageType that needs to be handled, else -1.</summary>
int ReceiveMessageType()
{
	int messageType;

	//Makes sure the queue contained a value to avoid error checking a null queue front
	if (gpInstance.getReceiverQueue().size() > 0)
	{
		ChampNetMessage* messageToCheck = gpInstance.getReceiverQueue().front();

		//Sets messageType to correct messageID
		messageType = messageToCheck->networkMessageID;
	}
	else //No messages to be handled
	{
		messageType = -1;
	}

	return messageType;
}

///<summary>Takes in the values needed for a ChatMessage and constructs it plugin side.</summary>
void AddChatMessageToSender(char * message)
{
	//Create new ChatMessage
	ChatMessage* newMessage = new ChatMessage();

	//Assign Values
	gpInstance.getUserName(newMessage->userName);
	strcpy(newMessage->message, message);

	newMessage->networkMessageID = 136; //ID_MESSAGE_DATA

	//Pushes to sender to be handled later
	gpInstance.addToSender(newMessage);
}

///<summary>Receives the char* userName from a ChatMessage on front of the queue, use ReceiveMessageType() to check.</summary>
char * ChatMessageUserName()
{
	//Create a pointer to the message then static_cast it as a ChatMessage
	ChampNetMessage* messageToCheck = gpInstance.getReceiverQueue().front();
	ChatMessage* chat = static_cast<ChatMessage*>(messageToCheck);

	return chat->userName;
}

///<summary>Receives the char* message from a ChatMessage on front of the queue, use ReceiveMessageType() to check.</summary>
char * ChatMessageMessage()
{
	//Create a pointer to the message then static_cast it as a ChatMessage
	ChampNetMessage* messageToCheck = gpInstance.getReceiverQueue().front();
	ChatMessage* chat = static_cast<ChatMessage*>(messageToCheck);

	return chat->message;
}

///<summary>Receives the int playerNum for this player.</summary>
int PlayerNumMessage()
{
	//Create a pointer to the message then static_cast it as a PlayerNumMessage
	ChampNetMessage* messageToCheck = gpInstance.getReceiverQueue().front();
	PlayerNum* number = static_cast<PlayerNum*>(messageToCheck);

	return number->playerNum;
}

void GetMissingChampion(int playerNum)
{
	//Create new PlayerNumMessage
	PlayerNum* missingChampion = new PlayerNum();

	missingChampion->playerNum = playerNum;

	missingChampion->networkMessageID = 137; //ID_PLAYER_NUM

	//Pushes to sender to be handled later
	gpInstance.addToSender(missingChampion);
}

void AddChampionDataToSender(char * monsterName, int monsterHealth, int monsterSpeed, int element, int alloy, int playerNum)
{
	//Create new ChampionData
	ChampionData* champion = new ChampionData();

	//Assign Values
	strcpy(champion->monsterName, monsterName);
	champion->monsterHealth = monsterHealth;
	champion->monsterSpeed = monsterSpeed;
	champion->element = element;
	champion->alloy = alloy;
	champion->playerNum = playerNum;

	champion->networkMessageID = 141; //ID_CHAMPION_DATA

	//Pushes to sender to be handled later
	gpInstance.addToSender(champion);
}

char * ChampionDataName()
{
	//Create a pointer to the message then static_cast it as a ChampionData
	ChampNetMessage* messageToCheck = gpInstance.getReceiverQueue().front();
	ChampionData* champion = static_cast<ChampionData*>(messageToCheck);

	return champion->monsterName;
}

int ChampionHealth()
{
	//Create a pointer to the message then static_cast it as a ChampionData
	ChampNetMessage* messageToCheck = gpInstance.getReceiverQueue().front();
	ChampionData* champion = static_cast<ChampionData*>(messageToCheck);

	return champion->monsterHealth;
}

int ChampionElement()
{
	//Create a pointer to the message then static_cast it as a ChampionData
	ChampNetMessage* messageToCheck = gpInstance.getReceiverQueue().front();
	ChampionData* champion = static_cast<ChampionData*>(messageToCheck);

	return champion->element;
}

int ChampionAlloy()
{
	//Create a pointer to the message then static_cast it as a ChampionData
	ChampNetMessage* messageToCheck = gpInstance.getReceiverQueue().front();
	ChampionData* champion = static_cast<ChampionData*>(messageToCheck);

	return champion->alloy;
}

int ChampionsPlayerNum()
{
	//Create a pointer to the message then static_cast it as a ChampionData
	ChampNetMessage* messageToCheck = gpInstance.getReceiverQueue().front();
	ChampionData* champion = static_cast<ChampionData*>(messageToCheck);

	return champion->playerNum;
}

int AttackingPlayer()
{
	//Create a pointer to the message then static_cast it as an AttackMessage
	ChampNetMessage* messageToCheck = gpInstance.getReceiverQueue().front();
	AttackMessage* attack = static_cast<AttackMessage*>(messageToCheck);

	return attack->playerNum;
}

///<summary>Senders a message to other users with players name indicating they won.</summary>
void AddEndGameMessageToSender()
{
	//Create new EndGame Message
	EndGame* newMessage = new EndGame();

	//Assign Values
	gpInstance.getUserName(newMessage->winnerName);

	newMessage->networkMessageID = 143; //ID_END_GAME

	//Pushes to sender to be handled later
	gpInstance.addToSender(newMessage);
}

///<summary>Receives the winning players name.</summary>
char * EndGameMessage()
{
	//Create a pointer to the message then static_cast it as an EndGameMessage
	ChampNetMessage* messageToCheck = gpInstance.getReceiverQueue().front();
	EndGame* endGameMessage = static_cast<EndGame*>(messageToCheck);

	return endGameMessage->winnerName;
}

///<summary>Pops the front of the receiver queue and deletes it.</summary>
void PopReceiver()
{
	ChampNetMessage* messageToCheck = gpInstance.getReceiverQueue().front();

	gpInstance.getReceiverQueue().pop();

	delete messageToCheck;
}

// dummy function for testing
int foo(int bar)
{
	return (bar + 1);
}

//testing
char* tester(char* test)
{
	return test;
}