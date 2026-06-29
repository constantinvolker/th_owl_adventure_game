using System;
using UnityEngine;

/// <summary>
/// Manages sprite assignment and visibility for a GameObject with a SpriteRenderer.  
/// </summary>
/// 
/// <remarks>
/// <para> <b>sprites</b>: List of possible sprites for the object </para>
/// 
/// <para> <b>Methods:</b></para>
/// 
/// <para><b>SetSprite(int index)</b> – Assigns a sprite by index and activates the GameObject.</para>
///     
/// <para><b>SetRandomSprite()</b> – Selects a random sprite from the list and activates the GameObject.</para>
/// 
/// <para><b>DeactivateSprite()</b> – Removes the currently assigned sprite but keeps the object active.</para>
/// 
/// <para><b>DeactivateObject()</b> – Deactivates the entire GameObject.</para>
/// </remarks>
/// 
public class SpriteController : MonoBehaviour
{
    [Header("List of sprites that can be assigned to this object.")]
    public Sprite[] sprites;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        Init();
    }
    private void OnValidate()
    {
        Init();
    }
    private void Init()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetSprite(int index)
    {
        if (spriteRenderer != null && sprites.Length > index)
        {
            spriteRenderer.sprite = sprites[index];
        }
        gameObject.SetActive(true);
    }
    public void DeactivateSprite()
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = null;
    }
    public void DeactivateObject()
    {
        gameObject.SetActive(false);
    }
    public void SetRandomSprite()
    {
        if (spriteRenderer != null && sprites.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, sprites.Length);
            spriteRenderer.sprite = sprites[index];
        }
        gameObject.SetActive(true);
    }

}
