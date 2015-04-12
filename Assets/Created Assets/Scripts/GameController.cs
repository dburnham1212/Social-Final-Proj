using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public bool is_server = true;
	private int playerCount = 0;
	private HostData[] host_data;
	private HostData curr_host_data;
	private int max_players = 8;
	private bool connected = false;

	public Text connTPanel;
	public Text lobby_list;

	void Awake() {

	}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		MasterServer.RequestHostList("gamea");
		host_data = MasterServer.PollHostList();
		int i = 0;
		lobby_list.text = "";
		while (i < host_data.Length) {
			lobby_list.text += host_data[i].gameName;
			Debug.Log("Game name: " + host_data[i].gameName);
			i++;
			if(!Network.isServer){
				if(host_data[i].connectedPlayers < max_players){
					curr_host_data = host_data[i];
				}
			}
		}
	}

	public void LaunchServer() {
		//Network.incomingPassword = "HolyMoly";
		bool useNat = !Network.HavePublicAddress();
		Network.InitializeServer(32, 25000, useNat);
		Network.maxConnections = max_players;
		MasterServer.RegisterHost ("gamea", Application.loadedLevelName);

	}

	public void ConnectToServer(){
		Network.Connect(curr_host_data);


	}

	void OnConnectedToServer() {
		Debug.Log("Connected to server");
	}

	void OnFailedToConnect(NetworkConnectionError error) {
		Debug.Log("Could not connect to server: " + error);
	}


	void OnPlayerConnected(NetworkPlayer player) {
		Debug.Log("Player " + playerCount++ + " connected from " + player.ipAddress + ":" + player.port);
	}
}
