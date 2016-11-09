using UnityEngine;
using System.Collections;

public class TestTransform : MonoBehaviour {
	public MapManager test;
	// Use this for initialization
	void Start () {
		Debug.Log (transform.position);
		Debug.Log("Local pos: " + transform.localPosition.ToString());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
