using UnityEngine;

public class RoomSetup : MonoBehaviour
{
    [SerializeField] private SpawnPoint defaultSpawnPoint;

    [Tooltip("BoxCollider2D defining camera movement area. Size = room size in Units.")]
    [SerializeField] private BoxCollider2D cameraBoundsCollider;

    void Start()
    {
        if (CameraFollow.Instance != null)
        {
            if (cameraBoundsCollider != null)
            {
                CameraFollow.Instance.SetRoomBounds(cameraBoundsCollider.bounds);
                cameraBoundsCollider.enabled = false;
            }
            else
            {
                CameraFollow.Instance.ClearBounds();
            }

            CameraFollow.Instance.SnapToTarget();
        }

        if (SceneTransition.Instance == null)
        {
            if (PlayerMovement.Instance != null)
            {
                PlayerMovement.Instance.canMove = true;

                if (defaultSpawnPoint != null)
                    PlayerMovement.Instance.transform.position = defaultSpawnPoint.transform.position;
            }
        }
    }
}