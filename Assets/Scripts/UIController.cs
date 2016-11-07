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
		var gamePlay = GamePlay.getInstance ();
		heart.text = gamePlay.Heart + "";
		score.text = gamePlay.Score + "";
		if (gamePlay.Score > 0 && gamePlay.Score == gamePlay.HighScore)
			score.color = Color.green;
	}
}
