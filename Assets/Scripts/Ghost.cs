using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour, MapManagerListener {
    public MapManager mapManager;

    public int smartness = 1; 
    public float speed = 1f; 
    
	// Use this for initialization
	void Start () {
        mapManager.addListener (this);
    }

	public void onMapReady() {
        GameObject player = mapManager.getPlayer();
       // mapManager chi duoc su dung sau khi onMapReady, su dung trong ham Start coi chung bug
	}
	// Update is called once per frame
	void Update () {
        
    }
}
