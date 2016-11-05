using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ghost : MonoBehaviour, MapManagerListener {
    public MapManager mapManager;

    // ghost attribute
    public float heart;
    public float speed;

    // moving and pathFinding variables
    private List<GameObject> booms;
    private List<GameObject> flames;
    private List<GameObject> obstructions;

    // Use this for initialization
    void Start () {
        mapManager.addListener (this);
    }

	public void onMapReady() {
        Debug.Log("Map change");
    }

    public void onMapChanged()
    {
        
    }

	// Update is called once per frame
	void Update () {
       
    }
}
