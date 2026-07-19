using UnityEngine;

/// Steuert die Player-Animation ohne Blend Tree.
/// Weist je nach Bewegungsrichtung direkt den richtigen Walk-Clip zu,
/// bei Stillstand den einzigen Idle-Clip.
///
/// Animator-Setup (nur States, KEINE Blend Trees, KEINE Transitions):
///   States:    Idle | Walk_Down | Walk_Up | Walk_Left | Walk_Right
///   Parameter: keine nötig – alles wird per script via Play() gesteuert.
///   Alle States stehen direkt in der Base-Layer, keinerlei Transitions.
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("Animator State Names")]
    [SerializeField] private string idleState      = "Idle";
    [SerializeField] private string walkDownState  = "Walk_Down";
    [SerializeField] private string walkUpState    = "Walk_Up";
    [SerializeField] private string walkLeftState  = "Walk_Left";
    [SerializeField] private string walkRightState = "Walk_Right";

    private Animator       _animator;
    private PlayerMovement _movement;

    private string _currentState = "";

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        bool isMoving = _movement != null && _movement.IsMoving();

        if (!isMoving)
        {
            Play(idleState);
            return;
        }

        Vector2 toTarget = (Vector2)(_movement.TargetPos - transform.position);
        if (toTarget.sqrMagnitude < 0.001f) return;

        // Dominante Achse bestimmt die Richtung
        string next;
        if (Mathf.Abs(toTarget.x) >= Mathf.Abs(toTarget.y))
            next = toTarget.x > 0 ? walkRightState : walkLeftState;
        else
            next = toTarget.y > 0 ? walkUpState : walkDownState;

        Play(next);
    }

    /// Wechselt den State nur wenn er sich geändert hat (verhindert Restart mid-clip).
    private void Play(string state)
    {
        if (state == _currentState) return;
        _currentState = state;
        _animator.Play(state);
    }
}