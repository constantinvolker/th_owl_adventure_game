using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const string PREFIX_ITEM    = "item_collected_";
    private const string PREFIX_SPOKEN  = "npc_spoken_";
    private const string KEY_BACKPACK   = "backpack_unlocked";
    private const string KEY_HAS_SAVE   = "has_save";
    private const string KEY_LAST_SCENE = "last_scene";
    private const string KEY_LAST_SPAWN = "last_spawn";

    private bool _pendingReset = false;
    public  bool PendingReset  => _pendingReset;

    [Header("New Game Settings")]
    [SerializeField] private string firstSceneName  = "Room_ApartmentBedroom";
    [SerializeField] private string firstSpawnPoint = "SpawnStart";

    [Header("Global Dialogues")]
    [Tooltip("Plays when player tries to pick up an item without a backpack.")]
    [SerializeField] public DialogueData noBackpackDialogue;

    [Tooltip("Plays when player uses the wrong item on an object.")]
    [SerializeField] public DialogueData wrongItemDialogue;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ── Items ─────────────────────────────────────────────────────────────────

    public bool IsItemCollected(string itemID)
        => PlayerPrefs.GetInt(PREFIX_ITEM + itemID, 0) == 1;

    public void MarkItemCollected(string itemID)
    {
        PlayerPrefs.SetInt(PREFIX_ITEM + itemID, 1);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
    }

    // ── Backpack ──────────────────────────────────────────────────────────────

    public bool IsBackpackUnlocked()
        => PlayerPrefs.GetInt(KEY_BACKPACK, 0) == 1;

    public void MarkBackpackUnlocked()
    {
        PlayerPrefs.SetInt(KEY_BACKPACK, 1);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
    }

    // ── NPC ───────────────────────────────────────────────────────────────────

    public bool HasSpokenTo(string npcID)
        => PlayerPrefs.GetInt(PREFIX_SPOKEN + npcID, 0) == 1;

    public void MarkSpokenTo(string npcID)
    {
        PlayerPrefs.SetInt(PREFIX_SPOKEN + npcID, 1);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
    }

    // ── Last room ─────────────────────────────────────────────────────────────

    /// Called by SceneLoader every time the player enters a room.
    public void SaveLastRoom(string sceneName, string spawnName)
    {
        PlayerPrefs.SetString(KEY_LAST_SCENE, sceneName);
        PlayerPrefs.SetString(KEY_LAST_SPAWN, spawnName);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
    }

    public string GetLastScene()
        => PlayerPrefs.GetString(KEY_LAST_SCENE, firstSceneName);

    public string GetLastSpawn()
        => PlayerPrefs.GetString(KEY_LAST_SPAWN, firstSpawnPoint);

    // ── Object state ─────────────────────────────────────────────────────────────

    private const string PREFIX_OBJ_OPEN   = "obj_open_";
    private const string PREFIX_OBJ_LOOTED = "obj_looted_";

    public bool IsObjectOpen(string objectID)
        => PlayerPrefs.GetInt(PREFIX_OBJ_OPEN + objectID, 0) == 1;

    public void MarkObjectOpen(string objectID)
    {
        PlayerPrefs.SetInt(PREFIX_OBJ_OPEN + objectID, 1);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
    }

    public bool IsObjectLooted(string objectID)
        => PlayerPrefs.GetInt(PREFIX_OBJ_LOOTED + objectID, 0) == 1;

    public void MarkObjectLooted(string objectID)
    {
        PlayerPrefs.SetInt(PREFIX_OBJ_LOOTED + objectID, 1);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
    }

    // ── Save state ────────────────────────────────────────────────────────────────

    public bool HasSaveData()
        => PlayerPrefs.GetInt(KEY_HAS_SAVE, 0) == 1;

    // ── New Game ──────────────────────────────────────────────────────────────

    public void StartNewGame(string sceneName = null, string spawnName = null)
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        _pendingReset = true;
        InventoryManager.Instance.ResetInventory();
        Time.timeScale = 1f;

        SceneLoader.Instance.LoadRoom(
            sceneName  ?? firstSceneName,
            spawnName  ?? firstSpawnPoint
        );
    }

    public void ClearPendingReset() => _pendingReset = false;
}