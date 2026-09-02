using System.Collections.Generic;
using UnityEngine;

public class StairPath : MonoBehaviour
{
    [Header("Zugänge")]
    [SerializeField] private Transform bottomEntry;
    [SerializeField] private Transform topEntry;

    [Header("Reihenfolge: unten nach oben")]
    [SerializeField] private List<Transform> pointsBottomToTop;

    public Transform BottomEntry => bottomEntry;
    public Transform TopEntry => topEntry;
    public IReadOnlyList<Transform> PointsBottomToTop => pointsBottomToTop;

    public bool IsCloserToBottom(Vector2 playerPosition)
    {
        float bottomDistance =
            Vector2.Distance(playerPosition, bottomEntry.position);

        float topDistance =
            Vector2.Distance(playerPosition, topEntry.position);

        return bottomDistance <= topDistance;
    }
}