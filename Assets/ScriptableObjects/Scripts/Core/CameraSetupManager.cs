using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Initialisiert alle Kameras im Projekt für URP + Pixel Perfect + 2D Lights Kompatibilität.
/// Wird in jeder Szene automatisch ausgeführt.
/// </summary>
public class CameraSetupManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void SetupAllCameras()
    {
        Camera[] allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);

        foreach (Camera cam in allCameras)
        {
            ConfigureCamera(cam);
        }
    }

    private static void ConfigureCamera(Camera cam)
    {
        // Stelle sicher, dass UniversalAdditionalCameraData existiert
        UniversalAdditionalCameraData additionalData = cam.GetComponent<UniversalAdditionalCameraData>();
        if (additionalData == null)
        {
            additionalData = cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
        }

        // CRITICAL: Richtige Einstellungen für URP + Pixel Perfect + 2D Lights
        additionalData.renderType = CameraRenderType.Base;

        // Culling Mask auf Everything setzen (damit URP Lights sichtbar sind)
        cam.cullingMask = -1; // -1 = Everything

        // Clear Flags richtig setzen
        cam.clearFlags = CameraClearFlags.SolidColor;
    }
}