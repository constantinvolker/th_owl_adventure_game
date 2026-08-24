using UnityEngine;
using UnityEngine.UI;

public class StatusLamp : MonoBehaviour
{
    public enum LampState
    {
        Off,
        Green,
        Red
    }

    public Image lampImage;

    [Header("Lamp Sprites")]
    public Sprite lampOffSprite;
    public Sprite lampGreenSprite;
    public Sprite lampRedSprite;

    void Start()
    {
        SetState(LampState.Off);
    }

    public void SetState(LampState state)
    {
        switch (state)
        {
            case LampState.Off:
                lampImage.sprite = lampOffSprite;
                break;

            case LampState.Green:
                lampImage.sprite = lampGreenSprite;
                break;

            case LampState.Red:
                lampImage.sprite = lampRedSprite;
                break;
        }
    }
}