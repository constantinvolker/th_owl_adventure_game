using UnityEngine;
using System;
using System.Collections;
using AdventureGame.SpriteManagement;
using System.Collections.Generic;

namespace AdventureGame.Elevator
{
    public class ElevatorManager : MonoBehaviour
    {
        [Serializable]
        [RequireComponent(typeof(SpriteController))]
        public class Button
        {
            public SpriteController sprite;
            public int floor;
            public string sceneName;
        }
        public Button[] buttons;
        public ElevatorDoor door;
        public SpriteController displayNumber;
        public TransitionHotspot transitionHotspot;
        public int maxFloor = 7;
        public int minFloor = 0;
        public float travelTimePerFloor = 2f;
        
        private int currentFloor = 0;
        private readonly List<int> floorRequests = new List<int>();
        private Coroutine elevatorRoutine = null;

        private enum Movement {GoingUp, GoingDown, NotMoving}
        private Movement movement = Movement.NotMoving;


        void Awake()
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
            UpdateDisplay();
            UpdateTargetScene();
        }
        void OnValidate()
        {
            if (buttons == null || buttons.Length == 0)
                Debug.LogWarning("Buttons array is empty!");

            if (displayNumber == null)
                Debug.LogWarning("DisplayNumber is not assigned!");

            if (door == null)
                Debug.LogWarning("Door is not assigned!");
        }
        void Start()
        {
            door.Close();
        }


        // ---------------------------------
        // Interactions
        // ---------------------------------
        /// <summary>
        /// method to use when the door is clicked
        /// </summary>
        public void HandleDoorInteraction()
        {
            // if moving
            if (movement == Movement.NotMoving)
                door.Toggle();
        }
        public void HandleBtnClick(int targetFloor)
        {
            if (targetFloor < minFloor || targetFloor > maxFloor || floorRequests.Contains(targetFloor))
                return;

            floorRequests.Add(targetFloor);
            UpdateButtonSprites();
            if (elevatorRoutine == null)
                elevatorRoutine = StartCoroutine(ElevatorRoutine()); 
        
        }
        // ---------------------------------------------
        // Elevator Movement
        // ---------------------------------------------
        private IEnumerator ElevatorRoutine()
        {
            while (floorRequests.Count > 0)
            {
                if (door.DoorIsOpen)
                {
                    door.Close();
                    yield return new WaitForSeconds(door.doorOpenTime);
                }

                int nextFloor = GetNextTargetFloor();
                if (nextFloor != currentFloor)
                {
                    switch (nextFloor > currentFloor)
                    {
                        case true:
                            movement = Movement.GoingUp;
                            break;
                        case false:
                            movement = Movement.GoingDown;
                            break;

                    }
                    UpdateHotspot();

                    // moving
                    yield return new WaitForSeconds(travelTimePerFloor/2);
                    switch (movement)
                    {
                        case Movement.GoingDown:
                            currentFloor -= 1;
                            break;
                        case Movement.GoingUp:
                            currentFloor += 1;
                            break;
                    }
                    UpdateDisplay();
                    UpdateTargetScene();
                    yield return new WaitForSeconds(travelTimePerFloor/2);

                }

                // next floor reached
                if (currentFloor == nextFloor)
                {
                    floorRequests.Remove(nextFloor);
                    UpdateButtonSprites();
                    movement = Movement.NotMoving;
                    UpdateHotspot();
                    door.Open();
                    yield return new WaitForSeconds(door.doorOpenTime*2);
                }

            }

            elevatorRoutine = null;
        }

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
        private void UpdateDisplay()
        {
            displayNumber.SetSprite(currentFloor);
        }

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
        private void UpdateHotspot()
        {
            if(movement == Movement.NotMoving)
                transitionHotspot.gameObject.SetActive(true);
            else
                transitionHotspot.gameObject.SetActive(false);
        }
    }
}
