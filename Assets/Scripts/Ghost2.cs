using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class Ghost2 : GhostBase {
	private static readonly int CELL_COUNT = MapManager.MAP_WIDTH * MapManager.MAP_HEIGHT;
	private readonly object syncLock = new object();


	public override void doUpdatePath() {
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


