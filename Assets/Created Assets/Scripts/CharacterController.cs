using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {
	public float speed;
	public bool initial = false;
	public int current_level;
	public int currentID;
	public bool living = true;
	public int guess_target;
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
		}
		else{
		}

		if (Network.isServer) {
			networkView.RPC("updateLiving", RPCMode.All, living);
		}

		if(living == false){
			if(networkView.isMine){
				FindObjectOfType<GameController>().out_of_game = true;
			}
			GetComponent<CanvasGroup>().alpha = 0;
		}
		else{
			GetComponent<CanvasGroup>().alpha = 1;
		}
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
