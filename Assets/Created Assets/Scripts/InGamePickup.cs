using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGamePickup : MonoBehaviour {
	public bool triggered = false;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GameController g_conn = FindObjectOfType<GameController> ();
		ServerVars s_vars = FindObjectOfType<ServerVars> ();
		if (g_conn.current_map_info.round_item_taken) {
			GetComponent<Collider2D>().enabled = false;
			GetComponent<CanvasGroup>().alpha = 0;
		}
		else{
			GetComponent<Collider2D>().enabled = true;
			GetComponent<CanvasGroup>().alpha = 1;

			if(g_conn.current_map_info.pick_up_type == MapPanelInfo.PickupType.COIN){
				GetComponent<Image>().sprite = s_vars.coin_sprite;
			}
			else if(g_conn.current_map_info.pick_up_type == MapPanelInfo.PickupType.XP){
				GetComponent<Image>().sprite = s_vars.xp_sprite;
			}
			else{
				GetComponent<Image>().sprite = s_vars.clue_options[s_vars.curr_clue].clue_sprite;
			}
		}
	
	}

	void OnTriggerEnter2D(Collider2D col){
		GameController g_conn = FindObjectOfType<GameController> ();
		ServerVars s_vars = FindObjectOfType<ServerVars> ();
		triggered = true;
		if (g_conn.current_map_info.pick_up_type == MapPanelInfo.PickupType.CLUE) {
			s_vars.found_clues.Add(s_vars.clue_options[s_vars.curr_clue]);
		}
		g_conn.current_map_info.round_item_taken = true;
	}
}
