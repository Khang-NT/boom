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

// trên map thì mình có mấy loại này thôi
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

    // này là get ra  list boom, 
    // khi t viết thằng player, nếu đặt boom thì t sẽ gọi
	public List<GameObject> getBooms() {
		return booms;
	}

	public List<GameObject> getFlame() {
		return flame;
	}

	public void registerFlame(GameObject f) {
		flame.Add (f);
	}

	public bool removeFlame(GameObject f) {
		return flame.Remove (f);
	}

    // nói chung map manager nó sẽ biết dc hết đối tượng trên bản đồ
    // chỉ trừ mấy cái item thôi, cái đó t handle trong thằng player nên khỏi quan tâm
    //
	public void registerBoom(GameObject boom) {
		booms.Add (boom);
	}

	public bool removeBoom(GameObject boom) {
		return booms.Remove (boom);
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
		if (player != null && getMapLocation (player) == location)
			return player;
		foreach (var ghost in ghosts) {
			if (getMapLocation (ghost) == location)
				return ghost;
		}
		foreach (var boom in booms) {
			if (getMapLocation (boom) == location)
				return boom;
		}
		foreach (var f in flame) {
			if (getMapLocation (f) == location)
				return f;
		}
		return null;
	}

	private void findAllBricks() {
		bricks.Clear ();
		GameObject[] brickObjects = GameObject.FindGameObjectsWithTag ("brick");
		foreach (GameObject brick in brickObjects) {
			MapLocation location = vector3ToMapLocation (brick.transform.position);
			bricks [location] = brick;
		}
	}

	public static MapLocation getMapLocation(GameObject gameObj) {
		return vector3ToMapLocation (gameObj.transform.position);
	}

    // này để check xem cái game object đó là cái gì
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
		throw new UnityException ("Unknown game object type in map: " + go.ToString());
	}



    // hai cái này cũng quan trong, ví dụ như ông viết cái script ghost, muốn
    // biết 1 cái game object bất kì ở tọa độ nào thì dùng hàm này
	public static MapLocation vector3ToMapLocation(Vector3 position) {
		// Debug.Log ((position.x - sInstance.gameBound.x) / sInstance.cellWidth);
		int x = Mathf.RoundToInt((position.x - sInstance.gameBound.x) / sInstance.cellWidth);
		// Debug.Log (x);
		// Debug.Log ("--");
		// Debug.Log((position.y - sInstance.gameBound.y) / sInstance.cellHeight);
		int y = Mathf.RoundToInt((position.y - sInstance.gameBound.y) / sInstance.cellHeight);
		// Debug.Log (y);
		return new MapLocation (x >= MAP_WIDTH ? MAP_WIDTH - 1 : x, y >= MAP_HEIGHT ? MAP_HEIGHT - 1 : y);
	}

    // này chuyển từ tọa độ index x y trong map sang tọa độ x y trong unity
    // 
	public static Vector3 mapLocationToVector3(MapLocation lc) {
		return new Vector3 (
			sInstance.gameBound.x + lc.X * (sInstance.gameBound.width / (float) (MAP_WIDTH - 1)),
			sInstance.gameBound.y + lc.Y * (sInstance.gameBound.height / (float) (MAP_HEIGHT - 1))
		);
	}
}
