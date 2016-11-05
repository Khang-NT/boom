using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour, MapManagerListener {

	public MapManager mapManager;

	private SpriteRenderer spriteRenderer;
	public int state;
	public List<Sprite> playerStates;
	public float speed = 1f;

	private bool boomDelay = false;
	private Rigidbody2D rigidBody;


	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		rigidBody = GetComponent<Rigidbody2D> ();
		mapManager.addListener (this);
	}

	public void onMapReady() {
		MapLocation lc = MapManager.getMapLocation (gameObject);
		Vector3 position = MapManager.mapLocationToVector3 (lc);
	}

	public void onMapChanged() {

	}

	protected void setState(int state) {
		if (this.state != state) {
			this.state = state;
			spriteRenderer.sprite = playerStates [state];
		}
	}

	private bool createBoom() {
		MapLocation lc = MapManager.getMapLocation (gameObject);
		GameObject go = mapManager [lc.X, lc.Y];
		if (go == null || go.name == "player") {
			Vector3 position = MapManager.mapLocationToVector3 (lc);
			GameObject monster = (GameObject)Instantiate (Resources.Load ("bomb"));
			monster.transform.position = position;
		
			return true;
		}
		return false;
	}
	
	// Update is called once per frame
	void Update () {
		float y = Input.GetAxis ("Vertical");
		float x = Input.GetAxis ("Horizontal");
		var move = new Vector2(
			x * speed * Time.deltaTime * mapManager.defaultSpeed, 
			y * speed * Time.deltaTime * mapManager.defaultSpeed);

		rigidBody.velocity = move;

		if (y < 0)
			setState (0);
		else if (y > 0)
			setState (2);
		else if (x < 0)
			setState (3);
		else if (x > 0)
			setState (1);

		if (Input.GetKeyDown ("space"))
			boomDelay = true;

		if (boomDelay && createBoom ())
			boomDelay = false;
	}
}
