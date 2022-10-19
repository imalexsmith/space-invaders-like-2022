using UnityEngine;


[RequireComponent(typeof(EdgeCollider2D))]
public class LevelBound : MonoBehaviour
{
    // ====================================================
    public EdgeCollider2D CachedEdgeCollider2D;


    // ====================================================
    protected void Reset()
    {
        CachedEdgeCollider2D = GetComponent<EdgeCollider2D>();
        CachedEdgeCollider2D.isTrigger = true;
    }

    protected void Awake()
    {
        if (!CachedEdgeCollider2D)
            CachedEdgeCollider2D = GetComponent<EdgeCollider2D>();
    }

    protected void Start()
    {
        var cam = Camera.main;

        var p1 = cam.ViewportToWorldPoint(new Vector2(0f, 0f));
        var p2 = cam.ViewportToWorldPoint(new Vector2(0f, 1f));
        var p3 = cam.ViewportToWorldPoint(new Vector2(1f, 1f));
        var p4 = cam.ViewportToWorldPoint(new Vector2(1f, 0f));

        CachedEdgeCollider2D.points = new Vector2[5] {
            new Vector2(p1.x, p1.y),
            new Vector2(p2.x, p2.y),
            new Vector2(p3.x, p3.y),
            new Vector2(p4.x, p4.y),
            new Vector2(p1.x, p1.y)
        };
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<ILevelBoundEnterReaction>(out var target))
            target.OnLevelBoundTriggerEnter(this);
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<ILevelBoundExitReaction>(out var target))
            target.OnLevelBoundTriggerExit(this);
    }
}