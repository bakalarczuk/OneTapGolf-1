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

    private Coroutine updateTrajectoryRoutine = default;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        gameScreen = GetComponent<GameScreen>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ballTrajectory.StopVelocityAngleOscillationRoutine();

        if (updateTrajectoryRoutine == null)
        {
            updateTrajectoryRoutine = StartCoroutine(IncreaseBallTrajectoryRangeRoutine());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (updateTrajectoryRoutine != null)
        {
            StopCoroutine(updateTrajectoryRoutine);
            updateTrajectoryRoutine = null;
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

    private float GetBallRangeMax()
    {
        Camera camera = Camera.main;
        float cameraRightEdgeX = camera.transform.position.x + camera.orthographicSize * camera.aspect;
        return cameraRightEdgeX - ball.transform.position.x;
    }

    private IEnumerator IncreaseBallTrajectoryRangeRoutine()
    {
        float maxRange = GetBallRangeMax();
        float deltaRangeFactor = 2f + Mathf.Min(10f, gameManager.Level * 0.15f);
        float begRange = ballTrajectory.GetRange();
        float range = begRange;
        float startTime = Time.unscaledTime;

        while (range <= maxRange)
        {
            range = begRange + (Time.unscaledTime - startTime) * deltaRangeFactor;
            ballTrajectory.SetInitialSpeedForRange(range);
            ballTrajectory.UpdateTrajectory();
            yield return null;
        }
        Shoot();

        updateTrajectoryRoutine = null;
    }
}
