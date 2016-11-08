using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostMovingCircle : GhostBase {

	public bool clockDirection;
	public bool flip;
	int state;

	new void Update () {
		if (path == null || path.Count == 0)
			requireUpdate = true;
		base.Update ();
	}


	private void onStateChanged() {
		if (clockDirection) {
			GetComponent<SpriteRenderer> ().flipX = (state == 0 || state == 3) ? !flip : flip;
		} else {
			GetComponent<SpriteRenderer> ().flipX = (state == 0 || state == 3) ? flip : !flip;
		}
	}

	public override void doUpdatePath ()
	{
		if (path == null)
			path = new List<MapLocation> ();
		path.Clear ();
		for (int i = 0; i < 4; i++) {
			if (updateInternal (clockDirection ? 3 - (state + i) % 4 : (state + i) % 4)) {
				this.state = clockDirection ? 3 - (state + i) % 4 : (state + i) % 4;
				onStateChanged ();
				break;
			}
		}
		path.Reverse ();
	}

	private bool updateInternal(int state) {
		MapLocation currentPos = MapManager.getMapLocation (gameObject);
		GameObject go;
		if ((state == 0 && !clockDirection) || (clockDirection && state == 2)) {
			for (int i = currentPos.X + 1; i < MapManager.MAP_WIDTH; i++) {
				go = mapManager [i, currentPos.Y];
				if (go == null || go.name.Equals ("player"))
					path.Add (new MapLocation (i, currentPos.Y));
				else
					break;
			}
		} else if ((state == 1 && !clockDirection) || (clockDirection && state == 3)) {
			for (int i = currentPos.Y + 1; i < MapManager.MAP_HEIGHT; i++) {
				go = mapManager [currentPos.X, i];
				if (go == null || go.name.Equals ("player"))
					path.Add (new MapLocation (currentPos.X, i));
				else
					break;
			}
		} else if ((state == 2 && !clockDirection) || (clockDirection && state == 0)) {
			for (int i = currentPos.X - 1; i > 0; i--) {
				go = mapManager [i, currentPos.Y];
				if (go == null || go.name.Equals ("player"))
					path.Add (new MapLocation (i, currentPos.Y));
				else
					break;
			}
		} else if ((state == 3 && !clockDirection) || (clockDirection && state == 1)) {
			for (int i = currentPos.Y - 1; i > 0; i--) {
				go = mapManager [currentPos.X, i];
				if (go == null || go.name.Equals ("player"))
					path.Add (new MapLocation (currentPos.X, i));
				else
					break;
			}
		}

		return path.Count > 0;
	}
}
