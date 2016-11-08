using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostMovingVertical : GhostBase {
	bool moveUp = true;

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
		if (mapManager [currentPos.X, currentPos.Y + (moveUp ? 1 : -1)] != null) {
			moveUp = !moveUp;
		}
		for (int i = currentPos.Y + (moveUp ? 1 : -1); moveUp ? i < MapManager.MAP_HEIGHT : i > 0; i += (moveUp ? 1 : -1)) {
			var go = mapManager [i, currentPos.Y];
			if (go == null || go.name.Equals("player"))
				path.Add (new MapLocation (i, currentPos.Y));
			else
				break;
		}
		path.Reverse ();
	}
}
