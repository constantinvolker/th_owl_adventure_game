using System.Collections;
using UnityEngine;

public class TransitionHotspot : MonoBehaviour
{
    public enum TransitionType { WalkThenFade, FadeOnly }

    [Header("Target")]
    public string targetScene;
    public string targetSpawnName;

    [Header("Transition")]
    public TransitionType transitionType = TransitionType.WalkThenFade;
    public float fadeDuration = 0f;

    [Header("Walk Position")]
    public Transform standPoint;

    [Header("Condition")]
    [Tooltip("Player must have this item before transition is allowed.")]
    [SerializeField] private ItemData requiredItem;

    [Tooltip("All items must be in inventory before transition. For packed backpack gate.")]
    [SerializeField] private ItemData[] requiredItems;

    [Tooltip("Dialogue shown when condition is not met.")]
    [SerializeField] private DialogueData blockedDialogue;

    private bool _triggered = false;

    public void Activate()
    {
        if (_triggered) return;
        if (string.IsNullOrEmpty(targetScene)) return;

        bool canPass = true;

        if (requiredItem != null)
        {
            bool hasItem = requiredItem.isBackpack
                ? InventoryManager.Instance.BackpackUnlocked
                : InventoryManager.Instance.HasItem(requiredItem);

            if (!hasItem) canPass = false;
        }

        if (requiredItems != null && requiredItems.Length > 0)
        {
            foreach (ItemData item in requiredItems)
            {

                if (item != null && !InventoryManager.Instance.HasItem(item))
                {
                    canPass = false;
                    break;
                }
            }
        }

        // Kontrolle
        if (!canPass)
        {
            if (blockedDialogue != null)
            {
                DialogueManager.Instance.PlaySimpleDialogue(blockedDialogue);
            }
            return;
        }

        _triggered = true;
        StartCoroutine(RunTransition());
    }

    public void ResetTrigger() => _triggered = false;

    private IEnumerator RunTransition()
    {
        PlayerMovement.Instance.canMove = false;

        if (transitionType == TransitionType.WalkThenFade && standPoint != null)
        {
            yield return StartCoroutine(WalkToStandPoint());
        }
        yield return StartCoroutine(
            SceneTransition.Instance.FadeOut(fadeDuration > 0 ? fadeDuration : -1f)
        );
        SceneLoader.Instance.LoadRoom(targetScene, targetSpawnName);
    }

    private IEnumerator WalkToStandPoint()
    {
        Vector3 target = standPoint.position;

        PlayerMovement.Instance.ForceMoveTo(target);

        float threshold = 0.3f;
        float timeout = 5f;
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            float dist = Vector2.Distance(
                PlayerMovement.Instance.transform.position,
                target
            );

            if (dist <= threshold)
            {
                PlayerMovement.Instance.StopMoving();
                break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (elapsed >= timeout)
        {
            PlayerMovement.Instance.StopMoving();
        }
    }
}