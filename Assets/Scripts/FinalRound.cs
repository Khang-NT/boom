using UnityEngine;
using System.Collections;

public class FinalRound : MonoBehaviour, MapManagerListener {

	public MapManager mapManager;

	bool isOpened = false;

	// Use this for initialization
	void Start () {
		mapManager.addListener (this);
	}

	public void onMapReady() {

	}

	public void onMapChanged() {
		if (!isOpened && mapManager.getGhosts ().Count == 1) {
			isOpened = true;
			foreach (var brick in GameObject.FindGameObjectsWithTag("cage")) {
				mapManager.removeBrick (MapManager.getMapLocation(brick));
				Destroy (brick);
			}
			Destroy(GameObject.Find ("BossCage"));
		}
	}

	public void onPlayerDied() {

	}

	public void onAllGhostsDied() {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
