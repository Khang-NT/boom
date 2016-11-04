using UnityEngine;
using System.Collections;

public class Boom : MonoBehaviour {

	public MapManager mapManager;

	public float time = 2;	// 2 seconds

	private BoxCollider2D boxCollider;

	// Use this for initialization
	void Start () {
		mapManager = MapManager.getInstance ();
		boxCollider = GetComponent<BoxCollider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!boxCollider.enabled) {
			float distance = Vector3.Distance (mapManager.getPlayer ().transform.position, transform.position);
			if (distance >= mapManager.getCellSize () * 2 / 3)
				boxCollider.enabled = true;
		}

//		time -= Time.deltaTime;
//		if (time <= 0)
//			Destroy (gameObject);
	}
}
