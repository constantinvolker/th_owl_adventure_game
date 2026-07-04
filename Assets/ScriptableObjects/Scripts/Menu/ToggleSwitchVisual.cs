using UnityEngine;
using UnityEngine.UI;

public class ToggleSwitchVisual : MonoBehaviour
{
    [SerializeField] private RectTransform knob;
    [SerializeField] private Image track;

    [SerializeField] private Color onColor = Color.green;
    [SerializeField] private Color offColor = Color.gray;

    private Toggle toggle;
    private Vector2 offPos = new Vector2(-15f, 0f);
    private Vector2 onPos = new Vector2(15f, 0f);

    private float speed = 12f;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    void Update()
    {
        if (toggle == null || knob == null || track == null)
            return;
        
        Vector2 target = toggle.isOn ? onPos : offPos;
        
        knob.anchoredPosition = Vector2.Lerp(
            knob.anchoredPosition,
            target,
            Time.unscaledDeltaTime * speed
        );

        track.color = toggle.isOn ? onColor : offColor;
    }
}
