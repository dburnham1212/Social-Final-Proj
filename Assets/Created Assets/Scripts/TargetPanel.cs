using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TargetPanel : MonoBehaviour {

	public int panel_position = 0;
	public ServerVars server_vars;
	public CanvasGroup dead_overlay;
	public Text str_value;
	public Text int_value;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		CharacterController[] players = FindObjectsOfType<CharacterController> ();
		bool player_dead = false;
		for(int i = 0; i < players.Length; i++){
			if(players[i].currentID == panel_position){
				GetComponent<Image>().sprite = players[i].GetComponent<Image>().sprite;
				if(players[i].living == false){
					player_dead = true;
				}
				str_value.text = "" +  players[i].strength;
				int_value.text = "" + players[i].intelligence;
			}
			
		}
		if(player_dead){
			dead_overlay.alpha = 1;
			dead_overlay.blocksRaycasts = true;
		}
		else{
			dead_overlay.alpha = 0;
			dead_overlay.blocksRaycasts = false;
		}

		ServerVars serv_vars = FindObjectOfType<ServerVars> ();
		GameController g_conn = FindObjectOfType < GameController >();
		if (panel_position == g_conn.playerID) {
			transform.parent.GetComponent<Image>().color = Color.green;
		}
		else if (g_conn.curr_kill_target == panel_position && g_conn.out_of_game == false) {
			transform.parent.GetComponent<Image>().color = Color.red;
		}
		else{
			transform.parent.GetComponent<Image>().color = Color.white;
		}
	}
	
	public void selectTarget(){
		ServerVars s = FindObjectOfType<ServerVars> ();
		GameController g = FindObjectOfType < GameController >();
		CharacterController[] p = FindObjectsOfType<CharacterController> ();
		if(p.Length > panel_position && g.out_of_game == false){
			if (g.select_target == true) {
				g.curr_kill_target = panel_position;
				g.display_target_selector = false;
				g.select_target = false;
			}
			else{
				g.curr_kill_target = panel_position;
			}
		}

	}
}
