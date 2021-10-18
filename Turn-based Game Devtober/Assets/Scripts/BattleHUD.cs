using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    
    public Slider hpSlider;
    public Slider mpSlider;

    private Color green = new Color(0, 219 / 255f, 42 / 255f);
    private Color yellow = new Color(219 / 255f, 192 / 255f, 0);
    private Color red = new Color(219 / 255f, 0, 0);

    private Color blue = new Color(0, 42 / 255f, 219 / 255f);

    public void SetHUD(Unit unit)
    {
        nameText.text = unit.unitName;

        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;

        ChangeHPColor();

        if (unit.unitName == "Player")
        {
            levelText.text = "Lvl " + unit.unitLevel;

            mpSlider.maxValue = unit.maxMP;
            mpSlider.value = unit.currentMP;
        }
    }

    public void SetHP(int hp)
    {
        hpSlider.value = hp;
        ChangeHPColor();
    }

    public void SetMP(int mp)
    {
        mpSlider.value = mp;
    }

    public void UpdateHUD(int hp, int mp)
    {
        SetHP(hp);
        SetMP(mp);
    }

    private void ChangeHPColor()
    {
        Image hpSliderImage = hpSlider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
        float hpRatio = hpSlider.value / hpSlider.maxValue;
        if (hpRatio <= 0.15f)
        {
            hpSliderImage.color = red;
        }
        else if (hpRatio <= 0.4f)
        {
            hpSliderImage.color = yellow;
        }
        else
        {
            hpSliderImage.color = green;
        }
    }
}
