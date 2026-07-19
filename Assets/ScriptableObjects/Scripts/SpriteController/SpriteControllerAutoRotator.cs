using UnityEngine;
using System.Collections;
using System.Linq;
namespace AdventureGame.SpriteController
{
    /// <summary>
    /// Automatically cycles through a defined sprite range on a SpriteController
    /// every X seconds. Rotation can be paused when the player hovers or clicks
    /// the object. Rotation only starts if the initial sprite is inside the
    /// configured rotation range.
    /// </summary>
    [RequireComponent(typeof(SpriteController))]
    public class SpriteControllerAutoRotator : MonoBehaviour
    {
        [Header("Time between sprite changes")]
        public int intervalSeconds = 2;

        [Header("Sprite index range used for rotation")]
        public int minIndex = 0;
        public int maxIndex = 0;

        [Header("Pauses Rotation when sprite index is:")]
        public int[] pausesRotationIndices;

        private SpriteController spriteController;
        private Coroutine rotationRoutine = null;
        private int rotationIndex = -1;
        private bool isPaused = false;

        void Awake()
        {
            spriteController = GetComponent<SpriteController>();
        }
        void Start()
        {
            StartRotation();
        }
        /// <summary>
        /// Coroutine that cycles through sprites in the configured range.
        /// Stops when sprite index is in the pausesRotationIndices list 
        /// and resumes when sprite index is in the configured range and not in the pausesRotationIndices list.
        /// </summary>
        private IEnumerator RotateSprites()
        {
            while (true)
            {
                if (!isPaused)
                {
                    if (SpriteIndexPausesRotation())
                        PauseRotation();
                    else
                        SetNextSprite();
                }
                else
                    if (IsInRange(spriteController.GetCurrentSpriteIndex()) && !SpriteIndexPausesRotation())
                        ResumeRotation();

                yield return new WaitForSeconds(intervalSeconds);
            }
        }
        private bool RotationIsActive()
        {
            return rotationRoutine != null;
        }
        /// <summary>
        /// Returns whether the index is in the configured index range (minIndex, maxIndex).
        /// </summary>
        private bool IsInRange(int index)
        {
            return index >= minIndex && index <= maxIndex;
        }
        /// <summary>
        /// Returns whether the sprite index is in the pausesRotationIndices list.
        /// </summary>
        private bool SpriteIndexPausesRotation()
        {
            int spriteIndex = spriteController.GetCurrentSpriteIndex();
            return pausesRotationIndices.Contains(spriteIndex);
        }
        /// <summary>
        /// Sets the next sprite.
        /// </summary>
        private void SetNextSprite()
        {
            rotationIndex++;
            if (rotationIndex > maxIndex)
                rotationIndex = minIndex;
            spriteController.SetSprite(rotationIndex);
        }
        /// <summary>
        /// If the sprite has an index within the range, that one is used; 
        /// if the current index is within the range, that one is used; 
        /// otherwise, the range minimum is used.
        /// </summary>
        private void InitIndexInRange()
        {
            int spriteIndex = spriteController.GetCurrentSpriteIndex();
            if (IsInRange(spriteIndex))
                rotationIndex = spriteIndex;
            else if (!IsInRange(rotationIndex))
                rotationIndex = minIndex;
        }
        /// <summary>
        /// Starts the rotation only if there is no rotation.
        /// </summary>
        public void StartRotation()
        {
            if(!RotationIsActive())
            {
                InitIndexInRange();
                rotationRoutine = StartCoroutine(RotateSprites());
            }
        }
        /// <summary>
        /// Stops the rotation only if there is an active rotation.
        /// </summary>
        public void StopRotation()
        {
            if (RotationIsActive())
            {
                StopCoroutine(rotationRoutine);
                rotationRoutine = null;
            }
        }
        /// <summary>
        /// Pauses the rotation. (Rotation is not stopped by this.)
        /// </summary>
        public void PauseRotation()
        {
            isPaused = true;
        }
        /// <summary>
        /// Resumes the rotation. (Rotation is not started by this.)
        /// </summary>
        public void ResumeRotation()
        {
            isPaused = false;
        }
        /// <summary>
        /// Stops the current the rotation when exists and starts a new rotation.
        /// </summary>
        public void RestartRotation()
        {
            StopRotation();
            StartRotation();
        }
        /// <summary>
        /// Sets the next sprite and restart the rotation,
        /// only when sprite index is not in the pausesRotationIndices list.
        /// </summary>
        public void NextSpriteWithRotationRestart()
        {
            if (SpriteIndexPausesRotation())
                return;
            SetNextSprite();
            RestartRotation();
        }

    }
}