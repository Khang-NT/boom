using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapLocation {
	public int X {
		get;
		set;
	}

	public int Y {
		get;
		set;
	}

	public MapLocation (int x, int y) {
		this.X = x;
		this.Y = y;
	}

	public override int GetHashCode() {
		return string.Format ("{0},{1}", X, Y).GetHashCode ();
	}

	public override bool Equals (object o)
	{
		if (o is MapLocation) {
			MapLocation m = (MapLocation)o;
			return m.X == X && m.Y == Y;
		}
		return false;
	}

	public override string ToString ()
	{
		return string.Format ("[MapLocation: X={0}, Y={1}]", X, Y);
	}
}

public enum MapObjectType {
	BRICK_UNDESTROYABLE,
	BRICK_DESTROYABLE,
	PLAYER,
	GHOST,
	ITEM,
	BOOM,
	FLAME
}

public interface MapManagerListener {
	void onMapReady ();
    void onMapChanged();
	void onPlayerDied();
	void onAllGhostsDied();
}

public class MapManager : MonoBehaviour {
	public static readonly int MAP_WIDTH = 15, MAP_HEIGHT = 13;

	private static MapManager sInstance;

	public static MapManager getInstance() {
		return sInstance;
	}


	private Dictionary<MapLocation, GameObject> bricks;
	private GameObject player;
	private List<GameObject> ghosts;
	private List<GameObject> booms;
	private List<GameObject> flame;
	public List<MapManagerListener> listeners = new List<MapManagerListener>();
	private Rect gameBound;
	private bool mapReady = false;
	public float defaultSpeed;
	private float cellSize;
	private float cellWidth, cellHeight;

	private bool[,] spaces = new bool[MAP_WIDTH,MAP_HEIGHT];


	void Start() {
		// init singleton instance
		MapManager.sInstance = this;

		// init game bound
		GameObject topLeft = GameObject.Find ("top_left"),
		bottomRight = GameObject.Find ("bottom_right");

		gameBound = new Rect ();
		gameBound.x = topLeft.transform.position.x;
		gameBound.y = bottomRight.transform.position.y;
		gameBound.width = bottomRight.transform.position.x - topLeft.transform.position.x;
		gameBound.height = topLeft.transform.position.y - bottomRight.transform.position.y;
		Debug.Log (gameBound);

		// fill spaces: true
		for (int i = 0; i < MAP_WIDTH; i++)
			for (int j = 0; j < MAP_HEIGHT; j++)
				spaces [i, j] = true;

		cellWidth = gameBound.width / (float) (MAP_WIDTH - 1);
		cellHeight = gameBound.height / (float) (MAP_HEIGHT - 1);
		cellSize = Mathf.Max (cellWidth, cellHeight);
		// find all bricks
		bricks = new Dictionary<MapLocation, GameObject> ();
		findAllBricks ();

		// find player and ghosts
		player = GameObject.Find ("player");

		Debug.Log (player);
		ghosts = new List<GameObject> ();
		GameObject[] gs = GameObject.FindGameObjectsWithTag ("ghost");
		foreach (GameObject go in gs)
			ghosts.Add (go);

		// create booms list as an empty list
		booms = new List<GameObject>();
		// create flame list as an empty list
		flame = new List<GameObject>();

		foreach (var listener in listeners) {
			listener.onMapReady ();
		}

		mapReady = true;
	}

	public void addListener(MapManagerListener listener) {
		listeners.Add (listener);
		if (mapReady)
			listener.onMapReady ();
	}

	public void removeListener(MapManagerListener listener) {
		listeners.Remove (listener);
	}

    public List<GameObject> getBooms() {
		return booms;
	}

	public List<GameObject> getFlame() {
		return flame;
	}

	public void registerFlame(GameObject f) {
		flame.Add (f);
		MapLocation flameLc = getMapLocation (f);
		spaces [flameLc.X, flameLc.Y] = false;
		foreach (var listener in listeners)
			listener.onMapChanged ();
	}

	public bool removeFlame(GameObject f) {
		bool res = flame.Remove (f);
		MapLocation flameLc = getMapLocation (f);
		spaces [flameLc.X, flameLc.Y] = true;
		foreach (var listener in listeners)
			listener.onMapChanged ();
		return res;
	}

