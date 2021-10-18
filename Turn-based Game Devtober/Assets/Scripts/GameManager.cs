using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager instance;

    #region Singleton

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    #endregion

    [HideInInspector]
    public string area = "Main";
    [HideInInspector]
    public bool isBossBattle = false;
    [HideInInspector]
    public int miniBossesDefeated = 0;

    public List<Item> items;
}
