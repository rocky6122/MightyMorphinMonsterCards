/////////////////////////////////////////////
// ChampNet| Written by John Imgrund (c) 2019
/////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// Class Description:
//The objective of the NetworkingInstance is to be the main interfacing object
//between Unity and the Plugin itself. The NetworkingInstance is a singleton 
//that once instantiated creates a new thread for the networking to run on in
//the background, and sets up a local RakNet peer to handle all networking.
//
//
//Public Member Functions:
//	void initializeHostInstance(char* userName)
//	static NetworkingInstance& getInstance()
//	void addToSender(ChampNetMessage* messageToSend)
//	ChampNetMessage* grabFromSender()
//	void addToReceiver(ChampNetMessage* messageToReceive)
//	ChampNetMessageMessage* grabFromReceiver()
//	queue<ChampNetMessage*>& getSenderQueue()
//	queue<ChampNetMessage*>& getReceiverQueue()
//	void cleanUpInstance()
//
// (GETTERS)
//	RakPeerInterface* getPeer() const
//	UserData* getClientData()
//	char* getServerIP()
//	int isThreadingAllowed() const
//	const unisigned short getServerPort() const
//	const unsigned int getMaxClients() const
//	int getCurrentClientNum() const
//	void getUserName(char* userName) const
//	void addToClientNum(int num)
//
//
//Private Member Functions:
//	static int NetworkingHostThread()
//	void cleanMembers()
//
//
//Member Variables:
//	RakPeerInterface *mPeer
//	RakNetGUID mHostID
//	int mCurrentClientNum
//	const unsigned int M_MAX_CLIENTS
//	UserData* mClientData
//	char mServerIP[bufferSize]
//	const unsigned short M_SERVER_PORT
//	char mUserName[bufferSize]
//	queue<ChampNetMessage*>mMessagesToSend
//	queue<ChampNetMessage*>mMessagesReceived
//	int mThreadAllowed
//	egpThread mNetworkThread[1]
//
///////////////////////////////////////////////////////////////////////////////

#ifndef _NETWORKING_INSTANCE_H_
#define _NETWORKING_INSTANCE_H_

#include "RakNet/RakPeerInterface.h"
#include "RakNet/MessageIdentifiers.h"
#include "RakNet/RakNetTypes.h"
#include "RakNet/BitStream.h"
#include "RakNet/GetTime.h"

//Generic includes
#include <queue>

//Plugin includes
#include "NetworkingStructs.h"
#include "egpThread.h"
#include "egpTimer.h"


const unsigned int bufferSize = 31;


class NetworkingInstance
{
public:
	//Functions used in main
	
	///<summary>Intializes the instance of the pseudo singleton if/only if it doesn't already exist. Also initializes various member variables for hosting needs and starts multithreading.</summary>
	void initializeHostInstance();
	
	///<summary>Returns a pointer to the instance.</summary>
	static NetworkingInstance& getInstance()
	{
		//This version of instantiating a singleton is 'thread safe' per C++11

		static NetworkingInstance mInstance; //Garaunteed to be destroyed and instantiated on first use.
		return mInstance;
	}

	//Queue functions

	///<summary>Pushes the message back onto a queue of NetworkMessages to be handled by the network handling thread.</summary>
	void addToSender(ChampNetMessage* messageToSend);

	///<summary>Pops a message off the sender queue to be sent acrros the network.</summary>
	ChampNetMessage* grabFromsender();

	///<summary>Pushes the message banck onto the queue of NetworkMessages to be handled by the actual game.</summary>
	void addToReceiver(ChampNetMessage* messageToReceive);

	///<summary>Pops a message from the receiver queue of NetworkMessages to be handled in Unity.</summary>
	ChampNetMessage* grabFromReciever();

	///<summary>Returns the sender queue to perform various queue functions with it.</summary>
	std::queue<ChampNetMessage*>& getSenderQueue() { return mMessagesToSend; };

	///<summary>Returns the receiver queue to perform various queue functions with it.</summary>
	std::queue<ChampNetMessage*>& getReceiverQueue() { return mMessagesReceived; };

	///<summary>Deletes the instance if/only if it currently exists. Also deletes various memembers and stops the multithreading.</summary>
	void cleanUpInstance();


	//Get Functions
	///<summary>Returns a pointer to the RakPeerInterface.</summary>
	RakNet::RakPeerInterface* getPeer() const { return mPeer; };

	///<summary>Returns an array of ClientData. ClientData includes: a UserName.</summary>
	UserData* getClientData()& { return mClientData; };

	///<summary>Returns the serverIP.</summary>
	char* getServerIP() { return mServerIP; };

	///<summary>Returns whether or not Threading is allowed.</summary>
	int isThreadingAllowed() const { return mThreadAllowed; };

	///<summary>Returns the serverPort to run the game out of.</summary>
	const unsigned short getServerPort() const { return M_SERVER_PORT; };

