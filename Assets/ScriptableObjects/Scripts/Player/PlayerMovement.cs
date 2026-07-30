using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    [Header("Movement")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float nextWaypointDistance = 0.2f;

    private Rigidbody2D rb;
    private Seeker seeker;

    private Path path;
    private int currentWaypoint;
    private bool moving;

    public bool canMove = true;

    private Vector3 targetPos;
    public Vector3 TargetPos => targetPos;

    private void Awake()
    {
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();

        targetPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (!moving || path == null)
            return;

        // Bereits erreichte beziehungsweise sehr nahe Wegpunkte überspringen
        while (
            currentWaypoint < path.vectorPath.Count &&
            Vector2.Distance(
                rb.position,
                path.vectorPath[currentWaypoint]
            ) <= nextWaypointDistance
        )
        {
            currentWaypoint++;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            StopMoving();
            return;
        }

        Vector2 waypoint = path.vectorPath[currentWaypoint];
        Vector2 direction = waypoint - rb.position;

        if (direction.sqrMagnitude <= 0.0001f)
        {
            currentWaypoint++;
            return;
        }

        float maxDistance = speed * Time.fixedDeltaTime;

        Vector2 newPosition =
            rb.position +
            direction.normalized *
            Mathf.Min(maxDistance, direction.magnitude);

        rb.MovePosition(newPosition);
    }

    public bool MoveTo(Vector3 worldPos)
    {
        if (!canMove)
            return false;

        return StartPathToNearestWalkable(worldPos);
    }

    public void ForceMoveTo(Vector3 worldPos)
    {
        StartPathToNearestWalkable(worldPos);
    }

    private bool StartPathToNearestWalkable(Vector3 worldPos)
    {
        if (AstarPath.active == null)
            return false;

        NNConstraint constraint = new NNConstraint
        {
            constrainWalkability = true,
            walkable = true
        };

        NNInfo nearest = AstarPath.active.GetNearest(worldPos, constraint);

        if (nearest.node == null || !nearest.node.Walkable)
            return false;

        Vector3 nearestPosition = nearest.position;

        targetPos = new Vector3(
            nearestPosition.x,
            nearestPosition.y,
            transform.position.z
        );

        moving = false;
        path = null;

        seeker.StartPath(rb.position, targetPos, OnPathComplete);
        return true;
    }

    public bool IsWalkable(Vector2 worldPos)
    {
        if (AstarPath.active == null)
            return false;

        NNConstraint constraint = new NNConstraint
        {
            constrainWalkability = true,
            walkable = true
        };

        NNInfo nearest = AstarPath.active.GetNearest(worldPos, constraint);

        return nearest.node != null && nearest.node.Walkable;
    }

    private void OnPathComplete(Path newPath)
    {
        if (
            newPath.error ||
            newPath.vectorPath == null ||
            newPath.vectorPath.Count == 0
        )
        {
            moving = false;
            path = null;
            return;
        }

        path = newPath;
        currentWaypoint = 0;

        // Startpunkt sofort überspringen, falls er direkt beim Player liegt
        while (
            currentWaypoint < path.vectorPath.Count &&
            Vector2.Distance(
                rb.position,
                path.vectorPath[currentWaypoint]
            ) <= nextWaypointDistance
        )
        {
            currentWaypoint++;
        }

        moving = currentWaypoint < path.vectorPath.Count;

        if (!moving)
            targetPos = transform.position;
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