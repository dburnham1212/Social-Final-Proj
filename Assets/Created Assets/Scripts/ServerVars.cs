using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// The game controller will classify and decide which will be a server
// when a class is a server this will hold all of the functionality to update clients
public class ServerVars : MonoBehaviour {

	public float current_size;
	public CanvasGroup start_game;
	public bool game_started = false;

	// games timed values stored on server for rounds
	public static float round_time = 10;
	public static float game_timer;

	// games current killer
	public bool killer_selected = false;
	public int killer = 0;
	private bool round_complete = false;


	
	// Simple UI Functions to be used on buttons
	public void startGame(){
		game_started = true;

	}

	void Start () {
		game_timer = round_time;

	}

	// Update is called once per frame
	void Update () {	
		if(Network.isServer){
			float new_size = FindObjectOfType<GameController> ().GetComponent<Canvas> ().scaleFactor;
			networkView.RPC("updateSize", RPCMode.All, new_size);
			networkView.RPC("updateGameStart", RPCMode.All, game_started);
			if(game_started){
				// when the game begins select a killer
				if(!killer_selected){
					CharacterController[] players = FindObjectsOfType<CharacterController>();
					killer = Random.Range(0, players.Length);
					killer_selected = true;
					networkView.RPC("updateKiller", RPCMode.All, killer);
				}
				// next increment through the rounds 
				else if(round_complete == true){
					CharacterController[] players = FindObjectsOfType<CharacterController>();
					bool k_found = true;
					// check if the other players have found the killer
					for(int i = 0; i<players.Length; i++){
						if(players[i].guess_target != killer && players[i].currentID != killer){
							k_found = false;
						}
					}

					if(k_found == false){
						for(int i = 0; i<players.Length; i++){
							if(players[i].currentID == killer && k_found == false){
								for(int j = 0; j < players.Length; j++){
									if(players[j].currentID == players[i].guess_target){
										players[j].living = false;
									}
								}
							}
							else{
							
							}
						}
					}
					round_complete = false;
				}
				else{
					incrementRounds();
					networkView.RPC("updateTimer", RPCMode.All, game_timer);
				}
			}

			if(!game_started){
				start_game.alpha = 1;
				start_game.blocksRaycasts = true;
			}
			else{
				start_game.alpha = 0;
				start_game.blocksRaycasts = false;
			}

		}
		else{
			start_game.alpha = 0;
			start_game.blocksRaycasts = false;
		}
		FindObjectOfType<GameController> ().resizePlayers ();
	}

	void incrementRounds(){
		if (game_timer > 0) {
			game_timer -= Time.deltaTime;
		}
		else{
			game_timer = round_time;
			round_complete = true;
		}
	}

	[RPC]
	void updateTimer(float new_timer){
		game_timer = new_timer;
	}

	[RPC]
	void updateKiller(int killer_val){
		killer = killer_val;
	}

	[RPC]
	void updateGameStart(bool new_start){
		game_started = new_start;
	}


	[RPC]
	void updateSize(float new_size){
		current_size = new_size;

	}
}
