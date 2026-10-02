using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    [Header("Rätsel Status Variable")]
    public bool isPuzzleSolved = false; // Diese Variable kannst du auslesen!
    public string errorSlot;
    private PuzzleSlot[] allSlots;

    void Awake()
    {
        // Singleton-Pattern, damit die Teile den Manager leicht finden
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        errorSlot = "TargetResistor";
        // Sucht automatisch alle Felder in der Szene
        allSlots = FindObjectsByType<PuzzleSlot>(FindObjectsSortMode.None);
    }

    // Wird von den Objekten aufgerufen, sobald eines losgelassen oder bewegt wird
    public void CheckPuzzleCondition()
    {
        bool allCorrect = true;

        foreach (PuzzleSlot slot in allSlots)
        {
            if (!slot.IsCorrect())
            {
                errorSlot = slot.name;
                allCorrect = false;
                break; // Ein falsches Feld reicht, um abzubrechen
            }
        }

        isPuzzleSolved = allCorrect;

        if (isPuzzleSolved)
        {
            Debug.Log("Rätsel gelöst! Die Variable 'isPuzzleSolved' ist jetzt TRUE.");
            // Hier könntest du z.B. eine Tür öffnen oder Sound abspielen
        }
        else
        {
            Debug.Log("Rätsel noch unvollständig oder falsch.");
        }
    }
}
