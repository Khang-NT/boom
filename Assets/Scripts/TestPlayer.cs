using UnityEngine;
using System.Collections;

public class TestPlayer : MonoBehaviour {
	public float movingSpeed = 1.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		var move = new Vector3(Mathf.Clamp(
			Input.GetAxis("Horizontal") * movingSpeed * Time.deltaTime + transform.position.x,  -15.5f, 15.5f), 
			Mathf.Clamp(Input.GetAxis("Vertical") * movingSpeed * Time.deltaTime + transform.position.y, -8.45f, 8.45f), 0);
		 
		transform.position = move;
	}
}
