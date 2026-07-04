using UnityEngine;

/// <summary>
/// Trigger zone that switches the referenced LayerToggle object
/// back to its foreground sorting layer.
/// </summary>
public class ForegroundZone : MonoBehaviour
{
    public LayerToggle wall;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            wall?.TriggerForeground();
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            wall?.TriggerForeground();
    }
}
