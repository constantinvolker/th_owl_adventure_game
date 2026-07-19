using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Button     toggleButton;
    [SerializeField] private Transform  slotContainer;
    [SerializeField] private GameObject slotPrefab;

    private bool _panelVisible = false;
    private bool _unlocked     = false;

    void Start()
    {
        // Subscribe to all events
        InventoryManager.Instance.OnBackpackUnlocked += HandleBackpackUnlocked;
        InventoryManager.Instance.OnInventoryChanged  += RebuildSlots;
        InventoryManager.Instance.OnInventoryReset    += HandleReset;

        // Apply initial state — covers scene reloads and session restores
        ApplyCurrentState();
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance == null) return;
        InventoryManager.Instance.OnBackpackUnlocked -= HandleBackpackUnlocked;
        InventoryManager.Instance.OnInventoryChanged  -= RebuildSlots;
        InventoryManager.Instance.OnInventoryReset    -= HandleReset;
    }

    // ── Event Handlers ────────────────────────────────────────────────────────

    private void HandleBackpackUnlocked()
    {
        _unlocked = true;
        toggleButton.gameObject.SetActive(true);
        SetPanelVisible(true);
    }

    private void HandleReset()
    {
        // Fully hide and lock the UI — called when a new game starts
        _unlocked = false;
        _panelVisible = false;
        toggleButton.gameObject.SetActive(false);
        inventoryPanel.SetActive(false);
        ClearSlots();
    }

    private void RebuildSlots()
    {
        ClearSlots();
        foreach (var item in InventoryManager.Instance.GetItems())
        {
            var slotGO = Instantiate(slotPrefab, slotContainer);
            slotGO.GetComponent<InventorySlot>().Setup(item);
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// Syncs UI to current InventoryManager state.
    /// Needed on Start() since events may have fired before this UI subscribed.
    private void ApplyCurrentState()
    {
        if (InventoryManager.Instance.BackpackUnlocked)
        {
            _unlocked = true;
            toggleButton.gameObject.SetActive(true);
            // Don't auto-open panel on restore — player may have closed it
        }
        else
        {
            _unlocked = false;
            toggleButton.gameObject.SetActive(false);
            inventoryPanel.SetActive(false);
        }

        RebuildSlots();
    }

    private void TogglePanel()
    {
        if (!_unlocked) return;
        SetPanelVisible(!_panelVisible);
    }

    private void SetPanelVisible(bool visible)
    {
        _panelVisible = visible;
        inventoryPanel.SetActive(visible);
    }

    private void ClearSlots()
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);
    }

    // toggleButton.onClick listener must be added after Start
    void Awake()
    {
        // Delay listener setup to Start so toggleButton is guaranteed assigned
    }

    // Add listener here since we need it after serialized fields are ready
    void OnEnable()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(TogglePanel);
    }

    void OnDisable()
    {
        if (toggleButton != null)
            toggleButton.onClick.RemoveListener(TogglePanel);
    }
}