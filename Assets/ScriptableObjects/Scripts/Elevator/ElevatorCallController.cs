using UnityEngine;
using System.Collections;
using AdventureGame.SpriteManagement;

namespace AdventureGame.ElevatorManagement
{
    /// <summary>
    /// Controls the elevator hall (outside area).
    /// Handles call buttons, waiting for the elevator, and opening the door when it arrives.
    /// </summary>
    public class ElevatorCallController : ElevatorBase
    {
        /// <summary> The sprite controller for the call button graphics. </summary>
        public SpriteController callBtn;

        [Header("On which floor is this hall?")]
        /// <summary> The floor where this hall is located. </summary>
        public int targetFloor;
        /// <summary> Internal state for call button direction. </summary>
        private enum BtnCall { NoCall, CallUp, CallDown, CallBoth }

        /// <summary> Current call button state. </summary>
        private BtnCall btnCall = BtnCall.NoCall;

        /// <summary>
        /// Sets the starting floor for the hall elevator.
        /// If the player comes from the elevator cabin scene,
        /// the elevator is placed on the correct floor and the door is opened.
        /// Otherwise, a random floor is chosen.
        /// After setting the floor, the base Awake() updates displays.
        /// </summary>
        public override void Awake()
        {
            string previousScene = GameManager.Instance.GetPreviousScene();
            if (previousScene == "Room_Elevator")
            {
                currentFloor = targetFloor;
                door.SetDoorIsOpen(true);
            }
            else
                currentFloor = Random.Range(minFloor,maxFloor+1);
            
            base.Awake();
        }
        public override void OnValidate()
        {
            base.OnValidate();

            if (callBtn == null)
                Debug.LogWarning("CallBtn is not assigned!");
        }

        // ---------------------------------
        // Interactions
        // ---------------------------------
        /// <summary> Handles door interaction from the hall hotspot. </summary>
        public override void HandleDoorInteraction(bool isTriggered)
        {
            if (movement == Movement.NotMoving)
                if(isTriggered && currentFloor == targetFloor)
                    door.Open();
        }
        /// <summary> Called when the "up" call button is pressed. </summary>
        public void HandleCallUp()
        {
            switch (btnCall)
            {
                case BtnCall.NoCall:
                    btnCall = BtnCall.CallUp;
                    break;
                case BtnCall.CallUp:
                    return;
                case BtnCall.CallDown:
                    btnCall = BtnCall.CallBoth;
                    break;
                case BtnCall.CallBoth:
                    return;
            }
            UpdateCallBtn();
            if (elevatorRoutine == null)
                elevatorRoutine = StartCoroutine(ElevatorRoutine()); 
        }
        /// <summary> Called when the "down" call button is pressed. </summary>
        public void HandleCallDown()
        {
            switch (btnCall)
            {
                case BtnCall.NoCall:
                    btnCall = BtnCall.CallDown;
                    break;
                case BtnCall.CallUp:
                    btnCall = BtnCall.CallBoth;
                    break;
                case BtnCall.CallDown:
                    return;
                case BtnCall.CallBoth:
                    return;
            }
            UpdateCallBtn();
            if (elevatorRoutine == null)
                elevatorRoutine = StartCoroutine(ElevatorRoutine()); 
        }

        /// <summary> Moves the elevator to this hall floor. </summary>
        private IEnumerator ElevatorRoutine()
        {
            if (currentFloor != targetFloor)
            {  
                door.Close();
                UpdateHotspot();
                while(door.DoorIsOpen)
                    yield return null;
                
                switch (targetFloor > currentFloor)
                {
                    case true:
                        movement = Movement.GoingUp;
                        break;
                    case false:
                        movement = Movement.GoingDown;
                        break;
                }
                UpdateDisplayArrow();
            }
            while (currentFloor != targetFloor)
            {
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
                yield return new WaitForSeconds(travelTimePerFloor/2f);
            }
            
            movement = Movement.NotMoving;

            UpdateDisplayArrow();
            door.Open();
            while(!door.DoorIsOpen)
                yield return null;
            UpdateHotspot();
            btnCall = BtnCall.NoCall;
            UpdateCallBtn();
            elevatorRoutine = null;
        }

        // ---------------------------------------------
        // Updates
        // ---------------------------------------------
        /// <summary> Updates the call button graphics based on the call state. </summary>
        private void UpdateCallBtn()
        {
            switch (btnCall)
            {
                case BtnCall.NoCall:
                    callBtn.SetSprite(0);
                    break;
                case BtnCall.CallDown:
                    callBtn.SetSprite(1);
                    break;
                case BtnCall.CallUp:
                    callBtn.SetSprite(2);
                    break;
                case BtnCall.CallBoth:
                    callBtn.SetSprite(3);
                    break;
            }
        }
        /// <summary>
        /// Enables the hotspot only when the elevator is at this hall floor.
        /// </summary>
        protected override void UpdateHotspot()
        {
            if(movement == Movement.NotMoving && currentFloor == targetFloor && door.DoorIsOpen)
                transitionHotspot.gameObject.SetActive(true);
            else
                transitionHotspot.gameObject.SetActive(false);
        }

    }
}
