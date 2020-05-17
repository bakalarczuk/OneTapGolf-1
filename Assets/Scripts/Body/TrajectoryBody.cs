using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class TrajectoryBody : MonoBehaviour
{
    [SerializeField]
    private BodyTrajectory trajectory = default;

    private new Rigidbody2D rigidbody = default;

    private Vector2 initialPosition = default;
    private Vector2 initialVelocity = default;

    private Coroutine updatePositionRoutine = default;

    public bool IsUpdatingPosition => updatePositionRoutine != null;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void ResetBody()
    {
        rigidbody.simulated = true;
        rigidbody.velocity = Vector2.zero;

        transform.position = initialPosition;

        trajectory.transform.SetParent(transform, true);
    }

    private Vector2 GetUpdatedPosition(float deltaTime)
    {
        Vector2 v0 = initialVelocity;
        if (Mathf.Abs(v0.x) > BodyTrajectory.FLOAT_EPSILON)
        {
            float gravity = Physics2D.gravity.y;
            float x0 = initialPosition.x;
            float y0 = initialPosition.y;
            float x = transform.position.x + v0.x * deltaTime;
            float x_shifted = x - x0;
            float y = y0 + x_shifted * v0.y / v0.x + gravity * x_shifted * x_shifted / (2f * v0.x * v0.x);
            return new Vector2(x, y);
        }
        return transform.position;
    }

    private Vector2 GetVelocity()
    {
        float gravity = Physics2D.gravity.y;
        float t = (transform.position.x - initialPosition.x) / initialVelocity.x;
        return new Vector2(initialVelocity.x, initialVelocity.y + gravity * t);
    }

    private IEnumerator UpdatePositionRoutine(Action onEnd)
    {
        trajectory.transform.SetParent(transform.parent, true);

        rigidbody.simulated = false;

        initialVelocity = trajectory.GetInitialVelocity();
        initialPosition = transform.position;

        Vector2 updatedPosition;
        while ((updatedPosition = GetUpdatedPosition(Time.deltaTime)).y >= initialPosition.y)
        {
            transform.position = updatedPosition;
            yield return null;
        }

        rigidbody.simulated = true;
        rigidbody.velocity = GetVelocity();

        updatePositionRoutine = null;

        onEnd?.Invoke();
    }

    public void StartPositionUpdateRoutine(Action onEnd)
    {
        if (updatePositionRoutine != null)
        {
            StopCoroutine(updatePositionRoutine);
        }
        updatePositionRoutine = StartCoroutine(UpdatePositionRoutine(onEnd));
    }
}
