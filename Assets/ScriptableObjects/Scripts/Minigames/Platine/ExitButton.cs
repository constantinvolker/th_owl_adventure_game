using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public TransitionHotspot transitionHotspot;
    void OnMouseDown()
    {
        transitionHotspot?.Activate();
    }


}
