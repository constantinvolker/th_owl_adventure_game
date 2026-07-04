using UnityEngine;

public class PuzzleSlot : MonoBehaviour
{
    [Header("Rätsel-Logik")]
    public string correctObjectID; // Die ID/Name des Objekts, das hierhin gehört

    [HideInInspector]
    public string currentPlacedObjectID = ""; // ID des Objekts, das aktuell hier liegt

    // Prüft, ob das richtige Objekt auf diesem Feld liegt
    public bool IsCorrect()
    {
        return currentPlacedObjectID == correctObjectID;
    }
}
