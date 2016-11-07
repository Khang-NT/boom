using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour, MapManagerListener, IHpValue {
	private static readonly int MAX_HP = 3;
	public MapManager mapManager;

	private SpriteRenderer spriteRenderer;
	public int state;
	public List<Sprite> playerStates;
	public float speed = 1f;

	private Rigidbody2D rigidBody;
	private int hp;
	private bool stopped = false;
	private float timer1;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		rigidBody = GetComponent<Rigidbody2D> ();
		this.hp = MAX_HP;
		this.timer1 = 2.5f;
		mapManager.addListener (this);
	}

	public int Hp {
		get {
			return hp;
		}
	}

	public int MaxHp {
		get {
			return MAX_HP;
		}
	}

	public bool IsUnEffect {
		get {
			return timer1 > 0;
		}
	}

	public void onMapReady() {
		// empty
	}

	public void onMapChanged() {
		// empty
	}

	public void onAllGhostsDied() {
		stopped = true;
	}

	public void onPlayerDied() {
		// empty
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (!IsUnEffect) {
			if (col.gameObject.tag.Equals ("flame")) {
				this.hp--;
			} else if (col.gameObject.tag.Equals ("ghost"))
				this.hp = 0;

			if (this.hp == 0) {
				GamePlay.getInstance ().Heart--;
				if (GamePlay.getInstance ().Heart == 0) {
					mapManager.removeListener (this);
					mapManager.removePlayer ();
					Destroy (this.gameObject);
				} else {
					MapLocation lc = new MapLocation (0, 0);
					do {
						lc.X = Random.Range (1, MapManager.MAP_WIDTH);
						lc.Y = Random.Range (1, MapManager.MAP_HEIGHT);
					} while(mapManager [lc.X, lc.Y] != null);
					this.transform.position = MapManager.mapLocationToVector3 (lc);
					this.hp = MAX_HP;
					this.timer1 = 2.5f;
				}
			}
		}
	}

	protected void setState(int state) {
		if (this.state != state) {
			this.state = state;
			spriteRenderer.sprite = playerStates [state];
		}
	}

	private void createBoom() {
		Vector3 position = MapManager.mapLocationToVector3 (MapManager.getMapLocation(gameObject));
		GameObject monster = (GameObject)Instantiate (Resources.Load ("bomb"));
		monster.transform.position = position;
	}
	
	// Update is called once per frame
	void Update () {
		if (timer1 > 0)
			timer1 -= Time.deltaTime;
		if (!stopped) {
			float y = Input.GetAxis ("Vertical");
			float x = Input.GetAxis ("Horizontal");
			var move = new Vector2 (
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
				createBoom ();
		} else {
			setState (0);
		}
	}
}
