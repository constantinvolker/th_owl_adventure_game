using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// Tracks a group of CleanupItems.
/// Fires OnAllCleaned when every item in the group is cleaned up.
public class CleanupGroup : MonoBehaviour
{
    [SerializeField] private CleanupItem[] items;

    public UnityEvent OnAllCleaned;

    private int _cleanedCount = 0;
    private int _targetCount = 0;

    void Start()
    {
        if (items == null || items.Length == 0)
            items = GetComponentsInChildren<CleanupItem>();

        // Gesamtanzahl aller Items (auch bereits deaktivierte zählen zum Target)
        _targetCount = items?.Length ?? 0;

        // Zähle bereits "gesäuberte" Items (null oder deaktiviert)
        _cleanedCount = (items == null) ? 0 : items.Count(i => i == null || !i.gameObject.activeInHierarchy);

        // Wenn keine Items vorhanden sind, nichts weiter tun (kein erneutes Auslösen)
        if (_targetCount == 0)
            return;

        // Wenn bereits komplett, event nicht nochmal auslösen — verhindert doppelte Belohnung nach Szenenwechsel
        if (_cleanedCount >= _targetCount)
            return;
    }

    public void ReportCleaned(CleanupItem item)
    {
        _cleanedCount++;

        // Event nur beim erstmaligen Erreichen der Zielanzahl feuern
        if (_cleanedCount == _targetCount)
            OnAllCleaned?.Invoke();
    }
}