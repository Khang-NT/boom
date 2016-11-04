using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour, MapManagerListener {
    public MapManager mapManager;

    public int smartness;
    public float speedIndex;
     
    private float speed; 
    
	// Use this for initialization
	void Start () {
        mapManager.addListener (this);
    }

	public void onMapReady() {
        GameObject player = mapManager.getPlayer();
        
	}

	// Update is called once per frame
	void Update () {
        
    }
}