    public void registerBoom(GameObject boom) {
		booms.Add (boom);
		MapLocation boomLc = getMapLocation (boom);
		spaces [boomLc.X, boomLc.Y] = false;
		foreach (var listener in listeners)
			listener.onMapChanged ();
	}

	public bool removeBoom(GameObject boom) {
		var res = booms.Remove (boom);
		// boom is replaced by flame
//		MapLocation boomLc = getMapLocation (boom);
//		spaces [boomLc.X, boomLc.Y] = true;
		foreach (var listener in listeners)
			listener.onMapChanged ();
		return res;
	}

	public void removeBrick(MapLocation brickLocation) {
		bricks.Remove (brickLocation);
		// brick is replaced by flames
//		spaces [brickLocation.X, brickLocation.Y] = true;
		// Dont trigger onMapChanged
//		foreach (var listener in listeners)
//			listener.onMapChanged ();
	}

	public void removeGhost(GameObject ghost) {
		ghosts.Remove (ghost);
		foreach (var listener in listeners)
			listener.onMapChanged ();
		if (ghosts.Count == 0)
			foreach (var listener in listeners)
				listener.onAllGhostsDied ();
	}

	public void removePlayer() {
		this.player = null;
		foreach (var listener in listeners)
			listener.onPlayerDied (); 
	}

    public GameObject getPlayer()
    {
        return player;
    }

	public float getCellSize() {
		return cellSize;
	}

	public GameObject this[int x, int y] {
		get {
			return getGameObjectAt(new MapLocation(x, y));
		}
	}

	public GameObject getGameObjectAt(MapLocation location) {
		if (bricks.ContainsKey (location))
			return bricks [location];
		if (player != null && getMapLocation (player).Equals(location))
			return player;
		foreach (var ghost in ghosts) {
			if (getMapLocation (ghost).Equals(location))
				return ghost;
		}
		foreach (var boom in booms) {
			if (getMapLocation (boom).Equals(location))
				return boom;
		}
		foreach (var f in flame) {
			if (getMapLocation (f).Equals(location))
				return f;
		}
		return null;
	}

	private void findAllBricks() {
		bricks.Clear ();
		GameObject[] brickObjects = GameObject.FindGameObjectsWithTag ("brick");
		foreach (GameObject brick in brickObjects) {
			MapLocation location = vector3ToMapLocation (brick.transform.position);
			spaces [location.X, location.Y] = false;
			bricks [location] = brick;
		}
	}

	public bool[,] getSpaces() {
		return spaces;
	}

	public static MapLocation getMapLocation(GameObject gameObj) {
		return vector3ToMapLocation (gameObj.transform.position);
	}

    public static MapObjectType getTypeOf(GameObject go) {
		if (go.tag.Equals ("brick")) {
			if (go.name.StartsWith ("destroyable"))
				return MapObjectType.BRICK_DESTROYABLE;
			else
				return MapObjectType.BRICK_UNDESTROYABLE;
		}
		if (go.tag.Equals("ghost"))
			return MapObjectType.GHOST;
		if (go.tag.Equals ("boom"))
			return MapObjectType.BOOM;
		if (go.tag.Equals ("item"))
			return MapObjectType.ITEM;
		if (go.name.Equals("player"))
			return MapObjectType.PLAYER;
		if (go.tag.Equals ("flame"))
			return MapObjectType.FLAME;
		throw new UnityException ("Unknown game object type in map: " + go.ToString());
	}



    public static MapLocation vector3ToMapLocation(Vector3 position) {
		int x = Mathf.RoundToInt((position.x - sInstance.gameBound.x) / sInstance.cellWidth);
		int y = Mathf.RoundToInt((position.y - sInstance.gameBound.y) / sInstance.cellHeight);
		return new MapLocation (x >= MAP_WIDTH ? MAP_WIDTH - 1 : x, y >= MAP_HEIGHT ? MAP_HEIGHT - 1 : y);
	}

	public static Vector3 mapLocationToVector3(MapLocation lc) {
		return new Vector3 (
			sInstance.gameBound.x + lc.X * (sInstance.gameBound.width / (float) (MAP_WIDTH - 1)),
			sInstance.gameBound.y + lc.Y * (sInstance.gameBound.height / (float) (MAP_HEIGHT - 1))
		);
	}
}
