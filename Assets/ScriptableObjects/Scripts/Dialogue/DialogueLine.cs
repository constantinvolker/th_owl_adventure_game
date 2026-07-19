using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [Tooltip("Assign a SpeakerData asset — name and color come from there.")]
    public SpeakerData speaker;

    [TextArea(2, 6)]
    public string text;

    // Convenience accessors used by DialogueUI
    public string speakerName  => speaker != null ? speaker.speakerName  : "???";
    public Color  speakerColor => speaker != null ? speaker.speakerColor : Color.white;
}