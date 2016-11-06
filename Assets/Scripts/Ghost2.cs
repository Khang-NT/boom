using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class Ghost2 : MonoBehaviour, MapManagerListener {
	private static readonly int CELL_COUNT = MapManager.MAP_WIDTH * MapManager.MAP_HEIGHT;
	private readonly object syncLock = new object();

	public MapManager mapManager;
	public float speed;
	public int smartness = 1;

//	private Rigidbody2D rgBody;
	private bool requireUpdate = false;
	private float requireUpdateTimeOut;
	private Vector3 playerTracking;
	private List<MapLocation> path;
	private bool stopped = false;
	// Use this for initialization
	void Start () {
//		rgBody = GetComponent<Rigidbody2D> ();

		mapManager.addListener (this);
	}

	void requireToUpdate() {
		this.requireUpdate = true;
		this.requireUpdateTimeOut = 0.1f; // 100 miliseconds	
	}

	public void onMapReady() {
		playerTracking = mapManager.getPlayer ().transform.position;
		requireToUpdate ();
	}

	public void onMapChanged()
	{
		requireToUpdate ();
	}

	public void onAllGhostsDied() {

	}

	public void onPlayerDied() {
		stopped = true;
	}

	void OnTriggerEnter2D(Collider2D col) {
		print (col.gameObject.name);
		print (col.gameObject.tag);
	}
	
	// Update is called once per frame
	void Update () {
		if (!stopped) {
			if (Vector3.Distance (mapManager.getPlayer ().transform.position, playerTracking) > mapManager.getCellSize ()) {
				playerTracking = mapManager.getPlayer ().transform.position;
				requireUpdate = true;
			}

			if (requireUpdate) {
				if (requireUpdateTimeOut > 0)
					requireUpdateTimeOut -= Time.deltaTime;
				else {
					requireUpdate = false;
					MapLocation startPos = MapManager.getMapLocation (gameObject);
					if (smartness < 3 || smartness == 4 || mapManager.getBooms ().Count == 0) {
						MapLocation targetPos = MapManager.getMapLocation (mapManager.getPlayer ());
						findShortestPath (startPos, targetPos);
						if (smartness == 4 && path == null)
							findSafestPlace (startPos);
					} else {
						findSafestPlace (startPos);
					}
				}
			}
			if (path == null || path.Count == 0) {
				// stop moving
			} else {
				Vector3 target = MapManager.mapLocationToVector3 (path [path.Count - 1]);
				Vector3 move = target - transform.position;
				float maxDistance = speed * Time.deltaTime;
				if (move.sqrMagnitude <= maxDistance) {
					transform.position = target;
					path.RemoveAt (path.Count - 1);
				} else {
					move = move.normalized * speed * Time.deltaTime;
					transform.Translate (move);
				}
			}
		}
	}

	protected void loadShortestMap(List<MapLocation> newPath) {
		lock (syncLock) {
			this.path = newPath;
		}
	}

	void findShortestPath(MapLocation startPos, MapLocation targetPos) {
		Stopwatch sw = new Stopwatch ();
		sw.Start ();
		Node startNode = new Node (startPos);
		Node targetNode = new Node(targetPos);

		Heap<Node> openSet = new Heap<Node>(CELL_COUNT);
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.add(startNode);

		while (openSet.size () > 0) {
			Node currentNode = openSet.RemoveFirst();
			closedSet.Add(currentNode);

			if (smartness == 4 && !currentNode.Equals(startNode)) {
				bool isWalkable = true;
				foreach (var b in mapManager.getBooms()) {
					Boom boom = b.GetComponent<Boom> ();
					MapLocation lc = MapManager.getMapLocation (b);
					if ((Math.Abs (lc.X - currentNode.X) < boom.radius && (lc.Y == currentNode.Y)) ||
					    Math.Abs (lc.Y - currentNode.Y) < boom.radius && (lc.X == currentNode.X)) {
						isWalkable = false;
						break;
					} else {
						continue;
					}
				}
				if (!isWalkable)
					continue;
			}

			if (currentNode.X == targetNode.X && currentNode.Y == targetNode.Y) {
				sw.Stop ();
//				print ("Path found: " + sw.ElapsedMilliseconds + " ms");
				retracePath(startNode,currentNode);
				return;
			}

			foreach (Node neighbour in getNeighbours(currentNode)) {
				if (closedSet.Contains(neighbour)) {
					continue;
				}

				int newMovementCostToNeighbour = currentNode.gCost + getDistance(currentNode, neighbour);
				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.contain(neighbour)) {
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = getDistance(neighbour, targetNode);
					neighbour.parent = currentNode;

					if (!openSet.contain(neighbour))
						openSet.add(neighbour);
				}
			}
		}
		sw.Stop ();
//		print ("Path not found: " + sw.ElapsedMilliseconds + " ms");
		loadShortestMap (null);
	}

	void findSafestPlace(MapLocation startPos) {
		Stopwatch sw = new Stopwatch ();
		sw.Start ();
		Node startNode = new Node (startPos);
		List<MapLocation> boomLcs = new List<MapLocation> ();
		foreach (var go in mapManager.getBooms()) {
			boomLcs.Add (MapManager.getMapLocation (go));
		}

		if (boomLcs.Count == 0) {
			loadShortestMap (null);
			return;
		}

		Heap<Node> openSet = new Heap<Node>(CELL_COUNT);
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.add(startNode);

		while (openSet.size () > 0) {
			Node currentNode = openSet.RemoveFirst();
			closedSet.Add(currentNode);

			bool isSafe = true;
			foreach (var lc in boomLcs)
				if (currentNode.X != lc.X && currentNode.Y != lc.Y)
					continue;
				else {
					isSafe = false; break;
				}
			if (isSafe) {
				sw.Stop ();
				//				print ("Path found: " + sw.ElapsedMilliseconds + " ms");
				retracePath(startNode,currentNode);
				return;
			}

			foreach (Node neighbour in getNeighbours(currentNode)) {
				if (closedSet.Contains(neighbour)) {
					continue;
				}

				int newMovementCostToNeighbour = currentNode.gCost + getDistance(currentNode, neighbour);
				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.contain(neighbour)) {
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = 1;
					neighbour.parent = currentNode;

					if (!openSet.contain(neighbour))
						openSet.add(neighbour);
				}
			}
		}

		sw.Stop ();
		//		print ("Path not found: " + sw.ElapsedMilliseconds + " ms");
		loadShortestMap (null);
	}

	int getDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.X - nodeB.X);
		int dstY = Mathf.Abs (nodeA.Y - nodeB.Y);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}

	List<Node> getNeighbours(Node currentNode) {
		bool[,] spaces = mapManager.getSpaces ();
		MapLocation lc = MapManager.getMapLocation (mapManager.getPlayer ());
		bool bk = spaces [lc.X, lc.Y];
		if (smartness == 1)
			spaces [lc.X, lc.Y] = true;
		List<Node> neighbours = new List<Node> ();
		bool left = false, top = false, bottom = false, right = false;
		for (int i = -1; i <= 1; i++)
			for (int j = -1; j <= 1; j++) {
				if (i == 0 && j == 0)
					continue;
				if (i * j != 0)
					continue;
				int x = currentNode.X + i;
				int y = currentNode.Y + j;
				if (x < 0 || y < 0 || x >= MapManager.MAP_WIDTH || y >= MapManager.MAP_HEIGHT)
					continue;
				if (spaces [x, y]) {
					neighbours.Add (new Node (x, y));
					if (i > 0)
						right = true;
					else if (i < 0)
						left = true;
					else if (j > 0)
						top = true;
					else if (j < 0)
						bottom = true;
				}
			}
		if (top && left && spaces [currentNode.X - 1, currentNode.Y + 1])
			neighbours.Add (new Node (currentNode.X - 1, currentNode.Y + 1));
		if (top && right && spaces[currentNode.X + 1, currentNode.Y + 1])
			neighbours.Add (new Node (currentNode.X + 1, currentNode.Y + 1));
		if (bottom && left && spaces [currentNode.X - 1, currentNode.Y - 1])
			neighbours.Add (new Node (currentNode.X - 1, currentNode.Y - 1));
		if (bottom && right && spaces [currentNode.X + 1, currentNode.Y - 1])
			neighbours.Add (new Node (currentNode.X + 1, currentNode.Y - 1));
		if (smartness == 1)
			spaces [lc.X, lc.Y] = bk;
		return neighbours;
	}

	void retracePath(Node startNode, Node endNode) {
//		print (startNode);
//		print (endNode);
		List<MapLocation> path = new List<MapLocation>();
		Node currentNode = endNode;

		while (currentNode.parent != null) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
//		path.Reverse();

		loadShortestMap (path);
	}
}


