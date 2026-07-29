using UnityEngine;
using AdventureGame.SpriteManagement;
using System.Collections;

namespace AdventureGame.Elevator
{
    [RequireComponent(typeof(SpriteController))]
    public class ElevatorDoor : MonoBehaviour
    {
        public SpriteController door;
        public float doorOpenTime = 3f;

        public bool DoorIsOpen;
        private Coroutine coroutine = null;


        void Awake()
        {
            if (DoorIsOpen)
                door.SetSprite(door.sprites.Length);
            else
                door.SetSprite(0);
        }

        private IEnumerator CloseAnimation()
        {
            if (door.GetCurrentSpriteIndex() == door.sprites.Length - 1)
            {
                for (int i = door.sprites.Length - 1; i >= 0; i--)
                {
                    door.SetSprite(i);
                    yield return new WaitForSeconds(doorOpenTime / door.sprites.Length);
                }
                DoorIsOpen = false;
            }

            coroutine = null;
        }


        private IEnumerator OpenAnimation()
        {
            if (door.GetCurrentSpriteIndex() == 0)
            {
                for (int i = 0; i < door.sprites.Length; i++)
                {
                    door.SetSprite(i);
                    yield return new WaitForSeconds(doorOpenTime / door.sprites.Length);
                }

                DoorIsOpen = true;
            }
            coroutine = null;
        }

        public void Toggle()
        {
            if (coroutine == null)
            {
                if (DoorIsOpen)
                    coroutine = StartCoroutine(CloseAnimation());
                else
                    coroutine = StartCoroutine(OpenAnimation());
            }
        }
        public void Close()
        {
            if (DoorIsOpen)
                Toggle();
        }
        public void Open()
        {
            if (!DoorIsOpen)
                Toggle();
        }
    }

}
