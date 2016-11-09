using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScoreRow : MonoBehaviour {

	public Text no, name, round, score;

	public void display(int number, PlayerScore playerScore) {
		no.text = number.ToString ();
		name.text = playerScore.playerName;
		round.text = playerScore.round.ToString ();
		score.text = playerScore.score.ToString ();
	}
}
