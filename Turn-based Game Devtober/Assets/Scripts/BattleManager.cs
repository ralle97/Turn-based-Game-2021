using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleManager : MonoBehaviour
{
    //public SceneFader sceneFader;

    public GameObject playerPrefab;
    public GameObject[] basicEnemiesPrefabs;
    public GameObject[] miniBossPrefabs;
    public GameObject mainBossPrefab;

    public GameObject itemPrefab;

    [SerializeField]
    private Transform playerPosition;
    [SerializeField]
    private Transform enemyPosition;

    private Unit playerUnit;
    private Unit enemyUnit;

    [SerializeField]
    private TextMeshProUGUI dialogueText;
    [SerializeField]
    private GameObject battleButtons;
    [SerializeField]
    private GameObject backButton;
    [SerializeField]
    private GameObject itemsMenu;
    [SerializeField]
    private GameObject descriptionText;

    [SerializeField]
    public BattleHUD playerHUD;
    [SerializeField]
    public BattleHUD enemyHUD;

    public BattleState state;

    private void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject player = Instantiate(playerPrefab, playerPosition);
        playerUnit = player.GetComponent<Unit>();

        GameObject enemy = Instantiate(mainBossPrefab, enemyPosition);
        enemyUnit = enemy.GetComponent<Unit>();

        dialogueText.text = "A wild " + enemyUnit.unitName + " appeared...";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        List<Item> items = GameManager.instance.items;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemCount <= 0)
                continue;

            GameObject item = (GameObject)Instantiate(itemPrefab);
            
            item.GetComponent<ItemUI>().item = items[i];
            
            item.transform.SetParent(itemsMenu.transform);

            item.transform.localScale = Vector3.one;
            item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 0);
        }

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogueText.text = "The attack is successful!";

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    public void UseItem(Item item)
    {
        item.UseItem(playerUnit);

        OnBackButton();
        battleButtons.gameObject.SetActive(false);

        dialogueText.text = "You used " + item.itemName + "!";
        playerHUD.UpdateHUD(playerUnit.currentHP, playerUnit.currentMP);

        StartCoroutine(WaitFunction(2f));

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator WaitFunction(float toWait)
    {
        yield return new WaitForSeconds(toWait);
    }

    IEnumerator EnemyTurn()
    {
        switch (enemyUnit.type)
        {
            case UnitType.ENEMY:
                dialogueText.text = enemyUnit.unitName + " attacks!";
                
                yield return new WaitForSeconds(1f);

                bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
                playerHUD.SetHP(playerUnit.currentHP);

                yield return new WaitForSeconds(1f);

                if (isDead)
                {
                    state = BattleState.LOST;
                    EndBattle();
                }
                else
                {
                    state = BattleState.PLAYERTURN;
                    PlayerTurn();
                }
                
                break;

            case UnitType.MINIBOSS:
                break;

            case UnitType.MAINBOSS:
                break;
        }
    }

    private void EndBattle()
    {
        if (state == BattleState.WON)
        {
            float randomize = Random.Range(0f, 1f);
            dialogueText.text = "You won the battle!\nYou earned " + enemyUnit.experience + " XP.";
            if (randomize <= 0.4f)
            {
                dialogueText.text += "\nYou found health potion!";
                bool found = false;
                
                foreach (Item item in GameManager.instance.items)
                {
                    if (item.itemName == "Health Potion")
                    {
                        item.itemCount++;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Item hpPotion = new Item("Health Potion", 1);
                    GameManager.instance.items.Add(hpPotion);
                }
            }
            else if (randomize <= 0.8f)
            {
                dialogueText.text += "\nYou found magic potion!";
                bool found = false;

                foreach (Item item in GameManager.instance.items)
                {
                    if (item.itemName == "Magic Potion")
                    {
                        item.itemCount++;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Item mpPotion = new Item("Magic Potion", 1);
                    GameManager.instance.items.Add(mpPotion);
                }
            }

            StartCoroutine(WaitFunction(5f));

            //sceneFader.FadeTo("PreviousExplorationStageFromPlayerPrefs");
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated.";

            StartCoroutine(WaitFunction(2f));

            //Show game over screen
        }
    }

    private void PlayerTurn()
    {
        dialogueText.text = "Chose an action: ";

        battleButtons.gameObject.SetActive(true);
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        state = BattleState.ENEMYTURN;

        battleButtons.gameObject.SetActive(false);

        StartCoroutine(PlayerAttack());
    }

    public void OnMagicButton()
    {
        backButton.gameObject.SetActive(true);
    }

    public void OnItemsButton()
    {
        backButton.gameObject.SetActive(true);
        descriptionText.gameObject.SetActive(true);
        itemsMenu.gameObject.SetActive(true);
    }

    public void OnFleeButton()
    {

    }

    public void OnBackButton()
    {
        backButton.gameObject.SetActive(false);
        itemsMenu.gameObject.SetActive(false);
        //magicMenu.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
    }
}
