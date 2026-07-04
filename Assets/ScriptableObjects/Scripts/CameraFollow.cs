using UnityEngine;
using UnityEngine.U2D;

/// Attach to the Pixel Perfect Camera in PERSISTOBJECTS.
/// Follows the player smoothly and stays within room bounds.
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [Header("Follow")]
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Bounds — set per room via RoomSetup")]
    private Bounds _roomBounds;
    private bool   _hasBounds = false;

    private Camera    _cam;
    private Transform _target;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        _cam = GetComponent<Camera>();
    }

    void Start()
    {
        if (PlayerMovement.Instance != null)
            _target = PlayerMovement.Instance.transform;
    }

    void LateUpdate()
    {
        if (_target == null) return;

        Vector3 desired = new Vector3(_target.position.x, _target.position.y, transform.position.z);

        if (_hasBounds)
            desired = ClampToBounds(desired);

        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }

    public void SetRoomBounds(Bounds bounds)
    {
        _roomBounds = bounds;
        _hasBounds  = true;
    }

    public void ClearBounds() => _hasBounds = false;

    public void SnapToTarget()
    {
        if (_target == null) return;
        Vector3 pos = new Vector3(_target.position.x, _target.position.y, transform.position.z);
        if (_hasBounds) pos = ClampToBounds(pos);
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

        // Room smaller than screen — center it
        if (minX > maxX) { float cx = _roomBounds.center.x; minX = maxX = cx; }
        if (minY > maxY) { float cy = _roomBounds.center.y; minY = maxY = cy; }

        return new Vector3(Mathf.Clamp(pos.x, minX, maxX), Mathf.Clamp(pos.y, minY, maxY), pos.z);
    }
}