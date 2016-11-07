using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
	public Text heart;
	public Text score;
	// Use this for initialization

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		heart.text = GamePlay.getInstance ().Heart + "";
		score.text = GamePlay.getInstance ().Score + "";
	}
}
