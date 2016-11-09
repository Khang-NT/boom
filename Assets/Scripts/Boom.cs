using UnityEngine;
using System.Collections;

public class Boom : MonoBehaviour {

	public MapManager mapManager;

	public float time;	// 2 seconds
	public int radius = 2;

	private BoxCollider2D boxCollider;
	public AudioClip explosionSound;

	private bool stop = false;

	// Use this for initialization
	void Start () {
		mapManager = MapManager.getInstance ();
//		mapManager.registerBoom (this.gameObject); playe will do this stuff
		boxCollider = GetComponent<BoxCollider2D> ();
	}

	void onExploded() {
		MapLocation lc = MapManager.getMapLocation (this.gameObject);
		GameObject flame = (GameObject)Instantiate (Resources.Load ("flame"));
		flame.transform.position = transform.position;
		for (int i = 1; i < radius; i++) {
			GameObject go = mapManager [lc.X, lc.Y + i];
			MapLocation lc2 = new MapLocation (lc.X, lc.Y + i);
			Vector3 pos = MapManager.mapLocationToVector3 (lc2);
			if (go == null) {
				((GameObject)Instantiate (Resources.Load ("flame"))).transform.position = pos;
			} else {
				MapObjectType type = MapManager.getTypeOf (go);
				bool shouldBreak = false;
				switch (type) {
				case MapObjectType.GHOST:
				case MapObjectType.ITEM:
				case MapObjectType.PLAYER:
				case MapObjectType.BOOM:
				case MapObjectType.FLAME:
					((GameObject)Instantiate (Resources.Load ("flame"))).transform.position = pos;
					break;
				case MapObjectType.BRICK_UNDESTROYABLE:
					shouldBreak = true;
					break;
				case MapObjectType.BRICK_DESTROYABLE:
					shouldBreak = true;
					mapManager.removeBrick (lc2);
					Destroy (go);
					((GameObject)Instantiate (Resources.Load ("flame"))).transform.position = pos;
					break;	
				}
				if (shouldBreak)
					break;
			}
		}
		for (int i = 1; i < radius; i++) {
			GameObject go = mapManager [lc.X, lc.Y - i];
			MapLocation lc2 = new MapLocation (lc.X, lc.Y - i);
			Vector3 pos = MapManager.mapLocationToVector3 (lc2);
			if (go == null) {
				((GameObject)Instantiate (Resources.Load ("flame"))).transform.position = pos;
			} else {
				MapObjectType type = MapManager.getTypeOf (go);
				bool shouldBreak = false;
				switch (type) {
				case MapObjectType.GHOST:
				case MapObjectType.ITEM:
				case MapObjectType.PLAYER:
				case MapObjectType.BOOM:
				case MapObjectType.FLAME:
					((GameObject)Instantiate (Resources.Load ("flame"))).transform.position = pos;
					break;
				case MapObjectType.BRICK_UNDESTROYABLE:
					shouldBreak = true;
					break;
				case MapObjectType.BRICK_DESTROYABLE:
					shouldBreak = true;
					mapManager.removeBrick (lc2);
					Destroy (go);
					((GameObject)Instantiate (Resources.Load ("flame"))).transform.position = pos;
					break;	
				}
				if (shouldBreak)
					break;
			}
		}

		for (int i = 1; i < radius; i++) {
			GameObject go = mapManager [lc.X + i, lc.Y];
			MapLocation lc2 = new MapLocation (lc.X + i, lc.Y);
			Vector3 pos = MapManager.mapLocationToVector3 (lc2);
			if (go == null) {
				((GameObject)Instantiate (Resources.Load ("flame"))).transform.position = pos;
			} else {
				MapObjectType type = MapManager.getTypeOf (go);
				bool shouldBreak = false;
				switch (type) {
				case MapObjectType.GHOST:
				case MapObjectType.ITEM:
				case MapObjectType.PLAYER:
				case MapObjectType.BOOM:
				case MapObjectType.FLAME:
					((GameObject)Instantiate (Resources.Load ("flame"))).transform.position = pos;
					break;
				case MapObjectType.BRICK_UNDESTROYABLE:
					shouldBreak = true;
					break;
				case MapObjectType.BRICK_DESTROYABLE:
					shouldBreak = true;
					mapManager.removeBrick (lc2);
					Destroy (go);
					((GameObject)Instantiate (Resources.Load ("flame"))).transform.position = pos;
					break;	
				}
				if (shouldBreak)
					break;
			}
		}

		for (int i = 1; i < radius; i++) {
			GameObject go = mapManager [lc.X - i, lc.Y];
			MapLocation lc2 = new MapLocation (lc.X - i, lc.Y);
			Vector3 pos = MapManager.mapLocationToVector3 (lc2);
			if (go == null) {
				((GameObject)Instantiate (Resources.Load ("flame"))).transform.position = pos;
			} else {
				MapObjectType type = MapManager.getTypeOf (go);
				bool shouldBreak = false;
				switch (type) {
				case MapObjectType.GHOST:
				case MapObjectType.ITEM:
				case MapObjectType.PLAYER:
				case MapObjectType.BOOM:
				case MapObjectType.FLAME:
					((GameObject)Instantiate (Resources.Load ("flame"))).transform.position = pos;
					break;
				case MapObjectType.BRICK_UNDESTROYABLE:
					shouldBreak = true;
					break;
				case MapObjectType.BRICK_DESTROYABLE:
					shouldBreak = true;
					mapManager.removeBrick (lc2);
					Destroy (go);
					((GameObject)Instantiate (Resources.Load ("flame"))).transform.position = pos;
					break;	
				}
				if (shouldBreak)
					break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!boxCollider.enabled) {
			float distance = mapManager.getPlayer () == null ? 100 : 
				Vector3.Distance (mapManager.getPlayer ().transform.position, transform.position);
			if (distance >= mapManager.getCellSize ())
				boxCollider.enabled = true;
		}

		time -= Time.deltaTime;
		if (!stop && time <= 0) {
			stop = true;
			mapManager.removeBoom (this.gameObject);
			onExploded ();
			gameObject.GetComponent<AudioSource> ().PlayOneShot (explosionSound);
			Destroy (gameObject, explosionSound.length);
		}
	}
}
