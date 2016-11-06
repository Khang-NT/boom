using UnityEngine;
using System.Collections;

public class GamePlay {
    private static GamePlay sInstance;

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
	int highScorePlayerName;

    protected GamePlay()
    {
        // load high score from file
        boardId = 1;
        score = 0;
        heart = 3;
        playerName = "bla";
    }
}
