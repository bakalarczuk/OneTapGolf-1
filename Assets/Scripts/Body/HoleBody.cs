//#define DEBUG_RANDOM_POSITION

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class HoleBody : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer rangeSprite = default;

    private Vector2 initialPosition = default;
    private Vector2 spriteExtents = default;

    private void Awake()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        spriteExtents = sprite.bounds.extents;

        initialPosition = transform.position;
        rangeSprite.transform.SetParent(transform.parent, true);

#if DEBUG_RANDOM_POSITION
        rangeSprite.gameObject.SetActive(true);
#else
        rangeSprite.gameObject.SetActive(false);
#endif
    }

    private Collider2D _collider2D = default;

    private Collider2D Collider2D { get => _collider2D ?? (_collider2D = GetComponent<Collider2D>()); }

    private void OnCollisionEnter2D(Collision2D collision) { }

    private void OnCollisionStay2D(Collision2D collision) { }

#if DEBUG_RANDOM_POSITION
    private void Update()
    {
        SetRandomPosition();
    }
#endif

    public bool ContainsBody(TrajectoryBody body)
    {
        //return Collider2D.IsTouching(body.Collider2D) && Collider2D.bounds.Contains(body.transform.position);
        return Collider2D.bounds.Contains(body.transform.position);
    }

    public void SetRandomPosition()
    {
        Vector2 position = transform.position;

        float maxDeltaX = rangeSprite.bounds.extents.x - spriteExtents.x;
        float minDeltaX = maxDeltaX * 0.25f;
        int deltaSign = 2 * Random.Range(0, 2) - 1;
        float deltaX = deltaSign * Random.Range(minDeltaX, maxDeltaX);

        position.x += deltaX;

        if (position.x > initialPosition.x + maxDeltaX || position.x < initialPosition.x - maxDeltaX)
        {
            position.x = initialPosition.x - deltaX;
        }
        transform.position = position;
    }
}
