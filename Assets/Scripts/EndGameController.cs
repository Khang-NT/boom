using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameController : MonoBehaviour {



	void Start () {
		GamePlay gamePlay = GamePlay.getInstance ();
		GameObject.Find("GameResult").GetComponent<Text>().text = gamePlay.Win ? "Win!!!" : "GameOver";
		GameObject.Find ("Score").GetComponent<Text> ().text = gamePlay.Score.ToString ();
		bool newHighScore = gamePlay.Score > gamePlay.HighScore;
		GameObject.Find ("HighScore").SetActive (newHighScore);
		if (newHighScore)
			GameObject.Find ("PlayerName").GetComponent<Text> ().text = gamePlay.PlayerName;
		gamePlay.saveScoreAndRenew ();
	}
	
	public void newGame() {
		SceneManager.LoadScene (6);
	}
}
