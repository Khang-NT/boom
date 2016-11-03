using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour, MapManagerListener {
    public MapManager mapManager;

	// Use this for initialization
	void Start () {
<<<<<<< HEAD
        Debug.Log(mapManager[0, 0]); 
    }
   
=======
		mapManager.addListener (this);
    }

	public void onMapReady() {
		        for (int i = 0; i < 10; i++)
		            for (int j = 0; j < 10; j++)
		                Debug.Log(mapManager[i, j]);
	}
	
>>>>>>> 133fcd598116f7853022b2643671035df123051d
	// Update is called once per frame
	void Update () {
        
    }
}
