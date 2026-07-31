using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

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

    //for stairs
    private bool followingFixedPath;
    private readonly List<Vector2> fixedPath = new();
    private int fixedPathIndex;
    private StairPath pendingStair;
    private bool pendingStairUpwards;

    private void Awake()
    {
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();

        targetPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (followingFixedPath)
        {
            FollowFixedPath();
            return;
        }

        if (!moving || path == null)
            return;

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
            OnNormalPathFinished();
            return;
        }

        Vector2 waypoint = path.vectorPath[currentWaypoint];
        MoveTowards(waypoint);
    }

    public bool UseStair(StairPath stair)
    {
        if (!canMove || stair == null)
            return false;

        bool moveUpwards = stair.IsCloserToBottom(rb.position);

        Transform entry = moveUpwards
            ? stair.BottomEntry
            : stair.TopEntry;

        pendingStair = stair;
        pendingStairUpwards = moveUpwards;

        bool pathStarted = StartPathToNearestWalkable(entry.position);

        if (!pathStarted)
        {
            pendingStair = null;
            return false;
        }

        return true;
    }

    private void MoveTowards(Vector2 target)
    {
        Vector2 direction = target - rb.position;

        if (direction.sqrMagnitude <= 0.0001f)
            return;

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

        pendingStair = null;

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
            pendingStair = null;
            return;
        }

        path = newPath;
        currentWaypoint = 0;

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

        // Falls der Player bereits direkt am Eingang steht,
        // muss die Treppe trotzdem gestartet werden.
        if (!moving)
        {
            OnNormalPathFinished();
        }
    }

    public bool IsMoving()
    {
        return moving;
    }

    private void OnNormalPathFinished()
    {
        moving = false;
        path = null;

        if (pendingStair != null)
        {
            StartFixedStairPath(
                pendingStair,
                pendingStairUpwards
            );

            pendingStair = null;
            return;
        }

        targetPos = transform.position;
    }

    public void StopMoving()
    {
        moving = false;
        path = null;
        targetPos = transform.position;
    }

    //Stairs logic

    private void StartFixedStairPath(StairPath stair, bool upwards)
    {
        fixedPath.Clear();

        IReadOnlyList<Transform> points = stair.PathPoints;

        if (upwards)
        {
            fixedPath.Add(stair.BottomEntry.position);

            for (int i = 0; i < points.Count; i++)
            {
                fixedPath.Add(points[i].position);
            }

            fixedPath.Add(stair.TopEntry.position);
        }
        else
        {
            fixedPath.Add(stair.TopEntry.position);

            for (int i = points.Count - 1; i >= 0; i--)
            {
                fixedPath.Add(points[i].position);
            }

            fixedPath.Add(stair.BottomEntry.position);
        }

        fixedPathIndex = 0;
        followingFixedPath = true;
        canMove = false;
    }

    private void FollowFixedPath()
    {
        if (fixedPathIndex >= fixedPath.Count)
        {
            FinishFixedPath();
            return;
        }

        Vector2 point = fixedPath[fixedPathIndex];

        if (
            Vector2.Distance(rb.position, point)
            <= nextWaypointDistance
        )
        {
            fixedPathIndex++;
            return;
        }

        MoveTowards(point);
    }

    private void FinishFixedPath()
    {
        followingFixedPath = false;
        fixedPath.Clear();
        fixedPathIndex = 0;

        canMove = true;
        targetPos = transform.position;
    }
}