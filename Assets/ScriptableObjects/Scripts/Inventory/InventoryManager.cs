using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public event Action OnInventoryChanged;
    public event Action OnBackpackUnlocked;
    public event Action OnInventoryReset;

    [Header("Starting Items")]
    [Tooltip("Items added to inventory at the start of every new game.")]
    [SerializeField] private ItemData[] startingItems;

    [Tooltip("Items added to inventory when the backpack is picked up.")]
    [SerializeField] private ItemData[] backpackItems;

    private readonly List<ItemData> _items = new();
    private bool _backpackUnlocked = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (GameManager.Instance.PendingReset)
        {
            GameManager.Instance.ClearPendingReset();
            GiveStartingItems();
            return;
        }
        RestoreFromSave();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public bool BackpackUnlocked => _backpackUnlocked;
    public IReadOnlyList<ItemData> GetItems() => _items.AsReadOnly();

    public void AddItem(ItemData item)
    {
        if (item == null) return;

        if (item.isBackpack)
        {
            if (_backpackUnlocked) return;
            _backpackUnlocked = true;
            GameManager.Instance.MarkBackpackUnlocked();
            GameManager.Instance.MarkItemCollected(item.itemID);
            // Add items that start inside the backpack
            GiveBackpackItems();
            OnBackpackUnlocked?.Invoke();
            return;
        }

        if (!_backpackUnlocked)
        {
            Debug.LogWarning($"[InventoryManager] Cannot pick up '{item.itemName}' — backpack not unlocked.");
            return;
        }

        _items.Add(item);
        GameManager.Instance.MarkItemCollected(item.itemID);
        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(ItemData item)
    {
        if (_items.Remove(item))
            OnInventoryChanged?.Invoke();
    }

    public bool HasItem(ItemData item) => _items.Contains(item);

    public void ResetInventory()
    {
        _items.Clear();
        _backpackUnlocked = false;
        OnInventoryReset?.Invoke();
    }

    // ── Save / Restore ────────────────────────────────────────────────────────

    private void GiveBackpackItems()
    {
        if (backpackItems == null || backpackItems.Length == 0) return;
        foreach (var item in backpackItems)
        {
            if (item == null) continue;
            if (GameManager.Instance.IsItemCollected(item.itemID)) continue;
            _items.Add(item);
            GameManager.Instance.MarkItemCollected(item.itemID);
        }
        if (_items.Count > 0)
            OnInventoryChanged?.Invoke();
    }

    private void GiveStartingItems()
    {
        if (startingItems == null || startingItems.Length == 0) return;

        foreach (var item in startingItems)
        {
            if (item == null) continue;
            // Starting items bypass backpack check
            _items.Add(item);
            GameManager.Instance.MarkItemCollected(item.itemID);
        }

        if (_items.Count > 0)
            OnInventoryChanged?.Invoke();
    }

    private void RestoreFromSave()
    {
        if (GameManager.Instance.IsBackpackUnlocked())
        {
            _backpackUnlocked = true;
            OnBackpackUnlocked?.Invoke();
        }

        var allItems = Resources.LoadAll<ItemData>("Items");
        foreach (var item in allItems)
        {
            if (item.isBackpack) continue;
            if (GameManager.Instance.IsItemCollected(item.itemID))
                _items.Add(item);
        }

        if (_items.Count > 0)
            OnInventoryChanged?.Invoke();
    }
}