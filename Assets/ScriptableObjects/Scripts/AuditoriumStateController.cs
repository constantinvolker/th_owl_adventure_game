using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controls the visual seating state of the auditorium by assigning sprites to seats
/// and activating NPCs based on the number of taken seats.
/// </summary>
/// <remarks>
/// <para>
/// <b>takenSeats</b>:  
/// Defines how many seats should be marked as occupied.  
/// The value is clamped between 0 and the total number of available seats.
/// </para>
/// 
/// <para> - reads all <see cref="SpriteController"/> components under <b>seatsRoot</b> and <b>npcsRoot</b>
/// </para>
/// 
/// <para> - Randomly selects which seats are occupied and assigns NPC sprites.
/// </para>
/// </remarks>
public class AuditoriumStateController : MonoBehaviour
{
    private int totalSeats;
    [Header("Number of chairs taken by NPCs")]
    [Range(0,64)]
    public int takenSeats;
    public Transform npcsRoot;
    private SpriteController[] npcs;
    public Transform seatsRoot;
    private SpriteController[] seats;
    private void Init()
    {
        if (seatsRoot != null)
        {
            seats = seatsRoot.GetComponentsInChildren<SpriteController>(true);
            totalSeats = seats.Length;
        }
        if (npcsRoot != null)
            npcs = npcsRoot.GetComponentsInChildren<SpriteController>(true);
    }
    private void Awake()
    {
        Init();
        ApplyState();
    }
    private void ApplyState()
    {
        takenSeats = Mathf.Clamp(takenSeats, 0, totalSeats);
        if (takenSeats == 0) {
            foreach (var seat in seats)
                seat.SetSprite(1);  // chair up
            foreach (var npc in npcs)
                npc.DeactivateObject();
            return;
        }
        else if (takenSeats >= totalSeats)
        {
            foreach (var seat in seats)
                seat.SetSprite(0);  // chair down
            foreach (var npc in npcs)
                npc.SetRandomSprite();
            return;
        }
        SetTakenSeats();

    }
    private void SetTakenSeats()
    {
        List<int> indices = new();
        for (int i = 0; i < totalSeats; i++)
            indices.Add(i);
    
        // shuffle
        for (int i = 0; i < indices.Count; i++)
        {
            int swap = Random.Range(i, indices.Count);
            (indices[i], indices[swap]) = (indices[swap], indices[i]);
        }

        // deactivate all
        for (int i = 0; i < totalSeats; i++)
        {
            seats[i].SetSprite(1);      // chair up
            npcs[i].DeactivateObject();
        }
    
        // activate 
        for (int i = 0; i < takenSeats; i++)
        {
            int idx = indices[i];
            seats[idx].SetSprite(0);      // chair down
            npcs[idx].SetRandomSprite();
        }
    }
}
