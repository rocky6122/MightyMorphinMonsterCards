/////////////////////////////////////////////
// ChampNet| Written by John Imgrund (c) 2019
/////////////////////////////////////////////

/////////////////////
//
//Main game loop here
//
/////////////////////

#include "../source/egp-net-plugin-Unity/NetworkingInstance.h"
#include "mainServerLoop.h"
#include <chrono>

NetworkingInstance& gpInstance = NetworkingInstance::getInstance();

int main()
{
	float elapsedTime = 0;
	std::chrono::duration<double> elapsed;
	std::chrono::steady_clock::time_point start;
	std::chrono::steady_clock::time_point finish;

	//Intialize Networking Loop
	gpInstance.initializeHostInstance();

	//Initialize mainServerLoop
	serverMain* server = new serverMain();

	//Gameplay Loop
	while (true)
	{
		//start timer
		start = std::chrono::high_resolution_clock::now();
		
		//Gameplay loop for server
		server->serverLoop((float)elapsedTime);


		//Extract incoming info from networkingInstance

		//handle incoming effects

		//RTB system simulation

		//Any possible changes
		
		//Load new info into the networkingInstance


		//Sleep for 16 milliseconds
		Sleep(16);

		//end timer
		finish = std::chrono::high_resolution_clock::now();

		//set elapsed time of last frame
		elapsed = finish - start;

		elapsedTime = (float)elapsed.count();

		//printf("%f\n", elapsed.count());
	}


	delete server;
	//End the Networking Loop
	gpInstance.cleanUpInstance();

	return 0;
}