////////////////////////////////////////////
// ChampNet| Written by John Imgrund(c) 2019
////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// Header Description:
//The objective of the NetworkingStructs header file is to store all structs to
//be sent via the networking System. This keeps them decoupled from the 
//networkManager and the NetworkingInstance for clarity, as well as usability.
//
//This file will change based on the needs of the game being used by the plugin
//as the structs sent are almost always specific to the game at hand.
//
//All structs inherit from generic struct 'ChampNetMessage', which contains 
//a unsigned char networkMessageID. This ID, denotes what kind of message the
//struct is and allows for it to be static_cast into its propper form when 
//pulled from the polymorphic queues of ChampNetMessages.
//
//Assuming all ChampNetMessage children will eventually be sent over the network
//all of the structs are pragma packed to eliminate excess data.
//
//
//Structs:
//	ChampNetMessage: unsigned char networkMessageID
//	UserData: char userName[31]
//	ChatMessage: char userName[31], char message[512]
//
///////////////////////////////////////////////////////////////////////////////


#ifndef _NETWORKING_STRUCTS_H_
#define _NETWORKING_STRUCTS_H_

//Messages to send over the network via a NetworkMessageContainer
#pragma pack(push, 1)

//Generic Struct that messages derive from to be stored in queue. DO NOT ACTUALLY USE THIS DIRECTLY
///<summary>A generic struct with a unsigned char networkMessageID to be inherited by other structs to be sent via the network. This allows them to all be stored in a queue of structs.</summary>
typedef struct ChampNetMessage
{
	unsigned char networkMessageID;
} NetworkMessage;


//ChampNetMessage inherited structs

///<summary>UserData is used to send a clients data to the Host for later reference. Each UserData includes: char userName[31].</summary>
struct UserData : ChampNetMessage
{
	char userName[31];
};

///<summary>ChatMessage is used to send messages between clients. Each ChatMessage includes: char userName[31], char message[512] .</summary>
struct ChatMessage : ChampNetMessage
{
	char userName[12];
	char message[40];
};

///<summary>Tells the players when to draw a new card. Contains nothing.</summary>
struct DrawCard : ChampNetMessage
{};

///<summary>Notifies players to start the game.</summary>
struct StartGame : ChampNetMessage
{};

///<summary>Tells player which player they are for when sending data relating to their cards</summary>
struct PlayerNum : ChampNetMessage
{
	int playerNum;
};

///<summary>Notifies players to switch the the battlephase.</summary>
struct BattlePhase : ChampNetMessage
{};

///<summary>Contains data relating to monster cards for both the player and server.</summary>
struct ChampionData : ChampNetMessage
{
	char monsterName[31];
	int monsterHealth;
	int monsterSpeed;
	int element;
	int alloy;
	int playerNum;
};

///<summary>Contains the playerNum of which players monster is primed to attack.</summary>
struct AttackMessage : ChampNetMessage
{
	int playerNum;
};

///<summary>Notifies players to switch to the end phase</summary>
struct EndGame : ChampNetMessage
{
	char winnerName[31];
};

#pragma pack (pop)


#endif 