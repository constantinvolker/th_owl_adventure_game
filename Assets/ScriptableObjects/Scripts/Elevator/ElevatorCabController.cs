using UnityEngine;
using System;
using System.Collections;
using AdventureGame.SpriteManagement;
using System.Collections.Generic;

namespace AdventureGame.ElevatorManagement
{
    /// <summary>
    /// Controls the elevator cabin. 
    /// Handles floor buttons, movement between floors, and door behavior inside the cabin.
    /// </summary>
    public class ElevatorCabController : ElevatorBase
    {
        /// <summary>
        /// A button inside the elevator cabin. 
        /// Each button has a floor number and a target scene name.
        /// </summary>
        [Serializable]
        public class Button
        {
            public SpriteController sprite;
            public int floor;
            public string sceneName;
        }
        /// <summary> All floor buttons inside the elevator cabin. </summary>
        public Button[] buttons;

        /// <summary> A list of floors that the elevator should move to. </summary>
        private readonly List<int> floorRequests = new List<int>();

        /// <summary>
        /// Sets the starting floor inside the elevator cabin.
        /// The floor is chosen based on the scene the player came from.
        /// The door is opened, and the target scene and button states are updated.
        /// After that, the base Awake() updates displays.
        /// </summary>
        public override void Awake()
        {
            string previousScene = GameManager.Instance.GetPreviousScene();
            foreach (var btn in buttons)
            {
                if (previousScene == btn.sceneName)
                {
                    currentFloor = btn.floor;
                    break;
                }
            }
            door.SetDoorIsOpen(true);

            UpdateTargetScene();
            UpdateButtonSprites();

            base.Awake();
        }
        public override void OnValidate()
        {
            base.OnValidate();

            if (buttons == null || buttons.Length == 0)
                Debug.LogWarning("Buttons array is empty!");
        }

        // ---------------------------------
        // Interactions
        // ---------------------------------
        /// <summary>
        /// Handles door interaction from the hotspot inside the cabin.
        /// </summary>
        public override void HandleDoorInteraction(bool isTriggered)
        {
            if (movement == Movement.NotMoving)
                if(isTriggered)
                    door.Open();
        }
        /// <summary>
        /// Called when a floor button is pressed. Adds the floor to the request list.
        /// </summary>
        public void HandleBtnClick(int targetFloor)
        {
            if (targetFloor < minFloor || targetFloor > maxFloor || floorRequests.Contains(targetFloor))
                return;

            floorRequests.Add(targetFloor);
            UpdateButtonSprites();
            if (elevatorRoutine == null)
                elevatorRoutine = StartCoroutine(ElevatorRoutine()); 
        
        }
        /// <summary>
        /// Moves the elevator to requested floors one by one.
        /// </summary>
        private IEnumerator ElevatorRoutine()
        {
            while (floorRequests.Count > 0)
            {
                int nextFloor = GetNextTargetFloor();
                if (nextFloor != currentFloor)
                {
                    door.Close();
                    UpdateHotspot();
                    while(door.DoorIsOpen)
                        yield return null;

                    switch (nextFloor > currentFloor)
                    {
                        case true:
                            movement = Movement.GoingUp;
                            break;
                        case false:
                            movement = Movement.GoingDown;
                            break;

                    }
                    UpdateDisplayArrow();

                    // moving
                    yield return new WaitForSeconds(travelTimePerFloor/2f);
                    switch (movement)
                    {
                        case Movement.GoingDown:
                            currentFloor -= 1;
                            break;
                        case Movement.GoingUp:
                            currentFloor += 1;
                            break;
                    }
                    UpdateDisplayNumber();
                    UpdateTargetScene();
                    yield return new WaitForSeconds(travelTimePerFloor/2f);

                }

                // next floor reached
                if (currentFloor == nextFloor)
                {
                    floorRequests.Remove(nextFloor);
                    movement = Movement.NotMoving;
                    UpdateDisplayArrow();
                    door.Open();
                    while(!door.DoorIsOpen)
                        yield return null;
                    UpdateHotspot();
                    UpdateButtonSprites();
                    yield return new WaitForSeconds(door.doorOpenTime);
                }

            }

            elevatorRoutine = null;
        }
        /// <summary>
        /// Finds the closest requested floor.
        /// </summary>
        private int GetNextTargetFloor()
        {
            // nearest floor
            int best = floorRequests[0];
            int bestDist = Mathf.Abs(best - currentFloor);

            foreach (int f in floorRequests)
            {
                int d = Mathf.Abs(f - currentFloor);
                if (d < bestDist)
                {
                    best = f;
                    bestDist = d;
                }
            }
            return best;
        }

        // ---------------------------------------------
        // Updates
        // ---------------------------------------------
        /// <summary>
        /// Updates button graphics to show which floors are requested.
        /// </summary>
        private void UpdateButtonSprites()
        {
            foreach (var btn in buttons)
            {
                int spriteIndex = 0;
                if (floorRequests.Contains(btn.floor)) 
                    spriteIndex = 1;                

                btn.sprite.SetSprite(spriteIndex);
            }
        }
        /// <summary>
        /// Updates the target scene based on the current floor.
        /// </summary>
        private void UpdateTargetScene()
        {
            foreach (var btn in buttons)
            {
                if (currentFloor == btn.floor)
                {
                    transitionHotspot.targetScene = btn.sceneName;
                    break;
                }
            }
        }
        /// <summary>
        /// Enables or disables the hotspot depending on movement.
        /// </summary>
        protected override void UpdateHotspot()
        {
            if(movement == Movement.NotMoving && door.DoorIsOpen)
                transitionHotspot.gameObject.SetActive(true);
            else
                transitionHotspot.gameObject.SetActive(false);
        }
    }
}
