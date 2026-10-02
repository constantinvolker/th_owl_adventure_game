using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    [Header("Objekt-Identifikation")]
    public string objectID; // Einzigartiger Name für dieses Objekt (z.B. "Schluessel", "Münze")

    [Header("Einstellungen")]
    public float snapDistance = 1.5f;

    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 startPosition;
    private PuzzleSlot currentSlot = null; // Das Feld, auf dem es gerade liegt

    void OnMouseDown()
    {
        //startPosition = transform.position;
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;

        // Wenn es von einem Feld runtergezogen wird, das Feld wieder freigeben
        if (currentSlot != null)
        {
            currentSlot.currentPlacedObjectID = "";
            currentSlot = null;
            PuzzleManager.Instance.CheckPuzzleCondition(); // Manager informieren
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    void OnMouseUp()
    {
        startPosition = transform.position;
        isDragging = false;
        CheckSnapping();
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Mathf.Abs(Camera.main.transform.position.z);
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void CheckSnapping()
    {
        // Finde alle Felder in der Szene
        PuzzleSlot[] allSlots = FindObjectsByType<PuzzleSlot>(FindObjectsSortMode.None);
        PuzzleSlot closestSlot = null;
        float closestDistance = snapDistance;

        foreach (PuzzleSlot slot in allSlots)
        {
            // Prüfen, ob das Feld bereits von einem ANDEREN Objekt besetzt ist
            if (slot.currentPlacedObjectID != "" && slot.currentPlacedObjectID != objectID)
                continue;

            float distance = Vector3.Distance(transform.position, slot.transform.position);
            if (distance < closestDistance)
            {
                /*if (slot.currentPlacedObjectID != "" && slot.currentPlacedObjectID != objectID)
                {
                    slot.transform.position
                }*/
                closestDistance = distance;
                closestSlot = slot;
            }
        }

        // Wenn ein freies Feld nah genug ist -> Einrasten (Egal ob richtig oder falsch!)
        if (closestSlot != null)
        {
            transform.position = closestSlot.transform.position;
            transform.rotation = closestSlot.transform.rotation;
            currentSlot = closestSlot;
            currentSlot.currentPlacedObjectID = objectID; // Dem Feld sagen, wer hier liegt
        }
        else
        {
            // Kein Feld in der Nähe -> Zurück zur letzten Startposition vor dem Ziehen
            transform.position = startPosition;
        }

        // Dem Manager sagen, dass sich etwas geändert hat
        PuzzleManager.Instance.CheckPuzzleCondition();
    }
}
