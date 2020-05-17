using MustHave.UI;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : UIScript
{
    [SerializeField]
    private Text scoreText = default;
    [SerializeField]
    private Text scoreBestText = default;

    public void SetScoreTexts(int score, int bestScore)
    {
        scoreText.text = "SCORE: " + score;
        scoreBestText.text = "BEST: " + bestScore;
    }
}
