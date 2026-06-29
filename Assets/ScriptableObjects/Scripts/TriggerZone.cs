using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Trigger zone that switches the referenced LayerToggle object
/// to its foreground and background sorting layer.
/// </summary>
/// <remarks>
/// Use case: Colliders are used for multiple objects.
/// </remarks>
public class TriggerZone : MonoBehaviour
{
    [Header("Objects to toggle")]
    public Transform[] targetsRoot;

    private LayerToggle[] targets;

    private void OnValidate()
    {
        if (targetsRoot == null || targetsRoot.Length == 0)
        {
            targets = new LayerToggle[0];
            return;
        }

        List<LayerToggle> collected = new List<LayerToggle>();

        foreach (var root in targetsRoot)
        {
            if (root == null) continue;

            collected.AddRange(root.GetComponentsInChildren<LayerToggle>(true));
        }

        targets = collected.ToArray();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (var t in targets)
        {
            t?.TriggerForeground();
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (var t in targets)
        {
            t?.TriggerForeground();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (var t in targets)
        {
            t?.TriggerBackground();
        }
    }
}
