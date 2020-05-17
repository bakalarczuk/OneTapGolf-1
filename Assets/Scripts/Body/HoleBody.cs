//#define DEBUG_RANDOM_POSITION

using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class HoleBody : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer rangeSprite = default;
    [SerializeField]
    private Tilemap tilemap = default;
    [SerializeField]
    private Tile groundTileWithCollider = default;
    [SerializeField]
    private Tile groundTileWithoutCollider = default;


    private new Collider2D collider = default;

    private Vector2 initialPosition = default;
    private Vector2 spriteExtents = default;

    private void Awake()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        spriteExtents = sprite.bounds.extents;

        collider = GetComponent<Collider2D>();

        initialPosition = transform.position;
        rangeSprite.transform.SetParent(transform.parent, true);

#if DEBUG_RANDOM_POSITION
        rangeSprite.gameObject.SetActive(true);
#else
        rangeSprite.gameObject.SetActive(false);
#endif
    }

    private void Start()
    {
        SetUnderneathTilesCollidersActive(false);
    }

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
        return collider.bounds.Contains(body.transform.position);
    }

    private void SetUnderneathTilesCollidersActive(bool active)
    {
        Vector3Int holeCell = tilemap.WorldToCell(transform.position);
        Vector3Int cell = holeCell;

        Tile groundTile = active ? groundTileWithCollider : groundTileWithoutCollider;

        for (int i = -1; i < 2; i++)
        {
            cell.x = i + holeCell.x;
            tilemap.SetTile(cell, groundTile);
        }
    }

    public void SetRandomPosition()
    {
        SetUnderneathTilesCollidersActive(true);

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

        SetUnderneathTilesCollidersActive(false);
    }
}
