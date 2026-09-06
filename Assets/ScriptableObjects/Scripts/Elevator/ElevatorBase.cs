using UnityEngine;
using AdventureGame.SpriteManagement;

namespace AdventureGame.ElevatorManagement
{
    /// <summary>
    /// Base class for all elevator types.
    /// It contains shared logic for doors, displays, movement and hotspots.
    /// </summary>
    public abstract class ElevatorBase : MonoBehaviour
    {
        /// <summary> The elevator door object. </summary>
        public ElevatorDoor door;

        /// <summary> The number display that shows the current floor. </summary>
        public SpriteController displayNumber;

        /// <summary> The arrow display that shows the movement direction. </summary>
        public SpriteController displayArrow;

        /// <summary> The hotspot used to change scenes when entering the elevator. </summary>
        public TransitionHotspot transitionHotspot;

        /// <summary> The highest floor the elevator can reach. </summary>
        public int maxFloor = 7;

        /// <summary> The lowest floor the elevator can reach. </summary>
        public int minFloor = 0;

        /// <summary> Time needed to move one floor. </summary>
        public float travelTimePerFloor = 2f;

        /// <summary> The current floor the elevator is on. </summary>
        protected int currentFloor = 0;

        /// <summary> The active movement coroutine. </summary>
        protected Coroutine elevatorRoutine = null;

        /// <summary> Possible movement directions. </summary>
        protected enum Movement { GoingUp, GoingDown, NotMoving }

        /// <summary> The current movement direction. </summary>
        protected Movement movement = Movement.NotMoving;

        /// <summary>
        /// Called when the elevator object is created.
        /// Updates the floor number display and the arrow display.
        /// Child classes can add more logic by overriding this method.
        /// </summary>
        public virtual void Awake()
        {
            UpdateDisplayNumber();
            UpdateDisplayArrow();
        }

        /// <summary> 
        /// Called when the elevator starts. Closes the door and updates the hotspot.
        /// </summary>
        public virtual void Start()
        {
            door.Close();
            UpdateHotspot();
        }

        /// <summary>
        /// Warns in the editor if important references are missing.
        /// </summary>
        public virtual void OnValidate()
        {
            if (displayNumber == null)
                Debug.LogWarning("DisplayNumber is not assigned!");

            if (door == null)
                Debug.LogWarning("Door is not assigned!");
        }

        // ---------------------------------
        // Interactions
        // ---------------------------------
        /// <summary>
        /// Called when the hotspot is triggered. Must be implemented by child classes.
        /// </summary>
        public abstract void HandleDoorInteraction(bool isTriggered);

        // ---------------------------------------------
        // Updates
        // ---------------------------------------------
        /// <summary>
        /// Updates the number display to show the current floor.
        /// </summary>
        protected void UpdateDisplayNumber()
        {
            displayNumber.SetSprite(currentFloor);
        }
        /// <summary>
        /// Updates the arrow display to show the movement direction.
        /// </summary>
        protected void UpdateDisplayArrow()
        {
            switch (movement)
            {
                case Movement.NotMoving:
                    displayArrow.DeactivateObject();
                    break;
                case Movement.GoingUp:
                    displayArrow.SetSprite(0);
                    break;
                case Movement.GoingDown:
                    displayArrow.SetSprite(1);
                    break;
            }
        }
        /// <summary>
        /// Enables or disables the hotspot.
        /// Must be implemented by child classes.
        /// </summary>
        protected abstract void UpdateHotspot();
    }
}
