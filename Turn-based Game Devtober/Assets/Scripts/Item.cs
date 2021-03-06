using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public Sprite itemIcon;
    public string itemName;
    public string itemDescription;
    public int itemCount = 0;

    [SerializeField]
    private int restoreValue = 10;

    public Item(string name, int count = 1)
    {
        this.itemName = name;
        this.itemCount = count;
    }

    public bool UseItem(Unit player)
    {
        if (itemCount <= 0)
            return false;

        if (itemName.Equals("Health Potion"))
        {
            player.Heal(restoreValue);
            itemCount--;
        }
        else
        {
            player.RestoreMP(restoreValue);
            itemCount--;
        }

        return true;
    }
}
