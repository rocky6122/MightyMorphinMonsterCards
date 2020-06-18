/*
EGP Networking: Plugin Interface/Wrapper
Dan Buckstein
October 2018

Main interface for plugin. Defines symbols to be exposed to whatever
application consumes the plugin. Targeted for Unity but should also
be accessible by custom executables (e.g. test app).
*/

////////////////////////////////////////////
// Heavily Modified by John Imgrund (c) 2019
////////////////////////////////////////////


#ifndef _EGP_NET_PLUGIN_H_
#define _EGP_NET_PLUGIN_H_

#include "egp-net-plugin-config.h"
#include "NetworkingStructs.h"

#ifdef __cplusplus
extern "C"
{
#endif //__cplusplus
	//START OF C COMPILER LAND
	
	//Init + Destroy
	DLL_SYMBOL
		void InitializeClientNetworking(char* userName, char* serverIP);

	DLL_SYMBOL
		void StopNetworking();

	//Polling
	DLL_SYMBOL
		int ReceiveMessageType();

	//Specific Message Functions

	//Chat Message Functions

	///<summary>Takes in the values needed for a ChatMessage and constructs it plugin side.</summary>
	DLL_SYMBOL
	void AddChatMessageToSender(char * message);

	///<summary>Receives the char* userName from a ChatMessage on front of the queue, use ReceiveMessageType() to check.</summary>
	DLL_SYMBOL
	char * ChatMessageUserName();

	///<summary>Receives the char* message from a ChatMessage on front of the queue, use ReceiveMessageType() to check.</summary>
	DLL_SYMBOL
	char * ChatMessageMessage();

	//PlayerNum Functions
	
	///<summary>Receives the int playerNum for this player.</summary>
	DLL_SYMBOL
	int PlayerNumMessage();

	///<summary>Asks server for missing Champion information.</summary>
	DLL_SYMBOL
		void GetMissingChampion(int playerNum);

	//Start Game Functions Not needed

	//Draw Card Functions Not needed

	//BattlePhase Functions Not Needed

	//ChampionData Functions
	DLL_SYMBOL
	void AddChampionDataToSender(char* monsterName, int monsterHealth, int monsterSpeed, int element, int alloy, int playerNum);

	DLL_SYMBOL
	char* ChampionDataName();

	DLL_SYMBOL
	int ChampionHealth();

	DLL_SYMBOL
	int ChampionElement();

	DLL_SYMBOL
	int ChampionAlloy();

	DLL_SYMBOL
	int ChampionsPlayerNum();

	//AttackMessage Functions
	DLL_SYMBOL
	int AttackingPlayer();

	//End Game functions

	///<summary>Senders a message to other users with players name indicating they won.</summary>
	DLL_SYMBOL 
	void AddEndGameMessageToSender();

	///<summary>Receives the winning players name.</summary>
	DLL_SYMBOL
	char * EndGameMessage();
	
	//END Specific Message Functions

	//Message Popper
	DLL_SYMBOL
		void PopReceiver();

	//Testing Purposes
	DLL_SYMBOL
		int foo(int bar);

	DLL_SYMBOL
		char* tester(char* test);


	//END OF C COMPILER LAND
#ifdef __cplusplus
}
#endif //__cplusplus

#endif	// !_EGP_NET_PLUGIN_H_