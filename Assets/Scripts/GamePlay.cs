using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class PlayerScore {
	public int score;
	public int round;
	public string playerName;

	public PlayerScore() {
		reset ();
	}

	public PlayerScore(int score, int round, string playerName) {
		this.score = score;
		this.round = round;
		this.playerName = playerName;
	}

	public void reset() {
		this.score = 0;
		this.round = 1;
	}

	public override string ToString ()
	{
		return score + "\n" + round + "\n" + playerName;
	}
}

public class GamePlay {
	public static readonly int MAX_ROUND = 4;

    private static GamePlay sInstance;
	private static readonly string HIGH_SCORE_FILE = "high_score.data";
	private static readonly string PLAYER_SETTINGS_FILE = "player_settings.data";


    public static GamePlay getInstance()
    {
        if (sInstance == null)
        {
            sInstance = new GamePlay();
        }
        return sInstance;
    }


	PlayerScore currentScore = new PlayerScore();

    int heart;
    int boardId;
	bool win;

	List<PlayerScore> histories;

	// player settings
	bool soundEnabled;

    protected GamePlay()
    {
		histories = new List<PlayerScore> ();
		loadHighScoreFromFile ();
		loadPlayerSettingsFromFile ();
		reset ();
    }

	public string PlayerName {
		get {
			return currentScore.playerName;
		}

		set {
			this.currentScore.playerName = value;
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

	public int Round {
		get {
			return currentScore.round;
		}

		set {
			currentScore.round = value;
		}
	}

	public int Score {
		get {
			return currentScore.score;
		}

		set {
			this.currentScore.score = value;
//			if (score >= highScore) {
//				this.highScore = score;
//				this.highScorePlayerName = playerName;
//				saveHighScoreToFile ();
//			}
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

	public int HighScore {
		get {
			return histories.Count == 0 ? 0 : histories[0].score;
		}
	}

	public bool IsSoundEnabled {
		get {
			return soundEnabled;
		}

		set {
			soundEnabled = value;
			savePlayerSettingsToFile ();
		}
	}

	public List<PlayerScore> Histories {
		get {
			return histories;
		}
	}

	public void clearHistories() {
		this.histories.Clear ();
		saveHighScoreToFile ();
	}

	public void saveScoreAndRenew() {
		if (histories.Count == 0) {
			histories.Add (currentScore);
		} else
			for (int i = 0; i < histories.Count; i++)
				if (histories [i].score <= currentScore.score) {
					histories.Add (currentScore);
					break;
				}
		saveHighScoreToFile ();
		// new player score instance
		currentScore = new PlayerScore ();
	}

	public void reset() {
		currentScore.reset ();
		boardId = 1;
		heart = 3;
	}

	void loadHighScoreFromFile() {
		histories.Clear ();
		if (File.Exists (HIGH_SCORE_FILE)) {
			string[] lines = File.ReadAllLines (HIGH_SCORE_FILE);
			if (lines.Length %  3 == 0) {
				for (int i = 0; i < lines.Length / 3; i++)
					histories.Add (new PlayerScore (int.Parse (lines [i * 3]), int.Parse (lines [i * 3 + 1]), lines [i * 3 + 2]));
			}
		}
	}

	void saveHighScoreToFile() {
		if (histories.Count > 0) {
			string[] data = new string[histories.Count];
			for (int i = 0; i < histories.Count; i++)
				data [i] = histories [i].ToString ();
			File.WriteAllLines (HIGH_SCORE_FILE, data);
		} else
			File.Delete (HIGH_SCORE_FILE);
	}

	void loadPlayerSettingsFromFile() {
		soundEnabled = true;
		if (File.Exists (PLAYER_SETTINGS_FILE)) {
			string[] lines = File.ReadAllLines (PLAYER_SETTINGS_FILE);
			if (lines.Length == 1) {
				soundEnabled = bool.Parse (lines [0]);
			}
		}
	}

	void savePlayerSettingsToFile() {
		File.WriteAllLines (PLAYER_SETTINGS_FILE, new string[]{ soundEnabled.ToString() });
	}
}
