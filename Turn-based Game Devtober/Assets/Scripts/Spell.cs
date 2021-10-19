using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellType { RESTORE, DAMAGE, BUFF, DEBUFF }

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell")]
public class Spell : ScriptableObject
{
    public string spellName;
    public string spellDescription;

    public int mpCost;

    public int effectValue;

    public SpellType type;

    public bool learned;

    public Spell(string name, string description, int mp, SpellType t)
    {
        spellName = name;
        spellDescription = description;
        mpCost = mp;
        type = t;
    }

    public bool CastSpell(Unit unit, bool crit = false)
    {
        if (type == SpellType.RESTORE)
        {
            if (spellName.Equals("Heal"))
            {
                unit.Heal(effectValue);
            }
            else if (spellName.Equals("Focus"))
            {
                unit.RestoreMP(effectValue);
            }
        }
        else if (type == SpellType.DAMAGE)
        {
            if (spellName.Equals("Earthsplitter"))
            {                
                if (crit)
                {
                    return unit.TakeDamage(2 * effectValue);
                }
                else
                {
                    return unit.TakeDamage(effectValue);
                }
            }
            else
            {
                return unit.TakeDamage(effectValue);
            }
        }

        return false;
    }
}
