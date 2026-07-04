using UnityEngine;
using UnityEngine.Events;

public class OpenableObject : MonoBehaviour
{
    [Header("Identity")]
    [Tooltip("Unique ID for save state. Must be unique across all scenes. E.g. 'Bedroom_Chest_01'")]
    [SerializeField] private string objectID;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite         closedSprite;
    [SerializeField] private Sprite         openSprite;

    [Header("Loot")]
    [SerializeField] private PickupHotspot lootPickup;

    [Header("Dialogue")]
    [SerializeField] private DialogueData openDialogue;
    [SerializeField] private DialogueData lootDialogue;
    [SerializeField] private DialogueData searchedDialogue;

    [Header("State")]
    [SerializeField] private bool startOpen = false;

    public UnityEvent OnOpened;
    public UnityEvent OnSearched;

    private bool _isOpen   = false;
    private bool _isLooted = false;

    void Start()
    {
        // Restore state from save
        if (!string.IsNullOrEmpty(objectID))
        {
            _isOpen   = GameManager.Instance.IsObjectOpen(objectID);
            _isLooted = GameManager.Instance.IsObjectLooted(objectID);
        }
        else
        {
            _isOpen = startOpen;
            Debug.LogWarning($"[OpenableObject] '{gameObject.name}' has no objectID — state won't be saved!");
        }

        // Disable only the PickupHotspot component, not the whole GO
        // (InteractHotspot on same GO must stay active for searched dialogue)
        if (_isLooted && lootPickup != null)
            lootPickup.enabled = false;

        UpdateSprite();
    }

    public void Interact()
    {
        if (!_isOpen)
        {
            _isOpen = true;
            UpdateSprite();

            // Save open state
            if (!string.IsNullOrEmpty(objectID))
                GameManager.Instance.MarkObjectOpen(objectID);

            // Give loot immediately and save looted state
            if (lootPickup != null && !_isLooted)
            {
                _isLooted = true;
                if (!string.IsNullOrEmpty(objectID))
                    GameManager.Instance.MarkObjectLooted(objectID);
                lootPickup.Pickup();
            }

            // Open dialogue first, loot dialogue queues after
            if (openDialogue != null)
                DialogueManager.Instance.PlaySimpleDialogue(openDialogue);
            if (lootDialogue != null)
                DialogueManager.Instance.QueueDialogue(lootDialogue);

            OnOpened?.Invoke();
        }
        else
        {
            if (searchedDialogue != null)
                DialogueManager.Instance.PlaySimpleDialogue(searchedDialogue);
            OnSearched?.Invoke();
        }
    }

    private void UpdateSprite()
    {
        if (spriteRenderer == null) return;
        // Only swap sprite if open sprite is assigned
        if (_isOpen && openSprite != null)
            spriteRenderer.sprite = openSprite;
        else if (!_isOpen && closedSprite != null)
            spriteRenderer.sprite = closedSprite;
    }
}