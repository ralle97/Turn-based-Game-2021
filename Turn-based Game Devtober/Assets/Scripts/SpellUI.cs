using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SpellUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Spell spell;

    public TextMeshProUGUI nameText;
    private TextMeshProUGUI descriptionText;
    private TextMeshProUGUI spellCost;

    public Button castButton;

    private Color oldColor;

    private void Start()
    {
        nameText.text = spell.spellName;

        descriptionText = GameObject.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        spellCost = GameObject.Find("SpellCostText").GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData pointer)
    {
        descriptionText.text = spell.spellDescription;
        Unit player = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();
        spellCost.text = "Cost: " + spell.mpCost + "/" + player.currentMP;
        
        if (player.currentMP < spell.mpCost)
        {
            oldColor = spellCost.color;
            spellCost.color = Color.red;
        }

        descriptionText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointer)
    {
        descriptionText.gameObject.SetActive(false);
        descriptionText.text = "";
        spellCost.text = "";
        if (spellCost.color == Color.red)
            spellCost.color = oldColor;
    }

    public void OnCastButton()
    {
        GameObject.Find("BattleManager").GetComponent<BattleManager>().CastSpell(spell);
    }
}
