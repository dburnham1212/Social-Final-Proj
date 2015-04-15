using UnityEngine;
using System.Collections;

[System.Serializable]
public class MapPanelInfo : MonoBehaviour {
	public enum PickupType{
		CLUE = 0,
		COIN = 1,
		XP = 2
	}

	public Door.DoorTag[] activated_doors;
	public int door_x = 0;
	public int door_y = 0;

	public PickupType pick_up_type;
	public bool round_item_taken = false;

	public void updateInClients(){
		if(Network.isServer){
			networkView.RPC("updateRoundItem", RPCMode.All, false);
			networkView.RPC("updatePickUp", RPCMode.All, (int)pick_up_type);
		}
	}

	[RPC]
	void updateRoundItem(bool new_item_taken){
		round_item_taken = new_item_taken;
	}
	[RPC]
	void updatePickUp(int new_p){
		pick_up_type = (PickupType)new_p;
	}

	
}
