using UnityEngine;
using UnityEngine.Events;

/// Tracks a group of CleanupItems.
/// Fires OnAllCleaned when every item in the group is cleaned up.
public class CleanupGroup : MonoBehaviour
{
    [SerializeField] private CleanupItem[] items;

    public UnityEvent OnAllCleaned;

    private int _cleanedCount = 0;

    void Start()
    {
        if (items == null || items.Length == 0)
            items = GetComponentsInChildren<CleanupItem>();
    }

    public void ReportCleaned(CleanupItem item)
    {
        _cleanedCount++;

        if (_cleanedCount >= items.Length)
            OnAllCleaned?.Invoke();
    }
}