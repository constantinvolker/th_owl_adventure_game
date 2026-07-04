using UnityEngine;
using UnityEngine.InputSystem;
using Pathfinding;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("Cursor Textures")]
    [SerializeField] private Texture2D cursorNormal;
    [SerializeField] private Texture2D cursorClicked;
    [SerializeField] private Texture2D cursorWalk;
    [SerializeField] private Sprite cursorWalkClicked;
    [SerializeField] private Texture2D cursorDialogue;
    [SerializeField] private Texture2D cursorInteract;
    [SerializeField] private Texture2D cursorTransition;
    [SerializeField] private Texture2D cursorBlocked;

    [Header("Raycast")]
    [SerializeField] private LayerMask hotspotLayer;
    [SerializeField] private Camera    worldCamera;

    [Header("Scale")]
    [Tooltip("Scale small cursors up to keep pixel art crispness.")]
    [SerializeField] private int cursorScale = 2;

    private CursorType _currentType = CursorType.Normal;
    private bool       _itemHeld    = false;
    private bool       _clicking    = false;
    public enum CursorType
    {
        Normal, Walk, Dialogue, Interact, Transition, Blocked
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    void Start()
    {
        if (!worldCamera) worldCamera = Camera.main;

        ItemSelectionState.Instance.OnItemSelected   += HandleItemSelected;
        ItemSelectionState.Instance.OnItemDeselected += HandleItemDeselected;

        SetCursor(cursorNormal);
    }

    void OnDestroy()
    {
        if (ItemSelectionState.Instance == null) return;
        ItemSelectionState.Instance.OnItemSelected   -= HandleItemSelected;
        ItemSelectionState.Instance.OnItemDeselected -= HandleItemDeselected;
    }

    void Update()
    {
        if (_itemHeld) return;
        if (DialogueManager.Instance.IsPlaying)
        {
            ApplyType(CursorType.Normal, false);
            return;
        }

        CursorType desired  = GetCursorForMousePosition();
        bool       clicking = Mouse.current != null && Mouse.current.leftButton.isPressed;

        // Start/stop walk animation
        if (desired == CursorType.Walk && clicking)
        {
            if (!_clicking) StartWalkAnimation();
        }
        else
        {
            if (desired != _currentType || clicking != _clicking)
                ApplyType(desired, clicking);
        }

        _clicking = clicking;
    }

    // ── Public ────────────────────────────────────────────────────────────────

    public void SetItemCursor(Sprite icon)
    {
        _itemHeld = true;
        if (icon == null || !icon.texture.isReadable)
        {
            SetCursor(cursorInteract);
            return;
        }
        // Extract the sprite's sub-region in case it comes from a sprite sheet,
        // otherwise the entire sheet would get pinned to the cursor.
        Texture2D iconTex = SpriteToTexture(icon);
        if (iconTex == null)
        {
            SetCursor(cursorInteract);
            return;
        }
        Texture2D scaled = ScaleTexture(iconTex, cursorScale);
        Cursor.SetCursor(scaled, new Vector2(scaled.width / 2f, scaled.height / 2f), CursorMode.Auto);
    }

    public void ResetToNormal()
    {
        _itemHeld = false;
        SetCursorType(CursorType.Normal);
    }

    // ── Event Handlers ────────────────────────────────────────────────────────

    private void HandleItemSelected(ItemData item)
    {
        SetItemCursor(item?.icon);
    }

    private void HandleItemDeselected()
    {
        ResetToNormal();
    }

    // ── Animation ─────────────────────────────────────────────────────────────

    private void StartWalkAnimation()
    {
        SetCursor(cursorWalkClicked != null ? SpriteToTexture(cursorWalkClicked) : cursorWalk);
    }



    // ── Cursor Logic ──────────────────────────────────────────────────────────

    private void ApplyType(CursorType type, bool clicking)
    {
        _currentType = type;

        if (clicking)
        {
            SetCursor(type == CursorType.Walk ? cursorWalk : cursorClicked);
            return;
        }

        SetCursorType(type);
    }

    private CursorType GetCursorForMousePosition()
    {
        if (worldCamera == null) return CursorType.Normal;
        if (!PlayerMovement.Instance.canMove) return CursorType.Normal;

        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return CursorType.Normal;

        Vector3 world = GetMouseWorld();
        var hit = Physics2D.Raycast(world, Vector2.zero, 0f, hotspotLayer);

        if (hit.collider != null)
        {
            if (hit.collider.GetComponent<NpcHotspot>())        return CursorType.Dialogue;
            if (hit.collider.GetComponent<PickupHotspot>())     return CursorType.Interact;
            if (hit.collider.GetComponent<UseHotspot>())        return CursorType.Interact;
            if (hit.collider.GetComponent<InteractHotspot>())    return CursorType.Interact;
            if (hit.collider.GetComponent<TransitionHotspot>())        return CursorType.Transition;
            if (hit.collider.GetComponent<BlockedTransitionHotspot>()) return CursorType.Blocked;
        }

        if (PlayerMovement.Instance.IsWalkable(world)) return CursorType.Walk;

        return CursorType.Normal;
    }

    private void SetCursorType(CursorType type)
    {
        _currentType = type;
        Texture2D tex = type switch
        {
            CursorType.Walk       => cursorWalk,
            CursorType.Dialogue   => cursorDialogue,
            CursorType.Interact   => cursorInteract,
            CursorType.Transition => cursorTransition,
            CursorType.Blocked    => cursorBlocked,
            _                     => cursorNormal,
        };
        SetCursor(tex);
    }

    private void SetCursor(Texture2D texture)
    {
        if (texture == null) return;
        Texture2D toUse = (texture.width < 32 || texture.height < 32)
            ? ScaleTexture(texture, cursorScale)
            : texture;
        Cursor.SetCursor(toUse, new Vector2(toUse.width / 2f, toUse.height / 2f), CursorMode.Auto);
    }

    private Texture2D SpriteToTexture(Sprite sprite)
    {
        if (sprite == null) return null;
        if (sprite.rect.width == sprite.texture.width)
            return sprite.texture;

        // Extract sub-region from sprite sheet
        var tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, sprite.texture.format, false);
        tex.filterMode = FilterMode.Point;
        var pixels = sprite.texture.GetPixels(
            (int)sprite.rect.x, (int)sprite.rect.y,
            (int)sprite.rect.width, (int)sprite.rect.height);
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    private Texture2D ScaleTexture(Texture2D source, int scale)
    {
        if (scale <= 1) return source;
        int newW = source.width  * scale;
        int newH = source.height * scale;
        var result = new Texture2D(newW, newH, source.format, false);
        result.filterMode = FilterMode.Point;
        for (int y = 0; y < newH; y++)
        for (int x = 0; x < newW; x++)
            result.SetPixel(x, y, source.GetPixel(x / scale, y / scale));
        result.Apply();
        return result;
    }

    private Vector3 GetMouseWorld()
    {
        if (Mouse.current == null) return Vector3.zero;
        float depth = Mathf.Abs(worldCamera.transform.position.z -
                                PlayerMovement.Instance.transform.position.z);
        Vector2 mouse = Mouse.current.position.ReadValue();
        Vector3 world = worldCamera.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, depth));
        world.z = PlayerMovement.Instance.transform.position.z;
        return world;
    }
}