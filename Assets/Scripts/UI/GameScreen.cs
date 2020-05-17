using UnityEngine;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager = default;
    [SerializeField]
    private SuccessPanel successPanel = default;
    [SerializeField]
    private GameOverPanel gameOverPanel = default;
    [SerializeField]
    private Button nextLevelButton = default;
    [SerializeField]
    private Button resetGameButton = default;
    [SerializeField]
    private Text scoreText = default;

    public bool IsResultPanelVisible => successPanel.gameObject.activeSelf || gameOverPanel.gameObject.activeSelf;

    private void Awake()
    {
        gameManager.SetLevelResultCallbacks(score => {
            SetScoreText(score);
            successPanel.Show();
        }, (score, bestScore) => {
            SetScoreText(score);
            gameOverPanel.SetScoreTexts(score, bestScore);
            gameOverPanel.Show();
        });

        nextLevelButton.onClick.AddListener(() => {
            successPanel.Hide();
            gameManager.StartNextLevel();
        });

        resetGameButton.onClick.AddListener(() => {
            gameOverPanel.Hide();
            gameManager.ResetGame();
            SetScoreText(gameManager.Score);
        });

        successPanel.Hide();
        gameOverPanel.Hide();
    }

    private void Start()
    {
        SetScoreText(gameManager.Score);
    }

    public void SetAnchors(Camera camera, Vector2 viewSize)
    {
        Vector2 cameraSize = new Vector2(2f * camera.orthographicSize * camera.aspect, 2f * camera.orthographicSize);
        Vector2 normalizedViewSize = viewSize / cameraSize;
        RectTransform rectTransform = transform as RectTransform;
        Vector2 anchorOffset = (Vector2.one - normalizedViewSize) / 2f;
        rectTransform.anchorMin = anchorOffset;
        rectTransform.anchorMax = Vector2.one - anchorOffset;
    }

    private void SetScoreText(int points)
    {
        scoreText.text = "Score: " + points;
    }

}
