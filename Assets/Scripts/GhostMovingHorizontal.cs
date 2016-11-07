using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostMovingHorizontal : GhostBase {
	bool moveRight = true;

	new void Update () {
		if (path == null || path.Count == 0)
			requireUpdate = true;
		base.Update ();
	}

	public override void doUpdatePath ()
	{
		if (path == null)
			path = new List<MapLocation> ();
		path.Clear ();
		MapLocation currentPos = MapManager.getMapLocation (gameObject);
		if (mapManager [currentPos.X + (moveRight ? 1 : -1), currentPos.Y] != null) {
			moveRight = !moveRight;
		}
		for (int i = currentPos.X + (moveRight ? 1 : -1); moveRight ? i < MapManager.MAP_WIDTH : i > 0; i += (moveRight ? 1 : -1)) {
			if (mapManager [i, currentPos.Y] == null)
				path.Add (new MapLocation (i, currentPos.Y));
			else
				break;
		}
		path.Reverse ();
	}
}
