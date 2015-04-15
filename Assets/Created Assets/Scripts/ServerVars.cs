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
	public static float round_time = 30;
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

	public Sprite coin_sprite;
	public Sprite xp_sprite;

	public InGamePickup in_game_pick;

	public Clue[] clue_options;
	public int curr_clue;

	public List<Clue> found_clues;
	public CluePanel[] display_panels;

	// Simple UI Functions to be used on buttons
	public void startGame(){
		game_started = true;

	}

	void Start () {
		game_timer = round_time;

	}

	// Update is called once per frame
	void Update () {	
		for (int i = 0; i < found_clues.Count; i++) {
			if(i < display_panels.Length){
				display_panels[i].GetComponent<Image>().sprite = found_clues[i].clue_sprite;
				display_panels[i].strength = found_clues[i].strength;
				display_panels[i].intelligence = found_clues[i].intelligence;
			}
		}

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
					GameController g_conn = FindObjectOfType<GameController>();
					int clue_x = 0;
					int clue_y = 0;

					clue_x = Random.Range(0, 2);
					clue_y = Random.Range(0, 1);

					CharacterController killer_char = new CharacterController();
					for(int i = 0; i < players.Length; i++){
						if(players[i].currentID == killer){
							killer_char = players[i];
						}
					}

					bool clue_set = false;
					while(clue_set == false){
						int clue_val = Random.Range (0, clue_options.Length);
						if(killer_char.strength >= clue_options[clue_val].strength && killer_char.intelligence >= clue_options[clue_val].intelligence && !clue_set){
							clue_set = true;
							curr_clue = clue_val;
						}
					}

					networkView.RPC("updateCurrClue", RPCMode.All, curr_clue);

					for(int i = 0; i < g_conn.map_info.Length; i++){
						if(g_conn.map_info[i].door_x == clue_x && g_conn.map_info[i].door_y == clue_y)
						{
							g_conn.map_info[i].pick_up_type = MapPanelInfo.PickupType.CLUE;
						}
						else{
							int other_choice = Random.Range (0,5);
							if(other_choice == 0){
								g_conn.map_info[i].pick_up_type = MapPanelInfo.PickupType.COIN;
							}
							else{
								g_conn.map_info[i].pick_up_type = MapPanelInfo.PickupType.XP;
							}

						}
						g_conn.map_info[i].round_item_taken = false;
						g_conn.map_info[i].updateInClients();
					}

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
					round_complete = false;
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
	public void updateCurrClue(int new_clue){
		curr_clue = new_clue;
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
