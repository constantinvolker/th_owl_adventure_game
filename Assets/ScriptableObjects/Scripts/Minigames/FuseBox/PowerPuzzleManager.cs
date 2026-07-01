using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class PowerPuzzleManager : MonoBehaviour
{
    [System.Serializable]
    public class Fuse
    {
        public Toggle fuseToggle;
        public int powerValue;
    }

    [Header("Hauptsicherung")]
    [Tooltip("Der Hauptschalter als Toggle")]
    public Toggle mainFuseToggle;
    public GameObject overloadWarningUI;

    [Header("Schaltkreis A")]
    public Fuse[] circuitAFuses;
    public TextMeshProUGUI displayA;
    public int targetMinA = 68;
    public int targetMaxA = 72;
    public int overloadLimitA = 80;

    [Header("Schaltkreis B")]
    public Fuse[] circuitBFuses;
    public TextMeshProUGUI displayB;
    public int targetMinB = 60;
    public int targetMaxB = 65;
    public int overloadLimitB = 75;

    [Header("Events")]
    public UnityEvent onPowerRestored;
    public UnityEvent onPowerLost;

    // Flag for other assets
    public bool IsPowerRestored { get; private set; }

    private bool isBlown = false;

    void Start()
    {
        foreach (var fuse in circuitAFuses)
            fuse.fuseToggle.onValueChanged.AddListener(delegate { OnSubFuseToggled(); });

        foreach (var fuse in circuitBFuses)
            fuse.fuseToggle.onValueChanged.AddListener(delegate { OnSubFuseToggled(); });

        mainFuseToggle.onValueChanged.AddListener(delegate { OnMainFuseToggled(); });

        UpdatePuzzleState();
    }

    public void OnSubFuseToggled()
    {
        if (isBlown && !mainFuseToggle.isOn)
        {
            isBlown = false;
        }
        UpdatePuzzleState();
    }

    public void OnMainFuseToggled()
    {
        isBlown = false;
        UpdatePuzzleState();
    }

    public void UpdatePuzzleState()
    {
        bool newPowerState = false; // Default Off
        int currentPowerA = 0;
        int currentPowerB = 0;

        foreach (var fuse in circuitAFuses)
            if (fuse.fuseToggle.isOn) currentPowerA += fuse.powerValue;

        foreach (var fuse in circuitBFuses)
            if (fuse.fuseToggle.isOn) currentPowerB += fuse.powerValue;

        // Main Off 
        if (!mainFuseToggle.isOn)
        {
            if (isBlown)
            {
                // If overload
                displayA.text = "Overload";
                displayB.text = "Overload";
                if (overloadWarningUI != null) overloadWarningUI.SetActive(true);
            }
            else
            {
                // Normal off
                displayA.text = $"0 / {targetMaxA} A";
                displayB.text = $"0 / {targetMaxB} A";
                if (overloadWarningUI != null) overloadWarningUI.SetActive(false);
            }

            UpdatePowerStatus(newPowerState);
            return; // Main off, no power
        }

        // Main ON

        if (overloadWarningUI != null) overloadWarningUI.SetActive(false);

        // If power show real values
        displayA.text = $"{currentPowerA} / {targetMaxA} A";
        displayB.text = $"{currentPowerB} / {targetMaxB} A";

        // Check overload
        if (currentPowerA > overloadLimitA || currentPowerB > overloadLimitB)
        {
            TriggerOverload();
        }
        // Check if win
        else if (currentPowerA >= targetMinA && currentPowerA <= targetMaxA &&
                 currentPowerB >= targetMinB && currentPowerB <= targetMaxB)
        {
            displayA.text = "Online";
            displayB.text = "Online";
            newPowerState = true; // Rätsel geschafft!

            // Optional: Lock main?
            // mainFuseToggle.interactable = false; 
        }

        UpdatePowerStatus(newPowerState);
    }

    // Ovserver System for other assets in scene
    private void UpdatePowerStatus(bool newState)
    {
        if (newState != IsPowerRestored)
        {
            IsPowerRestored = newState;

            if (IsPowerRestored)
                onPowerRestored?.Invoke();
            else
                onPowerLost?.Invoke();
        }
    }

    private void TriggerOverload()
    {
        isBlown = true;

        mainFuseToggle.SetIsOnWithoutNotify(false);
        UpdatePuzzleState();
    }
}


// How to read from this
//public PowerPuzzleManager puzzle;

//void Update()
//{
//    if (Input.GetKeyDown(KeyCode.E)) // Spieler drückt z.b. E am PC
//    {
//        if (puzzle.IsPowerRestored)
//        {
//            Debug.Log("PC bootet...");
//        }
//        else
//        {
//            Debug.Log("Kein Strom! Finde den Sicherungskasten.");
//        }
//    }
//}
