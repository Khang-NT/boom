using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ghost : MonoBehaviour, MapManagerListener {
    public MapManager mapManager;

	// Use this for initialization
	void Start () {
        mapManager.addListener (this);
    }

	public void onMapReady() {
    }

    public void onMapChanged()
    {

    }

	// Update is called once per frame
	void Update () {

    }
}
