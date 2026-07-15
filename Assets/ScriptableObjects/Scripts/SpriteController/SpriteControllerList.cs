using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages a collection of SpriteControllers and applies configuration rules to them.
/// </summary>
[Serializable]
public class SpriteControllerList : MonoBehaviour
{
    public SpriteController[] spriteControllers;

    [Header("Configurationss for Sprite Controller List according to Day State")]
    public SpriteControllerConfig[] configurations;

    /// <summary>
    /// Applies the given configuration to all SpriteControllers in the list,
    /// including sprite assignment and activation behavior.
    /// </summary>
    public void ApplyConfiguration(SpriteControllerConfig config)
    {
        foreach (var spriteController in spriteControllers)
        {
            if (config.fromAllSprites)
                spriteController.SetRandomSprite();
            else
                spriteController.SetSprite(UnityEngine.Random.Range(
                    config.SpriteIndexRange.min, config.SpriteIndexRange.max + 1
                ));
        }

        List<int> indices = GetRandomIndices(spriteControllers.Length);

        switch (config.activation)
        {
            case SpriteControllerConfig.Activation.ActivateAll:
                ActivateAll(spriteControllers);
                break;
            case SpriteControllerConfig.Activation.DeactivateAll:
                DeactivateAll(spriteControllers);
                break;
            case SpriteControllerConfig.Activation.ActivateRandom:
                ActivatePercentage(spriteControllers,UnityEngine.Random.Range(0,100),indices);
                break;
            case SpriteControllerConfig.Activation.ActivateLow:
                ActivatePercentage(spriteControllers,UnityEngine.Random.Range(5,35),indices);
                break;
            case SpriteControllerConfig.Activation.ActivateMedium:
                ActivatePercentage(spriteControllers,UnityEngine.Random.Range(35,65),indices);
                break;
            case SpriteControllerConfig.Activation.ActivateHigh:
                ActivatePercentage(spriteControllers,UnityEngine.Random.Range(65,95),indices);
                break;
        }        
    }
    /// <summary>
    /// Generates a shuffled list of indices used for randomized activation patterns.
    /// </summary>
    protected List<int> GetRandomIndices(int count)
    {
        List<int> indices = new();
        for (int i = 0; i < count; i++)
            indices.Add(i);

        // shuffle
        for (int i = 0; i < count; i++)
        {
            int swap = UnityEngine.Random.Range(i, count);
            (indices[i], indices[swap]) = (indices[swap], indices[i]);
        }

        return indices;
    }
    /// <summary>
    /// Deactivates all SpriteControllers in the provided list.
    /// </summary>
    protected void DeactivateAll(SpriteController[] list)
    {
        foreach(var i in list)
            i.DeactivateObject();
    }
    /// <summary>
    /// Activates all SpriteControllers in the provided list.
    /// </summary>
    protected void ActivateAll(SpriteController[] list)
    {
        foreach(var i in list)
            i.ActivateObject();
    }
    /// <summary>
    /// Activates a percentage of SpriteControllers based on the given activation percentage
    /// and optional pre-shuffled index list.
    /// </summary>
    protected void ActivatePercentage(SpriteController[] list, int activationPercentage = 100, List<int> indices = null)
    {
        int len = list.Length;
        int activationNumber = Mathf.Clamp(activationPercentage, 0, 100) * len / 100 ;

        List<int> indexList;
        if (indices == null)
            indexList = GetRandomIndices(len);
        else
            indexList = indices;

        DeactivateAll(list);
        // activate 
        for (int i = 0; i < activationNumber; i++)
        {
            int idx = indexList[i];
            list[idx].ActivateObject();
        }
    }
}
