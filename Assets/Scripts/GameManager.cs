using UnityEngine;
using MustHave.Utilities;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private HoleBody hole = default;
    [SerializeField]
    private TrajectoryBody ball = default;
    [SerializeField]
    private BodyTrajectory ballTrajectory = default;

    private const string PLAYERPREFS_LEVEL = "PLAYERPREFS_LEVEL";
    private const string PLAYERPREFS_SCORE = "PLAYERPREFS_SCORE";
    private const string PLAYERPREFS_SCORE_BEST = "PLAYERPREFS_SCORE_BEST";

    private Action<int> onLevelSuccess = default;
    private Action<int, int> onLevelFail = default;

    private int score = 0;
    private int level = 1;

    public int Score => score;
    public int Level => level;

    private void Awake()
    {
        level = PlayerPrefs.GetInt(PLAYERPREFS_LEVEL, level);
        score = PlayerPrefs.GetInt(PLAYERPREFS_SCORE, score);
    }

    public void SetLevelResultCallbacks(Action<int> onSuccess, Action<int, int> onFail)
    {
        onLevelSuccess = onSuccess;
        onLevelFail = onFail;
    }

    public void StartLevelResultCheckRoutine(Action onEnd)
    {
        this.StartCoroutineActionAfterTime(() => {
            if (hole.ContainsBody(ball))
            {
                PlayerPrefs.SetInt(PLAYERPREFS_SCORE, ++score);
                PlayerPrefs.SetInt(PLAYERPREFS_SCORE_BEST, Mathf.Max(PlayerPrefs.GetInt(PLAYERPREFS_SCORE_BEST), score));

                onLevelSuccess?.Invoke(score);
            }
            else
            {
                onLevelFail?.Invoke(score, PlayerPrefs.GetInt(PLAYERPREFS_SCORE_BEST));
            }
            onEnd?.Invoke();
        }, 2f);
    }

    public void ResetGame()
    {
        PlayerPrefs.SetInt(PLAYERPREFS_LEVEL, level = 1);
        PlayerPrefs.SetInt(PLAYERPREFS_SCORE, score = 0);

        InitLevel();
    }

    public void StartNextLevel()
    {
        PlayerPrefs.SetInt(PLAYERPREFS_LEVEL, ++level);

        InitLevel();
    }

    private void InitLevel()
    {
        ball.ResetRigidBody();
        ballTrajectory.SetRandomInitialVelocity();
        hole.SetRandomPosition();
    }
}
