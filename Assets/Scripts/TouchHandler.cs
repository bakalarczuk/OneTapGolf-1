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
            updateTrajectoryRoutine = StartCoroutine(UpdateBallTrajectoryRoutine());
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

    private IEnumerator UpdateBallTrajectoryRoutine()
    {
        float range = 0f;
        float maxRange = GetBallRangeMax();
        float deltaRangeFactor = 2f + Mathf.Min(10f, gameManager.Level * 0.15f);
        float deltaRange = Time.unscaledDeltaTime * deltaRangeFactor;

        while (range <= maxRange)
        {
            range = ballTrajectory.IncreaseRangeWithSpeed(deltaRange);
            ballTrajectory.UpdateTrajectory();
            yield return null;
        }
        Shoot();

        updateTrajectoryRoutine = null;
    }
}
