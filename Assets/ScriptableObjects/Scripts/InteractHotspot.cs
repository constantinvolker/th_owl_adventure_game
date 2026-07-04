using UnityEngine;
using UnityEngine.Events;

/// Attach to any world object the player can interact with (inspect, open, search etc.)
/// No item required — just click and something happens.
/// Wire up OnInteract in the Inspector per object.
public class InteractHotspot : MonoBehaviour
{
    [Tooltip("Optional label shown as tooltip or cursor hint. E.g. 'Schublade öffnen'")]
    public string interactLabel;

    [Space]
    public UnityEvent OnInteract;

    public void Interact()
    {
        OnInteract?.Invoke();
    }
}