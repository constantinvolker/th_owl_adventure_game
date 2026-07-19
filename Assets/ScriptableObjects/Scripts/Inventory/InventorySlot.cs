using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    private ItemData _item;

    public void Setup(ItemData item)
    {
        _item = item;
        iconImage.sprite  = item.icon;
        iconImage.enabled = item.icon != null;
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        // If holding an item and clicking a different slot — try combination
        if (ItemSelectionState.Instance.HasSelection &&
            ItemSelectionState.Instance.SelectedItem != _item)
        {
            CombinationManager.Instance.TryCombine(
                ItemSelectionState.Instance.SelectedItem, _item);
            return;
        }

        // Normal selection
        ItemSelectionState.Instance.SelectItem(_item);
    }
}