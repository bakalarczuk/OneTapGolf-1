//#define DEBUG_RANDOM_VEOCITY

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
    private const float RANDOM_VELOCITY_SLOPE_ANGLE_MAX = 65f;
    private const float RANDOM_VELOCITY_SLOPE_ANGLE_MIN = 35f;
    private const float RANDOM_SPEED_MIN = 8f;
    private const float RANDOM_SPEED_MAX = 10f;

    private void Awake()
    {
        Physics2D.gravity = new Vector2(0f, -GRAVITY);
        Physics.gravity = Physics2D.gravity;

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
#if DEBUG_RANDOM_VEOCITY
        else
        {
            SetRandomInitialVelocity();
        }
#endif
    }
#endif

    public void SetRandomInitialVelocity()
    {
        initialSpeed = Random.Range(RANDOM_SPEED_MIN, RANDOM_SPEED_MAX);
        initialVelocitySlopeAngle = Random.Range(RANDOM_VELOCITY_SLOPE_ANGLE_MIN, RANDOM_VELOCITY_SLOPE_ANGLE_MAX);

        UpdateTrajectory();
    }

    public Vector3 GetInitialVelocity()
    {
        float slopeAngle = Mathf.Deg2Rad * initialVelocitySlopeAngle;
        return initialSpeed * new Vector2(Mathf.Cos(slopeAngle), Mathf.Sin(slopeAngle));
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
        float slopeAngle = Mathf.Deg2Rad * initialVelocitySlopeAngle;
        Vector2 v_normalized = new Vector2(Mathf.Cos(slopeAngle), Mathf.Sin(slopeAngle));
        float denom = 2f * v_normalized.x * v_normalized.y;

        float range = GetRange();

        if (denom > FLOAT_EPSILON)
        {
            range += deltaRange;
            float speed = Mathf.Sqrt(range * GRAVITY / denom);
            initialSpeed = speed;
        }
        return range;
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
