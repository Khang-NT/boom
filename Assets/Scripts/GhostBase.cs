using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class GhostBase : MonoBehaviour, MapManagerListener, IHpValue {

	public MapManager mapManager;
	public float speed = 1.8f;
	public int smartness = 1;
	public int maxHp = 1;

	protected int hp;
	protected bool requireUpdate = false;
	protected float requireUpdateTimeOut;
	protected Vector3 playerTracking;
	protected List<MapLocation> path;
	protected bool stopped = false;

	void Start () {
		this.hp = maxHp;
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
		if (gameObject != null)
			requireToUpdate ();
	}

	public void onAllGhostsDied() {

	}

	public void onPlayerDied() {
		stopped = true;
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag.Equals ("flame")) {
			this.hp--;
			if (this.hp == 0) {
				mapManager.removeListener (this);
				mapManager.removeGhost (this.gameObject);
				Destroy (this.gameObject);
			}
		}
	}

	public int Hp {
		get { return hp; }
	}

	public int MaxHp {
		get { return maxHp; }
	}

	public bool IsUnEffect {
		get { return false; }
	}

	// Update is called once per frame
	public void Update () {
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
					doUpdatePath ();
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

	public abstract void doUpdatePath ();
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