using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Switches the sprite's sorting layer between background and foreground.
/// Used in the Room_Stairway scene on the LayerToggleStairs object.
/// 
/// ForegroundZone and BackgroundZone can switch the sorting layer externally,
/// but this component can also switch layers automatically if the object
/// itself has a trigger collider. Entering the collider switches to the
/// foreground layer, leaving it switches back to the background layer.
/// 
/// Additionally, this script can toggle separate colliders enabled state
/// when switching layers.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class LayerToggle : MonoBehaviour
{
    [Header("Sorting Layer Settings")]
    public string backgroundSortingLayer = "Background";
    public string foregroundSortingLayer = "Fade";

    [Header("Temporary Walls to toggle")]
    [Tooltip("Optional. Leave empty if no foreground wall exists.")]
    public GameObject WallInForeground;
    [Tooltip("Optional. Leave empty if no background wall exists.")]
    public GameObject WallInBackground;

    private SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    public void TriggerBackground()
    {
        if (_sr != null)
            _sr.sortingLayerName = backgroundSortingLayer;
        if (WallInForeground != null)
            WallInForeground.SetActive(true);
        if (WallInBackground != null)
            WallInBackground.SetActive(false);
    }

    public void TriggerForeground()
    {
        if (_sr != null)
            _sr.sortingLayerName = foregroundSortingLayer;
        if (WallInForeground != null)
            WallInForeground.SetActive(false);
        if (WallInBackground != null)
            WallInBackground.SetActive(true);
    }
    
    // automatic switching if this object has its own trigger collider
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            TriggerForeground();
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            TriggerForeground();
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            TriggerBackground();
    }

}
