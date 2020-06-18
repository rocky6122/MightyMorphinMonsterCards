/////////////////////////////////////////////
// ChampNet| Written by John Imgrund (c) 2019
/////////////////////////////////////////////

#include "mainServerLoop.h"

serverMain::serverMain()
{
	//Get the Networking Instance
	NetworkingInstance& gpInstance = NetworkingInstance::getInstance();

	//Set Timers
	mDrawTime = 0;
	mPlayerOneAttackTime = 0;
	mPlayerTwoAttackTime = 0;

	mPlayerOneSpeed = gpInstance.getPlayerOneSpeed(); //Turn it into a percentage
	mPlayerTwoSpeed = gpInstance.getPlayerTwoSpeed(); //Turn it into a percentage
}

float serverMain::DamageMultiplierArray[6][6] =
{ //Normal, Bronze, Steel, Brass, Pewter, Electrum
	{1, 1, 1, 1, 1, 1},       //Normal
	{1, 1, 1.5, 1, 1, 0.5}, //Water
	{1, 1.5f, 0.5f, 1, 1, 1}, //Fire
	{1, 0.5f, 1, 1, 1.5f, 1}, //Earth
	{1, 1, 1, 1.5f, 0.5f, 1}, //Air
	{1, 1, 1, 0.5f, 1, 1.5f}  //Lightning
};

serverMain::~serverMain()
{
}

void serverMain::serverLoop(float elapsed)
{
	//Get the Networking Instance
	NetworkingInstance& gpInstance = NetworkingInstance::getInstance();


	//Handle updates from players

	if (gpInstance.getGameMode() == 0) //Waiting for players
	{
		//Nothing, just waiting for other people to join
	}
	else if (gpInstance.getGameMode() == 1) //morphing phase
	{
		//Update Timer
		mDrawTime += elapsed;

		morphingPhaseLoop(gpInstance);
	}
	else if (gpInstance.getGameMode() == 2) //battle phase
	{
		//Update Timers
		mPlayerOneAttackTime += elapsed;
		mPlayerTwoAttackTime += elapsed;

		battlePhaseLoop(gpInstance);
	}
	else if (gpInstance.getGameMode() == 3) //End State
	{
		//Probably nothin again
	}
}

void serverMain::morphingPhaseLoop(NetworkingInstance& instance)
{
	//Check timer to see if players should draw cards
	if (mDrawTime > CARD_DRAW_TIME)
	{
		//Check to see if enough cards have been drawn for the morph phase
		if (mCardsDrawn >= CARD_DRAW_LIMIT)
		{
			printf("Go to Battle Phase\n");

			//Create new BattlePhase Message
			BattlePhase* BattleMessage = new BattlePhase();

			BattleMessage->networkMessageID = 140; //BattlePhase ID

			//Add to Sender
			instance.addToSender(BattleMessage);

			//Go to next game mode
			instance.setGameMode(2);
		}
		else
		{
			printf("draw\n");
			//reset timer
			mDrawTime = 0;

			//Create new DrawCard Message
			DrawCard* draw = new DrawCard();

			draw->networkMessageID = 139; //Draw card ID

			//Add to sender
			instance.addToSender(draw);

			++mCardsDrawn;
		}
	}


}

void serverMain::battlePhaseLoop(NetworkingInstance& instance)
{
	if (instance.getPlayerOneAttackReset())
	{
		//Reset Attack Timer
		mPlayerOneAttackTime = 0;
		instance.setPlayerOneReset(false);
	}


	//Check First players Timer
	if ((*mPlayerOneSpeed * BASE_SPEED) < mPlayerOneAttackTime)
	{
		//Player One Attacks
		AttackMessage* attack = new AttackMessage();

		attack->networkMessageID = 142; //Attack Message ID
		attack->playerNum = 1;

		//Add to Sender
		instance.addToSender(attack);

		//Reset Timer
		mPlayerOneAttackTime = 0;
	}

	if (instance.getPlayerTwoAttackReset())
	{
		//Reset Attack Timer
		mPlayerTwoAttackTime = 0;
		instance.setPlayerTwoReset(false);
	}

	//Check Second Players Timer
	if ((*mPlayerTwoSpeed * BASE_SPEED) < mPlayerTwoAttackTime)
	{
		//Player Two Attacks
		AttackMessage* attack = new AttackMessage();

		attack->networkMessageID = 142; //Attack Message ID
		attack->playerNum = 2;

		//Add to Sender
		instance.addToSender(attack);

		//Reset Timer
		mPlayerTwoAttackTime = 0;
	}
}