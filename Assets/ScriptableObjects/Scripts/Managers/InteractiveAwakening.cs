using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class InteractiveAwakening : MonoBehaviour
{
    [Header("Licht")]
    public Light2D visionLight;
    public float maxOuterRadius = 15f;

    [Header("Dialoge (DialogueData)")]
    public DialogueData[] thoughtDialogues;

    [Header("Automatischer Szenenwechsel")]
    [Tooltip("Der Name der Zielszene (z.B. Room_ApartmentBedroom)")]
    public string targetScene = "Room_ApartmentBedroom";

    [Tooltip("Der Name des Spawnpunktes in der Zielszene (z.B. Spawn_Main)")]
    public string targetSpawnName = "Spawn_Main"; // Passe das an deinen echten Spawn-Namen an!

    private int currentStep = 0;
    private float initialRadius;
    private bool isFinishing = false;
    private bool canClick = false;

    private void Start()
    {
        if (visionLight != null)
        {
            initialRadius = visionLight.pointLightOuterRadius;
        }

        // Echten Spieler im Intro unsichtbar machen
        GameObject realPlayer = GameObject.FindGameObjectWithTag("Player");
        if (realPlayer != null)
        {
            SpriteRenderer playerSprite = realPlayer.GetComponentInChildren<SpriteRenderer>();
            if (playerSprite != null)
            {
                playerSprite.enabled = false;
            }
        }

        Invoke("StartFirstDialogue", 0.1f);
    }

    private void StartFirstDialogue()
    {
        // 1. ZWANGS-AUFWECKEN DES CANVAS:
        GameObject persist = GameObject.Find("PERSISTOBJECTS(Clone)");
        if (persist == null) persist = GameObject.Find("PERSISTOBJECTS");

        if (persist != null)
        {
            Transform gameplayCanvas = persist.transform.Find("GameplayCanvas");
            if (gameplayCanvas != null)
            {
                gameplayCanvas.gameObject.SetActive(true);

                Transform inventory = gameplayCanvas.Find("InventoryUI");
                if (inventory != null) inventory.gameObject.SetActive(false);

                Transform hud = gameplayCanvas.Find("HUDController");
                if (hud != null) hud.gameObject.SetActive(false);
            }
        }

        // 2. DIALOG STARTEN:
        if (thoughtDialogues != null && thoughtDialogues.Length > 0)
        {
            DialogueManager.Instance.PlaySimpleDialogue(thoughtDialogues[0]);
            canClick = true;
        }
        else
        {
            Debug.LogWarning("ACHTUNG: Keine Dialoge im IntroManager eingetragen!");
            StartCoroutine(FinishIntro());
        }
    }

    private void Update()
    {
        if (canClick && !isFinishing)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                canClick = false;
                AdvanceAwakening();
            }
        }
    }

    private void AdvanceAwakening()
    {
        currentStep++;

        if (currentStep < thoughtDialogues.Length)
        {
            DialogueManager.Instance.PlaySimpleDialogue(thoughtDialogues[currentStep]);

            if (visionLight != null)
            {
                float progress = (float)currentStep / (thoughtDialogues.Length - 1);
                visionLight.pointLightOuterRadius = Mathf.Lerp(initialRadius, maxOuterRadius, progress);
            }

            Invoke("EnableClick", 0.2f);
        }
        else
        {
            isFinishing = true;
            StartCoroutine(FinishIntro());
        }
    }

    private void EnableClick()
    {
        canClick = true;
    }

    private IEnumerator FinishIntro()
    {
        // Das Licht flutet den Raum
        if (visionLight != null)
        {
            float t = 0;
            float currentRadius = visionLight.pointLightOuterRadius;

            while (t < 1f)
            {
                t += Time.deltaTime * 2f;
                visionLight.pointLightOuterRadius = Mathf.Lerp(currentRadius, maxOuterRadius * 3f, t);
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.5f);

        // 3. AUTOMATISCHER SZENENWECHSEL ÜBER DEINEN SCENELOADER:
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadRoom(targetScene, targetSpawnName);
        }
        else
        {
            Debug.LogError("SceneLoader nicht gefunden! Kann Szene nicht laden.");
        }
    }
}