using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [SerializeField] private string gameplayPrefix = "Room";
    [SerializeField] private GameObject gameplayCanvas;

    public TextMeshProUGUI uiTextDisplay;
    private Coroutine hideCoroutine;

    private bool _loadingViaRoutine = false;

    void Awake() => Instance = this;

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        if (uiTextDisplay == null)
        {
            GameObject textObj = GameObject.Find("RoomNameText");

            if (textObj != null)
            {
                uiTextDisplay = textObj.GetComponent<TextMeshProUGUI>();
                Debug.Log("RoomNameText gefunden");
            }
            else
            {
                Debug.LogWarning("RoomNameText nicht gefunden! UI-Text wird nicht angezeigt.");
            }
        }

        if (uiTextDisplay != null)
        {
            uiTextDisplay.text = "";
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isGameplay = scene.name.StartsWith(gameplayPrefix);
        bool isOpening = scene.name.ToLower().Contains("opening");

        if (gameplayCanvas != null)
        {
            // Canvas aktiv lassen, egal ob normales Spiel oder Intro
            gameplayCanvas.SetActive(isGameplay || isOpening);

            // FIX: Inventar und HUD zwingend wieder einschalten, wenn wir in einem normalen Raum sind!
            if (isGameplay)
            {
                Transform inventory = gameplayCanvas.transform.Find("InventoryUI");
                if (inventory != null) inventory.gameObject.SetActive(true);

                Transform hud = gameplayCanvas.transform.Find("HUDController");
                if (hud != null) hud.gameObject.SetActive(true);
            }
        }

        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.gameObject.SetActive(isGameplay);  // Nur in Rooms aktiv
            if (isGameplay && _loadingViaRoutine)
                PlayerMovement.Instance.canMove = false;
        }
    }

    public void LoadRoom(string sceneName, string spawnName)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveLastRoom(sceneName, spawnName);
        }
        StartCoroutine(LoadRoutine(sceneName, spawnName));
    }

    private IEnumerator LoadRoutine(string sceneName, string spawnName)
    {
        _loadingViaRoutine = true;

        yield return SceneManager.LoadSceneAsync(sceneName);

        _loadingViaRoutine = false;

        // Place player at spawn
        foreach (var sp in FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None))
        {
            if (sp.spawnName == spawnName)
            {
                PlayerMovement.Instance.transform.position = sp.transform.position;
                PlayerMovement.Instance.StopMoving();
                break;
            }
        }

        // Snap camera before fade-in so there's no camera pop
        if (CameraFollow.Instance != null)
            CameraFollow.Instance.SnapToTarget();

        // Fade in then enable movement
        if (SceneTransition.Instance != null)
        {
            yield return StartCoroutine(SceneTransition.Instance.FadeIn());
        }

        if (PlayerMovement.Instance != null)
            PlayerMovement.Instance.canMove = true;

        foreach (var h in FindObjectsByType<TransitionHotspot>(FindObjectsSortMode.None))
            h.ResetTrigger();

        // Raum-Name anzeigen (nur wenn UI existiert)
        if (uiTextDisplay != null)
        {
            if (hideCoroutine != null) StopCoroutine(hideCoroutine);

            uiTextDisplay.text = GetSceneNameForTooltip(sceneName);
            uiTextDisplay.color = new Color(uiTextDisplay.color.r, uiTextDisplay.color.g, uiTextDisplay.color.b, 1f);
            uiTextDisplay.ForceMeshUpdate();

            hideCoroutine = StartCoroutine(HideTextAfterDelay(3f));
        }
    }

    private System.Collections.IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (uiTextDisplay != null)
        {
            uiTextDisplay.text = "";
        }
    }

    private string GetSceneNameForTooltip(string sceneName)
    {
        switch (sceneName)
        {
            case "Opening":
                return "";
            case "Room_ApartmentBedroom":
                return "Schlafzimmer";
            case "Room_ApartmentLivingroom":
                return "Wohnzimmer";
            case "Room_Vorplatz":
                return "Vorplatz";
            case "Room_Entrancehall":
                return "Eingangshalle";
            case "Room_Library":
                return "Bibliothek";
            case "Room_Hall_0":
                return "Etage 0";
            case "Room_Hall_2":
                return "Etage 2";
            case "Room_Hall_3":
                return "Etage 3";
            case "Room_Hall_4":
                return "Etage 4";
            case "Room_Hall_5":
                return "Etage 5";
            case "Room_Hall_6":
                return "Etage 6";
            case "Room_Hall_7":
                return "Etage 7";
            case "Room_Auditorium":
                return "Audimax";
            case "Room_Stairway":
                return "Treppenhaus";
            case "Room_Elevator":
                return "Aufzug";
            case "Room_InformatikRaum":
                return "Informatikraum";
            case "Room_Physik":
                return "Physiklabor";
            default:
                return "Name des Raums: " + sceneName + " im Script SceneLoader hinzufügen";
        }
    }
}