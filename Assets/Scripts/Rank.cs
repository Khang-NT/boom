using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Rank : MonoBehaviour {

	public GameObject content;
	public GameObject sampleRow;

	List<GameObject> rows = new List<GameObject>();

	// Use this for initialization
	void Start () {
		if (GamePlay.getInstance ().Histories.Count > 0) {
			var list = GamePlay.getInstance ().Histories;
			print (list.Count);
			sampleRow.SetActive (true);
			sampleRow.GetComponent<PlayerScoreRow> ().display (1, list [0]);
			for (int i = 1; i < list.Count; i++) {
				rows.Add (sampleRow);
				sampleRow = Instantiate (sampleRow, content.transform) as GameObject;
				sampleRow.GetComponent<PlayerScoreRow> ().display (i + 1, list [i]);
			}
			rows.Add (sampleRow);
		} else {
			foreach (GameObject go in rows)
				Destroy (go);
			if (sampleRow != null)
				Destroy (sampleRow);
		}
	}
	
	public void newGame() {
		SceneManager.LoadScene (0);
	}

	public void clearHistory() {
		GamePlay.getInstance ().clearHistories ();
		Start ();
	}
}
