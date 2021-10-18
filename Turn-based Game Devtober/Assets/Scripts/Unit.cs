using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType { PLAYER, ENEMY, MINIBOSS, MAINBOSS }

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;

    public int damage;

    public int maxHP;
    public int currentHP;

    public int maxMP;
    public int currentMP;

    public int experience;

    public UnitType type;

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0)
            return true;
        else
            return false;
    }

    public bool UseMagic(int mp)
    {
        if (currentMP < mp)
        {
            return false;
        }
        else
        {
            currentMP -= mp;
            return true;
        }
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
    }

    public void RestoreMP(int amount)
    {
        currentMP = Mathf.Min(currentMP + amount, maxMP);
    }
}
