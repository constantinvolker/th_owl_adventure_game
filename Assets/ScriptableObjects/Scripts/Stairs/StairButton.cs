//stairs butten is used to run stairs up or down

using UnityEngine;

public class StairButton : MonoBehaviour
{
    [SerializeField] private StairPath stair;
    [SerializeField] private bool upwards;

    public void Activate()
    {
        if (stair == null)
        {
            Debug.LogWarning($"Bei {name} wurde keine StairPath zugewiesen.");
            return;
        }

        PlayerMovement.Instance.UseStair(stair, upwards);
    }
}