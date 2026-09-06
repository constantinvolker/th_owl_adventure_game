using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [Header("Verfolgung (Follow)")]
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Ziel (Target)")]
    [Tooltip("Ziehe deinen Spieler hier rein!")]
    [SerializeField] private Transform _target;

    [Header("Grenzen (Bounds)")]
    private Bounds _roomBounds;
    private bool _hasBounds = false;

    private Camera _cam;
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

        // URP Kamera Setup
        _additionalCameraData = GetComponent<UniversalAdditionalCameraData>();
        if (_additionalCameraData == null)
            _additionalCameraData = gameObject.AddComponent<UniversalAdditionalCameraData>();

        _additionalCameraData.renderType = CameraRenderType.Base;
        _cam.cullingMask = -1;
    }

    void Start()
    {
        TryFindTarget();
    }

    void LateUpdate()
    {
        // 1. Haben wir ein Ziel?
        if (_target == null)
        {
            TryFindTarget();
            if (_target == null) return;
        }

        // 2. Zielposition berechnen. Zwingt die Kamera auf Z = -10!
        Vector3 desired = new Vector3(
            _target.position.x,
            _target.position.y,
            -10f // WICHTIG: Fest auf -10 gesetzt, damit sie nicht in den Spieler rutscht
        );

        // 3. Grenzen anwenden
        if (_hasBounds)
            desired = ClampToBounds(desired);

        // 4. Bewegen
        transform.position = Vector3.Lerp(
            transform.position,
            desired,
            smoothSpeed * Time.deltaTime
        );
    }

    private void TryFindTarget()
    {
        if (_target == null && PlayerMovement.Instance != null)
        {
            _target = PlayerMovement.Instance.transform;
            Debug.Log("CameraFollow: Spieler wurde erfolgreich gefunden und als Ziel gesetzt!");
        }
        else if (_target == null)
        {
            Debug.LogWarning("CameraFollow: Warte auf Spieler... Kein Ziel gefunden!");
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

        Vector3 pos = new Vector3(_target.position.x, _target.position.y, -10f);

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