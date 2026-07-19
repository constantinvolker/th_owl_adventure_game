using UnityEngine;

[System.Serializable]
public class DialogueOption
{
    [Tooltip("Text shown on the button.")]
    public string optionText;

    [Tooltip("Dialogue that plays when chosen. Leave empty to return to main options.")]
    public DialogueData nextDialogue;

    [Tooltip("Check this to make this option END the conversation instead of looping back.")]
    public bool isEndOption = false;
}