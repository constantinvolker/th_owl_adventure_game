using UnityEngine;

/// Attach to any GameObject in a scene.
/// Automatically starts a dialogue when the scene loads.
/// Use for opening cutscenes, NPC greetings, tutorial starts.
public class SceneIntro : MonoBehaviour
{
    [Tooltip("Dialogue that plays automatically when scene starts.")]
    [SerializeField] private DialogueTrigger introTrigger;

    [Tooltip("Delay in seconds before dialogue starts.")]
    [SerializeField] private float delay = 0.5f;

    void Start()
    {
        if (introTrigger == null) return;
        Invoke(nameof(StartIntro), delay);
    }

    private void StartIntro()
    {
        introTrigger.StartDialogue();
    }
}