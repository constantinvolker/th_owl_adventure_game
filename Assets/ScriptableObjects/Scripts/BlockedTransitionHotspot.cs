using UnityEngine;

/// <summary>
/// Ersatz für TransitionHotspot bei gesperrten Räumen.
/// → Zeigt den Blocked-Cursor
/// → Bei Klick: spielt den Dialog des zugewiesenen NPCs ab
///
/// SETUP in der Entrance Hall:
///   1. Collider2D auf dem Türen-Hotspot lassen (gleiche Position wie vorher)
///   2. TransitionHotspot entfernen, dieses Skript stattdessen hinzufügen
///   3. "Blocked Npc" = den NPC der daneben steht (mit DialogueTrigger)
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BlockedTransitionHotspot : MonoBehaviour
{
    [Tooltip("Der NPC der erklärt warum der Raum geschlossen ist")]
    [SerializeField] private DialogueTrigger blockedNpc;

    /// Wird von ClickController aufgerufen (gleiche Schnittstelle wie andere Hotspots)
    public void Activate()
    {
        blockedNpc?.StartDialogue();
    }
}
