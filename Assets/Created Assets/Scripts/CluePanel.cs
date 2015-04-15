using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CluePanel : MonoBehaviour {
	
	public int strength = 0;
	public int intelligence = 0;

	public Text str_text;
	public Text int_text;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(strength != 0){
			str_text.text = "" + strength;
		}
		if (intelligence != 0) {
			int_text.text = "" + intelligence;
		}
	}
}