public class Node : MapLocation, IHeadItem<Node> {
	public Node parent;
	public int gCost;
	public int hCost;

	public int heapIndex_;

	public Node(MapLocation source) : this(source.X, source.Y) {

	}

	public Node(int x, int y) : base(x , y) {
		gCost = hCost = 0;
	}

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int heapIndex {
		get {
			return heapIndex_;
		}
		set {
			heapIndex_ = value;
		}
	}

	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}

public class Heap<T> where T : IHeadItem<T> {
	T[] items;
	int currentItemCount;

	public Heap(int maxSize) {
		items = new T[maxSize];
	}

	public void add(T item) {
		item.heapIndex = currentItemCount;
		items [currentItemCount] = item;
		sortUp (item);
		currentItemCount++;
	}

	public void updateItem(T item) {
		sortUp(item);
	}

	public T RemoveFirst() {
		T firstItem = items[0];
		currentItemCount--;
		items[0] = items[currentItemCount];
		items[0].heapIndex = 0;
		sortDown(items[0]);
		return firstItem;
	}

	public int size() {
		return currentItemCount;
	}

	public bool contain(T item) {
		return Equals(items[item.heapIndex], item);
	}

	void sortDown(T item) {
		while (true) {
			int childIndexLeft = item.heapIndex * 2 + 1;
			int childIndexRight = item.heapIndex * 2 + 2;
			int swapIndex = 0;

			if (childIndexLeft < currentItemCount) {
				swapIndex = childIndexLeft;

				if (childIndexRight < currentItemCount) {
					if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) {
						swapIndex = childIndexRight;
					}
				}

				if (item.CompareTo(items[swapIndex]) < 0) {
					swap (item,items[swapIndex]);
				} else {
					return;
				}

			} else {
				return;
			}

		}
	}

	void sortUp(T item) {
		int parentIndex = (item.heapIndex - 1) / 2;
		while (true) {
			T parent = items [parentIndex];
			if (item.CompareTo (parent) > 0) {
				swap (item, parent);
			} else
				break;
			parentIndex = (item.heapIndex - 1) / 2;
		}
	}

	void swap(T itemA, T itemB) {
		items[itemA.heapIndex] = itemB;
		items[itemB.heapIndex] = itemA;
		int itemAIndex = itemA.heapIndex;
		itemA.heapIndex = itemB.heapIndex;
		itemB.heapIndex = itemAIndex;
	}
}

public interface IHeadItem<T> : IComparable<T> {
	int heapIndex {
		get;
		set;
	}
}