using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [SerializeField] private string     gameplayPrefix = "Room";
    [SerializeField] private GameObject gameplayCanvas;

    public TextMeshProUGUI uiTextDisplay;
    private Coroutine hideCoroutine;

    private bool _loadingViaRoutine = false;

    void Awake() => Instance = this;

    void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        if (uiTextDisplay == null)
        {
            GameObject textObj = GameObject.Find("RoomNameText");

            if (textObj != null)
            {
                uiTextDisplay = textObj.GetComponent<TextMeshProUGUI>();
                Debug.Log(" gefunden");
            }
            else
                Debug.Log("nicht gefunden");
        }

        uiTextDisplay.text = "";
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isGameplay = scene.name.StartsWith(gameplayPrefix);

        if (gameplayCanvas != null)
            gameplayCanvas.SetActive(isGameplay);

        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.gameObject.SetActive(isGameplay);
            if (isGameplay && _loadingViaRoutine)
                PlayerMovement.Instance.canMove = false;
        }
    }

    public void LoadRoom(string sceneName, string spawnName)
    {
        GameManager.Instance.SaveLastRoom(sceneName, spawnName);
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
            CameraFollow.Instance?.SnapToTarget();

        // Fade in then enable movement
        yield return StartCoroutine(SceneTransition.Instance.FadeIn());

        if (PlayerMovement.Instance != null)
            PlayerMovement.Instance.canMove = true;

        foreach (var h in FindObjectsByType<TransitionHotspot>(FindObjectsSortMode.None))
            h.ResetTrigger();

        Debug.Log("Jetzt Text anzeigen");

        if (hideCoroutine != null) StopCoroutine(hideCoroutine);

        uiTextDisplay.text = GetSceneNameForTooltip(sceneName);
        Debug.Log(sceneName);
        uiTextDisplay.color = new Color(uiTextDisplay.color.r, uiTextDisplay.color.g, uiTextDisplay.color.b, 1f);

        uiTextDisplay.ForceMeshUpdate();

        hideCoroutine = StartCoroutine(HideTextAfterDelay(3f));
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
        Debug.Log(sceneName);
        switch (sceneName)
        {
            case "Room_ApartmentBedroom":
                return "Schlafzimmer";
                break;

            case "Room_ApartmentLivingroom":
                return "Wohnzimmer";
                break;

            case "Room_Vorplatz":
                return "Vorplatz";
                break;

            case "Room_Entrancehall":
                return "Eingangshalle";
                break;

            case "Room_Library":
                return "Bibliothek";
                break;

            case "Room_Hall_0":
                return "Etage 0";
                break;

            case "Room_Hall_2":
                return "Etage 2";
                break;

            case "Room_Hall_3":
                return "Etage 3";
                break;

            case "Room_Hall_4":
                return "Etage 4";
                break;

            case "Room_Hall_5":
                return "Etage 5";
                break;

            case "Room_Hall_6":
                return "Etage 6";
                break;

            case "Room_Hall_7":
                return "Etage 7";
                break;

            case "Room_Auditorium":
                return "Audimax";
                break;

            case "Room_Stairway":
                return "Treppenhaus";
                break;
        }

        return "Name des Raums: " + sceneName + " im Script SceneLoader hinzufügen";
    }
}