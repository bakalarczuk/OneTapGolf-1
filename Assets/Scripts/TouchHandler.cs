using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private TrajectoryBody ball = default;
    [SerializeField]
    private BodyTrajectory ballTrajectory = default;

    private GameManager gameManager = default;
    private GameScreen gameScreen = default;

    private Coroutine _updateTrajectoryRoutine = default;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        gameScreen = GetComponent<GameScreen>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_updateTrajectoryRoutine == null)
        {
            _updateTrajectoryRoutine = StartCoroutine(UpdateTrajectoryRoutine());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_updateTrajectoryRoutine != null)
        {
            StopCoroutine(_updateTrajectoryRoutine);
            _updateTrajectoryRoutine = null;
        }
        Shoot();
    }


    private void Shoot()
    {
        if (!ball.IsUpdatingPosition)
        {
            EventSystem currentEventSystem = EventSystem.current;
            currentEventSystem.enabled = false;

            ball.StartPositionUpdateRoutine(() => {
                gameManager.StartLevelResultCheckRoutine(() => {
                    currentEventSystem.enabled = true;
                });
            });
        }
    }

    private float GetMaxRange()
    {
        Camera camera = Camera.main;
        float cameraRightEdgeX = camera.transform.position.x + camera.orthographicSize * camera.aspect;
        return cameraRightEdgeX - ball.transform.position.x;
    }

    private IEnumerator UpdateTrajectoryRoutine()
    {
        float range = 0f;
        float maxRange = GetMaxRange();
        float deltaRangeFactor = 1.5f + Mathf.Min(10f, gameManager.Level * 0.25f);
        float deltaRange = Time.fixedDeltaTime * deltaRangeFactor;

        while (range <= maxRange)
        {
            range = ballTrajectory.IncreaseRangeWithSpeed(deltaRange);
            ballTrajectory.UpdateTrajectory();
            yield return null;
        }
        Shoot();

        _updateTrajectoryRoutine = null;
    }
}
