using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public MapManager mapManager;

	private SpriteRenderer spriteRenderer;
	public int state;
	public List<Sprite> playerStates;
	public float speed = 1f;


	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	protected void setState(int state) {
		if (this.state != state) {
			this.state = state;
			spriteRenderer.sprite = playerStates [state];
		}
	}
	
	// Update is called once per frame
	void Update () {
		float y = Input.GetAxis ("Vertical");
		float x = Input.GetAxis ("Horizontal");
		var move = new Vector3(
			x * speed * Time.deltaTime * mapManager.defaultSpeed, 
			y * speed * Time.deltaTime * mapManager.defaultSpeed, 
			0);

		transform.Translate (move);

		if (y < 0)
			setState (0);
		else if (y > 0)
			setState (2);
		else if (x < 0)
			setState (3);
		else if (x > 0)
			setState (1);
	}
}
