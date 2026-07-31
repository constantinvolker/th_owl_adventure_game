using System.Collections.Generic;
using UnityEngine;

public class StairPath : MonoBehaviour
{
    [Header("Path points from bottom to top")]
    [SerializeField] private List<Transform> pathPoints = new();

    [Header("Entry points")]
    [SerializeField] private Transform bottomEntry;
    [SerializeField] private Transform topEntry;

    public Transform BottomEntry => bottomEntry;
    public Transform TopEntry => topEntry;
    public IReadOnlyList<Transform> PathPoints => pathPoints;

    public bool IsCloserToBottom(Vector2 playerPosition)
    {
        float bottomDistance = Vector2.Distance(
            playerPosition,
            bottomEntry.position
        );

        float topDistance = Vector2.Distance(
            playerPosition,
            topEntry.position
        );

        return bottomDistance <= topDistance;
    }
}