	///<summary>Returns the max number of clients for the server.</summary>
	const unsigned int getMaxClients() const { return M_MAX_CLIENTS; };

	///<summary>Returns the current number of clients connected to the server.</summary>
	int getCurrentClientNum() const { return mCurrentClientNum; };

	///<summary>Adds 'parameter' to the currenClientNum.</summary>
	void addToClientNum(int num) { mCurrentClientNum += num; };

	///<summary>Returns current gameMode</summary>
	int getGameMode() const { return mGameMode;  };

	///<summary>Returns playerOnes champion speed</summary>
	float* const getPlayerOneSpeed() { return &mPlayerOneSpeed; };

	///<summary>Returns playerTwos champion speed</summary>
	float* const getPlayerTwoSpeed() { return &mPlayerTwoSpeed; };

	///<summary>Returns whether or not the playerone timer should reset</summary>
	bool const getPlayerOneAttackReset() { return mResetPlayerOneAttackTimer; };

	///<summary>Returns whether or not the playertwo timer should reset</summary>
	bool const getPlayerTwoAttackReset() { return mResetPlayerTwoAttackTimer; };

	///<summary>Returns championData for that player.</summary>
	void getPlayerChampion(int playerNum, ChampionData* outChampion) { 
		strcpy(outChampion->monsterName, mChampions[playerNum - 1].monsterName);
		outChampion->monsterHealth = mChampions[playerNum - 1].monsterHealth;
		outChampion->monsterSpeed = mChampions[playerNum - 1].monsterSpeed;
		outChampion->element = mChampions[playerNum - 1].element;
		outChampion->alloy = mChampions[playerNum - 1].alloy;
		outChampion->playerNum = mChampions[playerNum - 1].playerNum;
	};

	//Set Functions
	///<summary>Sets GameMode. Waiting for players = 0; Morphing Phase = 1; BattlePhase = 2; EndState = 3;</summary>
	void setGameMode(int mode) { mGameMode = mode; };

	///<summary>Sets player 1s champions speed.</summary>
	void setPlayerOneSpeed(float speed) { mPlayerOneSpeed = speed; mResetPlayerOneAttackTimer = true; };

	///<summary>Sets player 2s champion speed.</summary>
	void setPlayerTwoSpeed(float speed) { mPlayerTwoSpeed = speed; mResetPlayerTwoAttackTimer = true; };

	///<summary>Set playerone reset bool</summary>
	void setPlayerOneReset(bool reset) { mResetPlayerOneAttackTimer = reset; };

	///<summary>Set playertwo reset bool</summary>
	void setPlayerTwoReset(bool reset) { mResetPlayerTwoAttackTimer = reset; };

	///<summary>Sets the champion for that player number.</summary>
	void setPlayerChampion(int playerNum, ChampionData champion) { 
		strcpy(mChampions[playerNum - 1].monsterName, champion.monsterName);
		mChampions[playerNum - 1].monsterHealth = champion.monsterHealth;
		mChampions[playerNum - 1].monsterSpeed = champion.monsterSpeed;
		mChampions[playerNum - 1].element = champion.element;
		mChampions[playerNum - 1].alloy = champion.alloy;
		mChampions[playerNum - 1].playerNum = champion.playerNum;
		};

private:
	//Private constructors
	NetworkingInstance() {}
	//~NetworkingInstance() {}


	//Private Functions
	///<summary>Function to launch the host thread.</summary>
	static int networkingHostThread();

	///<summary>Resets the values of all the member variables.</summary>
	void cleanMembers();


	//Member Variables
	
	//RakNet members
	RakNet::RakPeerInterface *mPeer;
	RakNet::RakNetGUID mHostID;

	//Host info members
	int mCurrentClientNum = 0;
	const unsigned int M_MAX_CLIENTS = 2; //Game has a max of 2 players
	UserData* mClientData;

	float mPlayerOneSpeed = 100.0f; //Very high incase it fails, the server wont spam messages every frame
	float mPlayerTwoSpeed = 100.0f;

	bool mResetPlayerOneAttackTimer = false;
	bool mResetPlayerTwoAttackTimer = false;

	//Current Champions
	ChampionData mChampions[2]; //Max Clients


	//Client info members
	char mServerIP[bufferSize];
	
	//Generic info members
	const unsigned short M_SERVER_PORT = 25656;
	std::queue<ChampNetMessage*>mMessagesToSend;
	std::queue<ChampNetMessage*>mMessagesReceived;
	int mGameMode;

	// networking management with thread
	int mThreadAllowed;
	egpThread mNetworkThread[1];
	

public:
	// Forces the compiler not to allow for copies of the instance to be created
	NetworkingInstance(NetworkingInstance const& copy) = delete;            // Not Implemented
	NetworkingInstance& operator=(NetworkingInstance const& copy) = delete; // Not Implemented
};
#endif //!_NETWORKING_INSTANCE_H_