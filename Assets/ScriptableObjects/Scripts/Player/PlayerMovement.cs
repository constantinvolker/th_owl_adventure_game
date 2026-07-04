using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]

///seeker is part of Pathfinding package
///seeker is used to calculate the path within the walk area
[RequireComponent(typeof(Seeker))]

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    [Header("Movement")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float nextWaypointDistance = 0.1f;

    private Rigidbody2D rb;
    private Seeker seeker;

    private Path path;
    private int currentWaypoint;
    private bool moving;

    public bool canMove = true;

    private Vector3 targetPos;

    public Vector3 TargetPos => targetPos;

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();

        targetPos = transform.position;
    }

    void FixedUpdate()
    {
        if (!moving || path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            StopMoving();
            return;
        }

        Vector2 waypoint = path.vectorPath[currentWaypoint];

        Vector2 newPos = Vector2.MoveTowards(
            rb.position,
            waypoint,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPos);

        if (Vector2.Distance(rb.position, waypoint) <= nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    public bool MoveTo(Vector3 worldPos)
    {
        if (!canMove)
            return false;

        if (!IsWalkable(worldPos))
            return false;

        targetPos = new Vector3(worldPos.x, worldPos.y, transform.position.z);

        seeker.StartPath(rb.position, targetPos, OnPathComplete);

        return true;
    }

    public bool IsWalkable(Vector2 worldPos)
    {
        if (AstarPath.active == null)
            return false;

        var node = AstarPath.active.GetNearest(worldPos).node;
        return node != null && node.Walkable;
    }

    public void ForceMoveTo(Vector3 worldPos)
    {
        targetPos = new Vector3(worldPos.x, worldPos.y, transform.position.z);

        seeker.StartPath(rb.position, targetPos, OnPathComplete);
    }

    private void OnPathComplete(Path p)
    {
        if (p.error)
        {
            moving = false;
            path = null;
            return;
        }

        path = p;
        currentWaypoint = 0;
        moving = true;
    }

    public bool IsMoving()
    {
        return moving;
    }

    public void StopMoving()
    {
        moving = false;
        path = null;
        targetPos = transform.position;
    }
}