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

    private Action<int> _onLevelSuccess = default;
    private Action<int, int> _onLevelFail = default;

    private int _score = 0;
    private int _level = 1;

    public int Score => _score;
    public int Level => _level;

    private void Awake()
    {
        _level = PlayerPrefs.GetInt(PLAYERPREFS_LEVEL, _level);
        _score = PlayerPrefs.GetInt(PLAYERPREFS_SCORE, _score);
    }

    public void SetLevelResultCallbacks(Action<int> onSuccess, Action<int, int> onFail)
    {
        _onLevelSuccess = onSuccess;
        _onLevelFail = onFail;
    }

    public void StartLevelResultCheckRoutine(Action onEnd)
    {
        this.StartCoroutineActionAfterTime(() => {
            if (hole.ContainsBody(ball))
            {
                PlayerPrefs.SetInt(PLAYERPREFS_SCORE, ++_score);
                PlayerPrefs.SetInt(PLAYERPREFS_SCORE_BEST, Mathf.Max(PlayerPrefs.GetInt(PLAYERPREFS_SCORE_BEST), _score));

                _onLevelSuccess?.Invoke(_score);
            }
            else
            {
                _onLevelFail?.Invoke(_score, PlayerPrefs.GetInt(PLAYERPREFS_SCORE_BEST));
            }
            onEnd?.Invoke();
        }, 2f);
    }

    public void ResetGame()
    {
        PlayerPrefs.SetInt(PLAYERPREFS_LEVEL, _level = 1);
        PlayerPrefs.SetInt(PLAYERPREFS_SCORE, _score = 0);

        InitLevel();
    }

    public void StartNextLevel()
    {
        PlayerPrefs.SetInt(PLAYERPREFS_LEVEL, ++_level);

        InitLevel();
    }

    private void InitLevel()
    {
        ball.ResetRigidBody();
        ballTrajectory.SetRandomInitialVelocity();
        hole.SetRandomPosition();
    }
}
