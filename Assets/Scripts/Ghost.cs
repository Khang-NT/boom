using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour {
    public MapManager mapManager;

	// Use this for initialization
	void Start () {
        Debug.Log(mapManager[0, 0]); 
    }
   
	// Update is called once per frame
	void Update () {
        
    }
}
