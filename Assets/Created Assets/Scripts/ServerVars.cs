using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

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
	bool game_complete = false;
	bool killer_won = false;

	public Text chat_box_text;
	public InputField input_box_text;
	public List<string> char_text_to_add;
	public string view_string = "";
	private bool update_chat = false;

	private string username = "";
	
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
				// if the round is complete
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
						}
						round_complete = false;
					}
					else{
						game_complete = true;
						killer_won = false;
					}
					int living_count = 0; // represents those who are alive and NOT the killer
					for(int i = 0; i<players.Length; i++){
						if(players[i].currentID != killer && players[i].living == true){
							living_count++;
						}
					}
					if(living_count == 1){
						game_complete = true;
						killer_won = true;
					}
				}
				// move to the next round
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

			if(char_text_to_add.Count > 0){
				if(view_string == ""){
					view_string = char_text_to_add[0];
				}
				else{
					view_string += char_text_to_add[0];
				}
				char_text_to_add.RemoveAt(0);
			}
			if(update_chat){

				char_text_to_add.Add ("\n" + username + ": " + input_box_text.text );
				update_chat = false;
				input_box_text.text = "";
			}
			networkView.RPC("updateViewString", RPCMode.Server, view_string);
		}
		else{
			start_game.alpha = 0;
			start_game.blocksRaycasts = false;
			if(update_chat){
				networkView.RPC("sendTextToServer", RPCMode.Server, username + ": "+ input_box_text.text);
				update_chat = false;
				input_box_text.text = "";
			}
		}

		FindObjectOfType<GameController> ().resizePlayers ();
		if (view_string != "") {
			chat_box_text.text = view_string;
		}
		username = "Player" + FindObjectOfType<GameController> ().playerID;
	}

	[RPC]
	public void updateViewString(string new_string){
		view_string = new_string;
	}

	[RPC]
	public void sendTextToServer(string new_string){
		char_text_to_add.Add ("\n" + new_string);
	}

	public void updateChatBox(){
		if(input_box_text.text != ""){
			update_chat = true;
		}
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
