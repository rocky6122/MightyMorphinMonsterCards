/////////////////////////////////////////////
// ChampNet| Written by John Imgrund (c) 2019
/////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// Header Description:
//The objective of the networkManager header file is to contain the actual networking loop as
//well as any other functions that help deal directly with Networking. It is 
//decoupled from our NetworkingInstance singleton for clarity, and interacts with it
//via the singletons getInstance() return.
//
//
//Functions:
//	hostGame()
//	hostHandleNetworking()
//	hostRead()
//	hostWrite()
//
///////////////////////////////////////////////////////////////////////////////

#pragma once


#include "RakNet/RakPeerInterface.h"
#include "RakNet/MessageIdentifiers.h"
#include "RakNet/RakNetTypes.h"
#include "RakNet/BitStream.h"
#include "RakNet/GetTime.h"

//List of gameMessages to tag our games custom messages
enum GameMessages
{
	ID_CUSTOM_MESSAGE_START = ID_USER_PACKET_ENUM,

	//Custom message types that can be sent
	ID_SEND_USER_DATA,
	ID_MESSAGE_DATA,
	ID_PLAYER_NUM,
	ID_START_GAME,
	ID_DRAW_CARD,
	ID_BATTLE_PHASE,
	ID_CHAMPION_DATA,
	ID_ATTACK,
	ID_END_GAME,
};

//Starters
///<summary>Sets up a server based on host instances parameters.</summary>
void hostGame();


//Handlers
///<summary>The main networking loop for hosts.</summary>
void hostHandleNetworking();


//Readers/Writers
///<summary>Handles all incoming packets aimed at the host.</summary>
void hostRead();
///<summary>Handles all the outgoing packets from the host.</summary>
void hostWrite();