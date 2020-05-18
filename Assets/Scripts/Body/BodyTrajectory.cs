using UnityEngine;
using MustHave.Utilities;
using System.Collections;

[ExecuteInEditMode]
public class BodyTrajectory : MonoBehaviour
{
    [SerializeField]
    private Transform bodyTransform = default;
    [SerializeField]
    private Transform markers = default;
    [SerializeField]
    private SpriteRenderer markerPrefab = default;

    [SerializeField, Tooltip("In degrees"), Range(0f, 85f)]
    private float initialVelocitySlopeAngle = 30f;
    [SerializeField, Range(0f, 20f)]
    private float initialSpeed = 8f;

    public const float FLOAT_EPSILON = 0.00001f;
    public const float GRAVITY = 10f;

    private const int MARKERS_COUNT = 75;
    private const float VELOCITY_SLOPE_ANGLE_MAX = 65f;
    private const float VELOCITY_SLOPE_ANGLE_MIN = 30f;

    private Coroutine velocityAngleOscillationRoutine = default;

    private void Awake()
    {
        CreateMarkers();
    }

    private void Start()
    {
        UpdateTrajectory();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying)
        {
            UpdateTrajectory();
        }
    }
#endif

    public void SetInitialVelocityForRandomRange(float minRange, float maxRange)
    {
        initialVelocitySlopeAngle = 45f;
        initialSpeed = GetInitialSpeedForRange(Random.Range(minRange, maxRange));
        UpdateTrajectory();
    }

    public Vector3 GetInitialVelocity()
    {
        float slopeAngle = Mathf.Deg2Rad * initialVelocitySlopeAngle;
        return initialSpeed * new Vector2(Mathf.Cos(slopeAngle), Mathf.Sin(slopeAngle));
    }

    public void StopVelocityAngleOscillationRoutine()
    {
        if (velocityAngleOscillationRoutine != null)
        {
            StopCoroutine(velocityAngleOscillationRoutine);
        }
        velocityAngleOscillationRoutine = null;
    }

    public void StartVelocityAngleOscillationRoutine(float period)
    {
        velocityAngleOscillationRoutine = StartCoroutine(VelocityAngleOscillationRoutine(period));
    }

    public IEnumerator VelocityAngleOscillationRoutine(float period)
    {
        float begAngle = VELOCITY_SLOPE_ANGLE_MIN;
        float endAngle = VELOCITY_SLOPE_ANGLE_MAX;

        while (true)
        {
            yield return CoroutineUtils.UpdateRoutine(period, (elapsedTime, transition) => {
                initialVelocitySlopeAngle = Mathf.Lerp(begAngle, endAngle, transition);
                UpdateTrajectory();
            });
            initialVelocitySlopeAngle = endAngle;

            float tempAngle = begAngle;
            begAngle = endAngle;
            endAngle = tempAngle;
        }
    }

    public void UpdateTrajectory()
    {
        if (!bodyTransform || !markers || markers.childCount == 0)
        {
            return;
        }

        Vector2 v0 = GetInitialVelocity();

        if (Mathf.Abs(v0.x) > FLOAT_EPSILON)
        {
            float tTop = v0.y / GRAVITY;
            float hMax = GRAVITY * tTop * tTop / 2f;
            //float xMlp = v0.y / v0.x;
            float xSquaredMlp = -GRAVITY / (2f * v0.x * v0.x);

            Vector2 pTop = (Vector2)bodyTransform.position + new Vector2(v0.x * tTop, v0.y * tTop - hMax);
            Vector2 p1 = Vector2.zero;
            Vector2 p2 = Vector2.zero;

            float dx = 0.75f;
            int accuracyInterations = 5;
            Vector2 delta = Vector2.zero;

            int markerIndex = 0;

            SetMarkerActiveAtPosition(ref markerIndex, pTop);

            while (p2.y > -hMax && markerIndex < markers.childCount - 2)
            {
                p1 = p2;

                p2.x = p1.x + dx;
                p2.y = p2.x * p2.x * xSquaredMlp;

                for (int i = 0; i < accuracyInterations; i++)
                {
                    delta = (p2 - p1).normalized;

                    p2.x = p1.x + delta.x * dx;
                    p2.y = p2.x * p2.x * xSquaredMlp;
                }

                SetMarkerActiveAtPosition(ref markerIndex, p2 + pTop);
                SetMarkerActiveAtPosition(ref markerIndex, new Vector2(-p2.x, p2.y) + pTop);
            }
            markerIndex = Mathf.Max(0, markerIndex - 2);

            for (int i = markerIndex; i < markers.childCount; i++)
            {
                ResetMarker(i);
            }
        }
    }

    public float IncreaseRangeWithSpeed(float deltaRange)
    {
        float range = GetRange() + deltaRange;
        initialSpeed = GetInitialSpeedForRange(range);
        return initialSpeed > 0f ? range : 0f;
    }

    public float IncreaseRangeWithVelocitySlopeAngle(float deltaRange)
    {
        float range = GetRange();

        if (initialSpeed > FLOAT_EPSILON)
        {
            range += deltaRange;
            float sinDoubleSlopeAngle = range * GRAVITY / (initialSpeed * initialSpeed);
            if (sinDoubleSlopeAngle >= 0f && sinDoubleSlopeAngle <= 1f)
            {
                initialVelocitySlopeAngle = Mathf.Rad2Deg * Mathf.Asin(sinDoubleSlopeAngle) / 2f;
            }
            else
            {
                initialVelocitySlopeAngle = 45f;
            }
        }
        return range;
    }

    private float GetInitialSpeedForRange(float range)
    {
        float slopeAngle = Mathf.Deg2Rad * initialVelocitySlopeAngle;
        Vector2 v_normalized = new Vector2(Mathf.Cos(slopeAngle), Mathf.Sin(slopeAngle));
        float denom = 2f * v_normalized.x * v_normalized.y;

        if (Mathf.Abs(denom) > FLOAT_EPSILON)
        {
            return Mathf.Sqrt(range * GRAVITY / denom);
        }
        return 0f;
    }

    private float GetRange()
    {
        Vector2 v0 = GetInitialVelocity();
        return 2f * v0.x * v0.y / GRAVITY;
    }

    private void CreateMarkers()
    {
        if (markers)
        {
            if (Application.isPlaying)
            {
                markers.DestroyAllChildren();
            }
            else
            {
                markers.DestroyAllChildrenImmediate();
            }

            for (int i = 0; i < MARKERS_COUNT; i++)
            {
                SpriteRenderer marker = Instantiate(markerPrefab, markers);
            }
        }
    }

    private void ResetMarker(int markerIndex)
    {
        Transform marker = markers.GetChild(markerIndex);
        marker.position = bodyTransform.position;
        marker.gameObject.SetActive(false);
    }

    private void SetMarkerActiveAtPosition(ref int markerIndex, Vector2 position, bool incrementMarkerIndex = true)
    {
        Transform marker = markers.GetChild(markerIndex);
        marker.position = position;
        marker.gameObject.SetActive(true);

        if (incrementMarkerIndex)
        {
            markerIndex++;
        }
    }
}
