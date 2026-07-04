using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [SerializeField] private string     gameplayPrefix = "Room";
    [SerializeField] private GameObject gameplayCanvas;

    private bool _loadingViaRoutine = false;

    void Awake() => Instance = this;

    void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

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
    }
}