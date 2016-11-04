using UnityEngine;
using System.Collections;

public class Flame : MonoBehaviour {

	public float flameDuration = 0.8f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		flameDuration -= Time.deltaTime;
		if (flameDuration <= 0)
			Destroy (this.gameObject);
	}
}
