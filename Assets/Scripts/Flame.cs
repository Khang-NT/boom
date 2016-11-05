using UnityEngine;
using System.Collections;

public class Flame : MonoBehaviour {

	public float flameDuration = 0.8f;

	// Use this for initialization
	void Start () {
		MapManager.getInstance ().registerFlame (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		flameDuration -= Time.deltaTime;
		if (flameDuration <= 0) {
			MapManager.getInstance ().removeFlame (this.gameObject);
			Destroy (this.gameObject);
		}
	}
}
