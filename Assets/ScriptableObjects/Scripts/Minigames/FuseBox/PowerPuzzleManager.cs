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

    [Header("Status Lampe")]
    public Image statusLamp;
    public Sprite lampOffSprite;
    public Sprite lampGreenSprite;
    public Sprite lampRedSprite;

    [Header("Schaltkreis A")]
    public Fuse[] circuitAFuses;
    public TextMeshProUGUI displayA;
    public RectTransform needleA;
    public int targetMinA = 68;
    public int targetMaxA = 72;
    public int overloadLimitA = 80;

    [Header("Schaltkreis B")]
    public Fuse[] circuitBFuses;
    public TextMeshProUGUI displayB;
    public RectTransform needleB;
    public int targetMinB = 60;
    public int targetMaxB = 65;
    public int overloadLimitB = 75;

    [Header("Analoganzeige")]
    public float needleSmoothTime = 0.25f;

    [Header("Events")]
    public UnityEvent onPowerRestored;
    public UnityEvent onPowerLost;

    // Flag for other assets
    public bool IsPowerRestored { get; private set; }

    private bool isBlown = false;

    private const float needleZeroAngle = 73f;
    private const float needleMaxAngle = -73f;

    private float currentNeedleAngleA;
    private float currentNeedleAngleB;
    private float targetNeedleAngleA;
    private float targetNeedleAngleB;
    private float needleVelocityA;
    private float needleVelocityB;

    void Start()
    {
        foreach (var fuse in circuitAFuses)
            fuse.fuseToggle.onValueChanged.AddListener(delegate { OnSubFuseToggled(); });

        foreach (var fuse in circuitBFuses)
            fuse.fuseToggle.onValueChanged.AddListener(delegate { OnSubFuseToggled(); });

        mainFuseToggle.onValueChanged.AddListener(delegate { OnMainFuseToggled(); });

        // Start needles at zero
        currentNeedleAngleA = needleZeroAngle;
        currentNeedleAngleB = needleZeroAngle;
        targetNeedleAngleA = needleZeroAngle;
        targetNeedleAngleB = needleZeroAngle;

        if (needleA != null)
            needleA.localRotation = Quaternion.Euler(0f, 0f, currentNeedleAngleA);

        if (needleB != null)
            needleB.localRotation = Quaternion.Euler(0f, 0f, currentNeedleAngleB);

        UpdatePuzzleState();
    }

    void Update()
    {
        // Animate needles
        currentNeedleAngleA = Mathf.SmoothDampAngle(
            currentNeedleAngleA,
            targetNeedleAngleA,
            ref needleVelocityA,
            needleSmoothTime
        );

        currentNeedleAngleB = Mathf.SmoothDampAngle(
            currentNeedleAngleB,
            targetNeedleAngleB,
            ref needleVelocityB,
            needleSmoothTime
        );

        if (needleA != null)
            needleA.localRotation = Quaternion.Euler(0f, 0f, currentNeedleAngleA);

        if (needleB != null)
            needleB.localRotation = Quaternion.Euler(0f, 0f, currentNeedleAngleB);
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

        // Show maximum values
        displayA.text = $"{overloadLimitA} A";
        displayB.text = $"{overloadLimitB} A";

        // Main Off 
        if (!mainFuseToggle.isOn)
        {
            // No power
            UpdateNeedleTargets(0, 0);
            SetLampSprite(lampOffSprite);

            if (isBlown)
            {
                // If overload
                if (overloadWarningUI != null) overloadWarningUI.SetActive(true);
            }
            else
            {
                // Normal off
                if (overloadWarningUI != null) overloadWarningUI.SetActive(false);
            }

            UpdatePowerStatus(newPowerState);
            return; // Main off, no power
        }

        // Main ON

        if (overloadWarningUI != null) overloadWarningUI.SetActive(false);

        // Red until puzzle is finished
        SetLampSprite(lampRedSprite);

        // Move needles to real values
        UpdateNeedleTargets(currentPowerA, currentPowerB);

        // Check overload
        if (currentPowerA > overloadLimitA || currentPowerB > overloadLimitB)
        {
            TriggerOverload();
        }
        // Check if win
        else if (currentPowerA >= targetMinA && currentPowerA <= targetMaxA &&
                 currentPowerB >= targetMinB && currentPowerB <= targetMaxB)
        {
            SetLampSprite(lampGreenSprite);
            newPowerState = true; // Rätsel geschafft!

            // Optional: Lock main?
            // mainFuseToggle.interactable = false; 
        }

        UpdatePowerStatus(newPowerState);
    }

    private void UpdateNeedleTargets(int currentPowerA, int currentPowerB)
    {
        targetNeedleAngleA = CalculateNeedleAngle(currentPowerA, overloadLimitA);
        targetNeedleAngleB = CalculateNeedleAngle(currentPowerB, overloadLimitB);
    }

    private float CalculateNeedleAngle(int currentPower, int overloadLimit)
    {
        // Keep needle between zero and maximum
        float clampedPower = Mathf.Clamp(currentPower, 0, overloadLimit);
        float normalizedPower = clampedPower / overloadLimit;

        return Mathf.Lerp(needleZeroAngle, needleMaxAngle, normalizedPower);
    }

    private void SetLampSprite(Sprite lampSprite)
    {
        if (statusLamp != null)
            statusLamp.sprite = lampSprite;
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