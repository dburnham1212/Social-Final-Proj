using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	private int playerCount = 0;
	private HostData[] host_data;
	private HostData curr_host_data;
	private int max_players = 8;
	private bool connected = false;

	public Text connTPanel;
	public Text lobby_list;
	public Text time_panel;
	public Text inn_panel;
	public CanvasGroup host_selection;
	public bool connecting = true;

	public GameObject player_prefab;	// The player to spawn
	public GameObject map; 				// The Game Board

	public ServerVars server_vars;
	public bool display_map;
	public CanvasGroup player_map;
	public int playerID = 0;
	private GameObject local_player;

	public CanvasGroup target_selector;
	public bool display_target_selector = false;
	public bool select_target = true;
	public Text player_c_panel;

	public CanvasGroup clue_panel;
	public bool show_clue_panel = false;


	public int curr_kill_target = -1;
	public bool out_of_game = false;

	public MapPanelInfo[] map_info;
	public MapPanelInfo current_map_info;
	
	public int intelligence = 10;
	public int strength = 0;
	public Text inn_text;
	public Text str_text;

	public Slider inn_str_slider;

	public void showCluePanel(){
		if(server_vars.game_started){
			show_clue_panel = true;
		}
	}

	public void hideCluePanel(){
		show_clue_panel = false;
	}

	void Awake() {
		MasterServer.RequestHostList("gamea");
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		intelligence = (int)inn_str_slider.value;
		strength = (int)(inn_str_slider.maxValue - inn_str_slider.value);
		inn_text.text = "" + intelligence;
		str_text.text = "" + strength;
		if(!connected){
			host_data = MasterServer.PollHostList();

			lobby_list.text = "";
			for(int i = 0; i < host_data.Length; i++){
				lobby_list.text += host_data[i].gameName;
				Debug.Log("Game name: " + host_data[i].gameName);
				if(!Network.isServer){
					if(host_data[i].connectedPlayers < max_players){
						curr_host_data = host_data[i];
					}
				}
			}
			host_selection.alpha = 1;
			host_selection.blocksRaycasts = true;

			player_map.alpha = 0;
			player_map.blocksRaycasts = false;

			target_selector.alpha = 0;
			target_selector.blocksRaycasts = false;

			clue_panel.alpha = 0;
			clue_panel.blocksRaycasts = false;
		}
		else{
			host_selection.alpha = 0;
			host_selection.blocksRaycasts = false;

			if(server_vars.game_started){
				// control the games UI
				if(show_clue_panel == true){
					clue_panel.alpha = 1;
					clue_panel.blocksRaycasts = true;
				}
				else{
					clue_panel.alpha = 0;
					clue_panel.blocksRaycasts = false;
				}
				if(display_map){
					player_map.alpha = 1;
					player_map.blocksRaycasts = true;
				}
				else{
					player_map.alpha = 0;
					player_map.blocksRaycasts = false;
				}
				if(select_target || display_target_selector){
					target_selector.alpha = 1;
					target_selector.blocksRaycasts = true;
				}
				else{
					target_selector.alpha = 0;
					target_selector.blocksRaycasts = false;
				}
				if(server_vars.killer == playerID){
					inn_panel.text = "Killer";
				}
				else{
					inn_panel.text = "Innocent";
				}

				updateLivingCounter();
			}
		}

		map.GetComponent<Image> ().sprite = current_map_info.GetComponent<Image> ().sprite;
		if (Network.isServer) {
			if(connected == false){
				spawnPlayer();
			}
			connTPanel.text = "Server";
			connected = true;


		}

		if(Network.isClient){
			if(connecting == true){
				connTPanel.text = "Client: Connecting";
			}
			else{
				if(connected == false){
					spawnPlayer();
				}
				connTPanel.text = "Client: Connected";
				connected = true;
			}
		}

		updateClientPlayers ();

	}

	public void displayMap(){
		if(connected){
			display_map = true;
		}
	}
	
	public void displayTargetSelector(){
		display_target_selector = true;
	}
	
	public void hideTargetSelector(){
		display_target_selector = false;
	}
	
	public void hideMap(){
		if(connected){
			display_map = false;
		}
	}

	public void updateLivingCounter(){
		int player_count = 0;
		CharacterController[] players = FindObjectsOfType<CharacterController> ();
		for(int i = 0; i < players.Length; i++){
			if(players[i].living){
				player_count ++;
			}
		}
		player_c_panel.text = "Alive: " + player_count + "/" + players.Length;
	}
	
	public void updateClientPlayers(){
		CharacterController[] players = FindObjectsOfType<CharacterController>();
		for (int i = 0; i < players.Length; i++) {
			if(	players[i].transform.parent != map.transform){
				players[i].transform.parent = map.transform;
			}
		}
		time_panel.text = "" + ServerVars.game_timer;
	}
	
	
	public void resizePlayers(){
		CharacterController[] players = FindObjectsOfType<CharacterController>();
		for (int i = 0; i < players.Length; i++) {
			players[i].GetComponent<RectTransform>().localScale = player_prefab.GetComponent<RectTransform>().localScale * server_vars.current_size;
		}
	}


	public void LaunchServer() {
		//Network.incomingPassword = "HolyMoly";
		bool useNat = Network.HavePublicAddress();
		Network.InitializeServer(32, 25000, useNat);
		//Network.maxConnections = max_players;
		MasterServer.RegisterHost ("gamea", Application.loadedLevelName);

	}

	public void ConnectToServer(){
		Network.Connect("127.0.0.1", 25000);
	}

	void spawnPlayer(){
		local_player = (GameObject)Network.Instantiate (player_prefab, map.transform.position, Quaternion.identity, 0);
		CharacterController[] players = FindObjectsOfType<CharacterController>();
		playerID = players.Length - 1;
	}

	void OnConnectedToServer() {
		Debug.Log("Connected to server");
		connecting = false;
	}

	void OnFailedToConnect(NetworkConnectionError error) {
		Debug.Log("Could not connect to server: " + error);
		connTPanel.text = "Client: Failed To Connect";
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		Debug.Log("Disconnected from server: " + info);
		connTPanel.text = "Client: Disconnected From Server";
	}
	
	
	void OnPlayerConnected(NetworkPlayer player) {
		Debug.Log("Player " + playerCount++ + " connected from " + player.ipAddress + ":" + player.port);
	}


}
