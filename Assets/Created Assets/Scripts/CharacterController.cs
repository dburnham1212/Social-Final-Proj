using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {



	public float speed;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		rigidbody2D.velocity = Vector2.zero;
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



}
