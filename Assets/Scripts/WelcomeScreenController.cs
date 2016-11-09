using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WelcomeScreenController : MonoBehaviour {

	public void startGame() {
		string heroName = GameObject.Find ("HeroName").GetComponent<Text>().text;
		if (heroName != null && heroName.Length > 0) {
			GamePlay.getInstance ().reset ();
			GamePlay.getInstance ().PlayerName = heroName;
			SceneManager.LoadScene (1); 
		}
	}

	public void showHighScores() {
		SceneManager.LoadScene (6);
	}
}
