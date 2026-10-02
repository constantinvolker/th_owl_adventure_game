using UnityEngine;

public class OpenLinkButton : MonoBehaviour
{
    [SerializeField]
    private string url;

    public void OpenLink()
    {
        Application.OpenURL(url);
    }
}
