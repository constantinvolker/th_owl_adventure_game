using UnityEngine;

/// <summary>
/// Trigger zone that switches the referenced LayerToggle object
/// back to its background sorting layer.
/// </summary>
public class BackgroundZone : MonoBehaviour
{
    public LayerToggle wall;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            wall?.TriggerBackground();
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            wall?.TriggerBackground();
    }
}
