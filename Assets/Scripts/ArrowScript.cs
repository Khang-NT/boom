using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour, MapManagerListener {
	public MapManager mapManager;
	// Use this for initialization
	void Start () {
		mapManager.addListener (this);
	}

	public void onMapReady() {

	}

	public void onMapChanged() {

	}

	public void onPlayerDied() {

	}

	public void onAllGhostsDied() {
		GetComponent<SpriteRenderer> ().enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
