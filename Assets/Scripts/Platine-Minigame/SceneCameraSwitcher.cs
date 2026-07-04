using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCameraSwitcher : MonoBehaviour
{
    [Header("Kamera Einstellungen")]
    public Camera puzzleCamera; // Ziehe hier die neue Kamera dieser Szene rein

    private Camera persistentCamera; // Die Kamera aus der vorherigen Szene

    void Start()
    {
        // 1. Finde alle Kameras in der Szene
        Camera[] allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);

        foreach (Camera cam in allCameras)
        {
            // Wenn es nicht die neue Puzzle-Kamera ist, muss es die mitgereiste Kamera sein
            if (cam != puzzleCamera)
            {
                persistentCamera = cam;

                // 2. Die alte Kamera und ihr AudioListener deaktivieren
                persistentCamera.gameObject.SetActive(false);
                break;
            }
        }

        // 3. Sicherstellen, dass die Puzzle-Kamera aktiv ist
        if (puzzleCamera != null)
        {
            puzzleCamera.gameObject.SetActive(true);
        }
    }

    void OnEnable()
    {
        // Registrieren für den Szenenwechsel
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        // Abmelden vom Szenenwechsel
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // Wird aufgerufen, BEVOR diese Szene gelöscht wird
    private void OnSceneUnloaded(Scene currentScene)
    {
        // 4. Die alte persistent Kamera wieder einschalten, bevor wir gehen
        if (persistentCamera != null)
        {
            persistentCamera.gameObject.SetActive(true);
        }
    }
}