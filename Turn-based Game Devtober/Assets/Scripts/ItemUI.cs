using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;

    public Image icon;
    public TextMeshProUGUI nameText;
    private TextMeshProUGUI descriptionText;
    public TextMeshProUGUI countText;

    public Button useButton;

    private void Start()
    {
        icon.sprite = item.itemIcon;
        nameText.text = item.itemName;
        countText.text = "x " + item.itemCount;

        descriptionText = GameObject.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData pointer)
    {
        descriptionText.text = item.itemDescription;
        descriptionText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointer)
    {
        descriptionText.gameObject.SetActive(false);
        descriptionText.text = "";
    }

    public void OnUseButton()
    {
        StartCoroutine(GameObject.Find("BattleManager").GetComponent<BattleManager>().UseItem(item));
        countText.text = "x " + item.itemCount;
    }
}
