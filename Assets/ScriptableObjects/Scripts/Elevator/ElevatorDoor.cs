using UnityEngine;
using AdventureGame.SpriteManagement;
using System.Collections;

namespace AdventureGame.ElevatorManagement
{
    /// <summary>
    /// Controls the elevator door. 
    /// It can open, close, and play animations between the states.
    /// </summary>
    [RequireComponent(typeof(SpriteController))]
    public class ElevatorDoor : MonoBehaviour
    {
        /// <summary> The sprite controller that shows the door graphics. </summary>
        public SpriteController door;

        /// <summary> How long the door needs to fully open or close. </summary>
        public float doorOpenTime = 3f;

        /// <summary> True if the door is open. False if the door is closed. </summary>
        public bool DoorIsOpen { get; private set; }

        /// <summary> The current animation coroutine for opening or closing. </summary>
        private Coroutine coroutine = null;

        /// <summary> Internal door state used to control animations. </summary>
        private enum State { Open, Close, Closing, Opening }

        /// <summary> The current door state. </summary>
        private State state;

        /// <summary> Sets the correct door sprite when the object starts. </summary>
        void Awake()
        {
            if (DoorIsOpen)
            {
                door.SetSprite(door.sprites.Length-1);
                state = State.Open;
            }
            else
            {
                door.SetSprite(0);
                state = State.Close;
            }
        }

        /// <summary> Instantly sets the door to open or closed without animation. </summary>
        public void SetDoorIsOpen(bool doorIsOpen)
        {
            StopAnimation();

            int spriteIndex = doorIsOpen ? door.sprites.Length - 1 : 0;
            door.SetSprite(spriteIndex);

            DoorIsOpen = doorIsOpen;
            state = doorIsOpen ? State.Open : State.Close;
        }

        /// <summary> Runs the closing animation step by step. </summary>
        private IEnumerator ClosingAnimation()
        {
            state = State.Closing;
            DoorIsOpen = false;
            int currentSpriteIndex = door.GetCurrentSpriteIndex();

            for (int i = currentSpriteIndex; i >= 0; i--)
            {
                door.SetSprite(i);
                // wait only if this is NOT the last sprite
                if (i < door.sprites.Length - 1)
                    yield return new WaitForSeconds(doorOpenTime / door.sprites.Length);
            }

            state = State.Close;
            coroutine = null;
        }

        /// <summary> Runs the opening animation step by step. </summary>
        private IEnumerator OpeningAnimation()
        {
            state = State.Opening;
            int currentSpriteIndex = door.GetCurrentSpriteIndex();

            for (int i = currentSpriteIndex; i < door.sprites.Length; i++)
            {
                door.SetSprite(i);
                // wait only if this is NOT the last sprite
                if (i < door.sprites.Length - 1)
                    yield return new WaitForSeconds(doorOpenTime / door.sprites.Length);
            }

            DoorIsOpen = true;
            state = State.Open;
            coroutine = null;
        }

        /// <summary> Opens the door if it is closed, or closes it if it is open. </summary>
        public void Toggle()
        {
            if (state == State.Close || state == State.Closing)
                Open();
            else
                Close();
        }

        /// <summary> Starts the closing animation if the door is not already closing or closed. </summary>
        public void Close()
        {
            if (state == State.Close || state == State.Closing)
                return;

            StopAnimation();
            coroutine = StartCoroutine(ClosingAnimation());
        }

        /// <summary> Starts the opening animation if the door is not already opening or open. </summary>
        public void Open()
        {
            if (state == State.Open || state == State.Opening)
                return;

            StopAnimation();
            coroutine = StartCoroutine(OpeningAnimation());
        }

        /// <summary> Stops the current animation coroutine. </summary>
        private void StopAnimation()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }


}
