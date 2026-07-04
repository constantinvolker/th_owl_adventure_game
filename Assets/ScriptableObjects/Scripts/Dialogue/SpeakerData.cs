using UnityEngine;

/// Create one asset per character. Assign to DialogueLine instead of
/// typing name and color every time.
/// Create via: Right-click -> Dialogue -> Speaker Data
[CreateAssetMenu(fileName = "Speaker_New", menuName = "Dialogue/Speaker Data")]
public class SpeakerData : ScriptableObject
{
    public string      speakerName;
    public Color       speakerColor = Color.white;
}