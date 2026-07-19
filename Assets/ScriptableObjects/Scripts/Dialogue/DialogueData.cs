using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Tooltip("Lines spoken before options appear.")]
    public DialogueLine[] lines;

    [Tooltip("Options shown after all lines. Leave empty for linear dialogue.")]
    public DialogueOption[] options;

    [Tooltip("If set, this dialogue plays the second time the player talks to this NPC.")]
    public DialogueData repeatingDialogue;
}