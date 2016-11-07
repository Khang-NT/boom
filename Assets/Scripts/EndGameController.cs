using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameController : MonoBehaviour {



	void Start () {
		GamePlay gamePlay = GamePlay.getInstance ();
		GameObject.Find("GameResult").GetComponent<Text>().text = gamePlay.Win ? "Win!!!" : "GameOver";
		GameObject.Find ("Score").GetComponent<Text> ().text = gamePlay.Score.ToString ();
		GameObject.Find ("PlayerName").GetComponent<Text> ().text = gamePlay.PlayerName;
		GameObject.Find ("HighScore").SetActive (gamePlay.Score == gamePlay.HighScore);
	}
	
	public void newGame() {
		SceneManager.LoadScene (0);
	}
}
