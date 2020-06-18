////////////////////////////////////////////////////////////////////////////////////
// ChampNet| Written by John Imgrund, Anthony Pascone, Parker Staszkiewicz (c) 2018
////////////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// Class Description:
//The objective of the NetworkingInstance is to be the main interfacing object
//between Unity and the Plugin itself. The NetworkingInstance is a singleton 
//that once instantiated creates a new thread for the networking to run on in
//the background, and sets up a local RakNet peer to handle all networking.
//
//
//Public Member Functions:
//	void initializeClientInstance(char* userName, char* serverIP))
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
//	char* getServerIP()
//	int isThreadingAllowed() const
//	const unisigned short getServerPort() const
//	void getUserName(char* userName) const
//
//
//Private Member Functions:
//	static int NetworkingClientThread()
//	void cleanMembers()
//
//
//Member Variables:
//	RakPeerInterface *mPeer
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
	//Functions used in Plugin Bridge

	///<sumary>Intializes the instance of the pseudo singleton if/only if it doesn't already exist. Also initializes various member variables for client needs and starts multithreading.</summary>
	void initializeClientInstance(char* userName,  char* serverIP);
	
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

	///<summary>Returns the serverIP.</summary>
	char* getServerIP() { return mServerIP; };

	///<summary>Returns whether or not Threading is allowed.</summary>
	int isThreadingAllowed() const { return mThreadAllowed; };

	///<summary>Returns the serverPort to run the game out of.</summary>
	const unsigned short getServerPort() const { return M_SERVER_PORT; };

	///<summary>Parameter: char* Return: Will set 'parameter' equal to the userName of the current networkingInstance.</summary>
	void getUserName(char* userNameGetter) const { strcpy(userNameGetter, mUserName); };

	///<summary>Returns the Hosts GUID</summary>
	RakNet::RakNetGUID getHostID() const { return mHostID;  };

	//Set Functions
	///<summary>Sets the hostGUID</summary>
	void setHostID(RakNet::RakNetGUID guid) { mHostID = guid; };

private:
	//Private constructors
	NetworkingInstance() {}
	//~NetworkingInstance() {}


	//Private Functions
	///<summary>Function to launch the client thread.</summary>
	static int networkingClientThread();

	///<summary>Resets the values of all the member variables.</summary>
	void cleanMembers();


	//Member Variables
	
	//RakNet members
	RakNet::RakPeerInterface *mPeer;
	RakNet::RakNetGUID mHostID;

	//Client info members
	char mServerIP[bufferSize];
	
	//Generic info members
	const unsigned short M_SERVER_PORT = 25656;
	char mUserName[bufferSize];
	std::queue<ChampNetMessage*>mMessagesToSend;
	std::queue<ChampNetMessage*>mMessagesReceived;

	// networking management with thread
	int mThreadAllowed;
	egpThread mNetworkThread[1];
	

public:
	// Forces the compiler not to allow for copies of the instance to be created
	NetworkingInstance(NetworkingInstance const& copy) = delete;            // Not Implemented
	NetworkingInstance& operator=(NetworkingInstance const& copy) = delete; // Not Implemented
};
#endif //!_NETWORKING_INSTANCE_H_