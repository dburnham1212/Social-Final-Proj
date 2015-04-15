using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

	public enum DoorTag{
		UP,
		DOWN,
		LEFT,
		RIGHT
	}

	public DoorTag door_tag;
	public GameObject door_spawn_pos;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.transform.GetComponent<CharacterController> () && col.transform.networkView.isMine) {
			bool can_traverse = false;
			for(int i = 0; i < FindObjectOfType<GameController>().current_map_info.activated_doors.Length; i++){
				if(door_tag == FindObjectOfType<GameController>().current_map_info.activated_doors[i]){
					can_traverse = true;
				}

			}
			if(can_traverse){
				col.transform.GetComponent<RectTransform> ().localPosition = door_spawn_pos.GetComponent<RectTransform> ().localPosition;
				if(door_tag == DoorTag.UP){
					col.transform.GetComponent<CharacterController> ().board_y -- ;
				}
				else if(door_tag == DoorTag.LEFT){
					col.transform.GetComponent<CharacterController> ().board_x -- ;
				}
				else if(door_tag == DoorTag.RIGHT){
					col.transform.GetComponent<CharacterController> ().board_x ++ ;
				}
				else if(door_tag == DoorTag.DOWN){
					col.transform.GetComponent<CharacterController> ().board_y ++ ;
				}
			}
		}
	}
}
