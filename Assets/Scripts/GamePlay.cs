using UnityEngine;
using System.Collections;
using System.IO;

public class GamePlay {
    private static GamePlay sInstance;
	private static readonly string FILE_NAME = "gameplay.data";


    public static GamePlay getInstance()
    {
        if (sInstance == null)
        {
            sInstance = new GamePlay();
        }
        return sInstance;
    }


	int score;
    int heart;
    int boardId;
    string playerName;
	int highScore;
	string highScorePlayerName;
	bool win;

    protected GamePlay()
    {
		loadDataFromFile ();
		reset ();
    }

	public string PlayerName {
		get {
			return playerName;
		}

		set {
			this.playerName = value;
		}
	}

	public int Heart {
		get {
			return heart;
		}

		set {
			this.heart = value;
		}
	}

	public int BoardId {
		get {
			return boardId;
		}

		set {
			this.boardId = value;
		}
	}

	public int Score {
		get {
			return score;
		}

		set {
			this.score = value;
			if (score >= highScore) {
				this.highScore = score;
				this.highScorePlayerName = playerName;
				saveDataToFile ();
			}
		}
	}

	public bool Win {
		get {
			return win;
		}

		set {
			win = value;
		}
	}

	public void reset() {
		boardId = 1;
		score = 0;
		heart = 3;
	}

	protected void loadDataFromFile() {
		highScore = 0;
		highScorePlayerName = "";
		if (File.Exists (FILE_NAME)) {
			string[] lines = File.ReadAllLines (FILE_NAME);
			if (lines.Length == 2) {
				highScore = int.Parse (lines [0]);
				highScorePlayerName = lines [1];
			}
		}
	}

	protected void saveDataToFile() {
		File.WriteAllLines (FILE_NAME, new string[]{ highScore.ToString (), highScorePlayerName });
	}
}
