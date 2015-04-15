using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {
	public float speed;
	public bool initial = false;
	public int current_level;
	public int currentID;
	public bool living = true;
	public int guess_target;
	 


	public int board_x = 0;
	public int board_y = 0;

	public int strength;
	public int intelligence;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		rigidbody2D.velocity = Vector2.zero;
		if(networkView.isMine){
			if(living){
				if (Input.GetKey (KeyCode.W)) {
					rigidbody2D.velocity = new Vector2(0, 1) * speed;
				}
				if (Input.GetKey (KeyCode.S)) {
					rigidbody2D.velocity = new Vector2(0, -1) * speed;
				}
				if (Input.GetKey (KeyCode.A)) {
					rigidbody2D.velocity = new Vector2(-1, 0) * speed;
				}
				if (Input.GetKey (KeyCode.D)) {
					rigidbody2D.velocity = new Vector2(1, 0) * speed;
				}
			}
		
			networkView.RPC("updateID", RPCMode.All, FindObjectOfType<GameController>().playerID);

			guess_target = FindObjectOfType<GameController>().curr_kill_target;
			// update values over network
			networkView.RPC("updateKillTarget", RPCMode.All, guess_target);
			GameController g_cont = FindObjectOfType<GameController>();
			for(int i = 0; i < g_cont.map_info.Length; i++){
				if(g_cont.map_info[i].door_x == board_x && g_cont.map_info[i].door_y == board_y){
					g_cont.current_map_info = g_cont.map_info[i];
				}
			}
			CharacterController[] players = FindObjectsOfType<CharacterController>();
			for(int i = 0; i < players.Length; i++){
				if(players[i].board_x != board_x || players[i].board_y != board_y){
					players[i].GetComponent<CanvasGroup>().alpha = 0;
					players[i].GetComponent<Collider2D>().enabled = false;
				}
				else{
					if(players[i].living){
						players[i].GetComponent<CanvasGroup>().alpha = 1;
						players[i].GetComponent<Collider2D>().enabled = true;
					}
				}
			}

			strength = g_cont.strength;
			intelligence = g_cont.intelligence;

			networkView.RPC("updateBoardX", RPCMode.All, board_x);
			networkView.RPC("updateBoardY", RPCMode.All, board_y);

			networkView.RPC("updateIntel", RPCMode.All, intelligence);
			networkView.RPC("updateStre", RPCMode.All, strength);
		}

		// take server values and disperse them to the characters
		if (Network.isServer) {
			networkView.RPC("updateLiving", RPCMode.All, living);
		}


		
		if(living == false){
			if(networkView.isMine){
				FindObjectOfType<GameController>().out_of_game = true;
			}
		}
	}

	[RPC]
	void updateBoardX(int new_x){
		board_x = new_x;
	}

	[RPC]
	void updateBoardY(int new_y){
		board_y = new_y;
	}

	[RPC]
	void updateIntel(int new_intel){
		intelligence = new_intel;
	}
	
	[RPC]
	void updateStre(int new_str){
		strength = new_str;
	}

	[RPC]
	void updateID(int new_id){
		currentID = new_id;
	}

	[RPC]
	void updateLiving(bool new_living){
		living = new_living;
	}

	[RPC]
	void updateKillTarget(int new_target){
		guess_target = new_target;
		
	}
}
