using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [Header("Verfolgung (Follow)")]
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Ziel (Target)")]
    [Tooltip("Ziehe deinen Spieler hier rein!")]
    [SerializeField] private Transform _target; // <-- HIER IST DER FIX! Jetzt im Inspector sichtbar.

    [Header("Grenzen (Bounds)")]
    private Bounds _roomBounds;
    private bool _hasBounds = false;

    private Camera _cam;
    private UnityEngine.U2D.PixelPerfectCamera _pixelPerfect;
    private UniversalAdditionalCameraData _additionalCameraData;

    void Awake()
    {
        // Singleton Setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _cam = GetComponent<Camera>();
        _pixelPerfect = GetComponent<UnityEngine.U2D.PixelPerfectCamera>();

        // URP Kamera Setup
        _additionalCameraData = GetComponent<UniversalAdditionalCameraData>();
        if (_additionalCameraData == null)
            _additionalCameraData = gameObject.AddComponent<UniversalAdditionalCameraData>();

        _additionalCameraData.renderType = CameraRenderType.Base;
        _cam.cullingMask = -1;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Start()
    {
        TryFindTarget();
    }

    void LateUpdate()
    {
        // Wenn wir kein Ziel haben, versuche es nochmal zu finden. 
        // Bricht ab, wenn immer noch keins da ist.
        if (_target == null)
        {
            TryFindTarget();
            if (_target == null) return;
        }

        // Zielposition berechnen. WICHTIG: Beh�lt die aktuelle Z-Position der Kamera bei (sollte -10 sein!)
        Vector3 desired = new Vector3(
            _target.position.x,
            _target.position.y,
            transform.position.z
        );

        // Grenzen anwenden, falls vorhanden
        if (_hasBounds)
            desired = ClampToBounds(desired);

        // Sanfte Kamerabewegung (Lerp)
        transform.position = Vector3.Lerp(
            transform.position,
            desired,
            smoothSpeed * Time.deltaTime
        );
    }

    private void TryFindTarget()
    {
        // Sucht nur automatisch nach dem Spieler, wenn das Feld im Inspector leer gelassen wurde
        if (_target == null && PlayerMovement.Instance != null)
        {
            _target = PlayerMovement.Instance.transform;
        }
    }

    public void SetRoomBounds(Bounds bounds)
    {
        _roomBounds = bounds;
        _hasBounds = true;
    }

    public void ClearBounds()
    {
        _hasBounds = false;
    }

    public void SnapToTarget()
    {
        if (_target == null) return;

        Vector3 pos = new Vector3(
            _target.position.x,
            _target.position.y,
            transform.position.z
        );

        if (_hasBounds)
            pos = ClampToBounds(pos);

        transform.position = pos;
    }

    private Vector3 ClampToBounds(Vector3 pos)
    {
        float halfH = _cam.orthographicSize;
        float halfW = halfH * _cam.aspect;

        float minX = _roomBounds.min.x + halfW;
        float maxX = _roomBounds.max.x - halfW;
        float minY = _roomBounds.min.y + halfH;
        float maxY = _roomBounds.max.y - halfH;

        if (minX > maxX)
        {
            float cx = _roomBounds.center.x;
            minX = maxX = cx;
        }

        if (minY > maxY)
        {
            float cy = _roomBounds.center.y;
            minY = maxY = cy;
        }

        return new Vector3(
            Mathf.Clamp(pos.x, minX, maxX),
            Mathf.Clamp(pos.y, minY, maxY),
            pos.z
        );
    }
}