/////////////////////////////////////////////
// ChampNet| Written by John Imgrund (c) 2019
/////////////////////////////////////////////

#ifndef _MAIN_SERVER_LOOP_H_
#define _MAIN_SERVER_LOOP_H_

#include "../source/egp-net-plugin-Unity/NetworkingInstance.h"

class serverMain
{
public:
	//Constructors
	serverMain();
	~serverMain();

	void serverLoop(float elapsed);

	void morphingPhaseLoop(NetworkingInstance& instance);

	void battlePhaseLoop(NetworkingInstance& instance);

	//GETTERS
	float getDrawTime() { return CARD_DRAW_TIME; };

	float getBaseSpeed() { return BASE_SPEED; };

private:
	static float DamageMultiplierArray[6][6];

	//Const variables

	//Morphing Phase
	float const CARD_DRAW_TIME = 6;
	float const CARD_DRAW_LIMIT = 14;

	//Battle Phase
	float const BASE_SPEED = 4;
	
	//End of Const variables

	//Morphing Phase variables
	float mCardsDrawn;
	float mDrawTime;

	//BattlePhase
	float mPlayerOneAttackTime;
	float mPlayerTwoAttackTime;

	float* mPlayerOneSpeed;
	float* mPlayerTwoSpeed;
};


#endif