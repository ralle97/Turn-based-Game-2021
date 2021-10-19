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
    public GameObject spellPrefab;

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
    private Button attackButton;
    
    [SerializeField]
    private GameObject backButton;
    
    [SerializeField]
    private GameObject itemsMenu;
    [SerializeField]
    private GameObject magicMenu;

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

        foreach (Item itemGM in GameManager.instance.items)
        {
            if (itemGM.itemCount <= 0)
                continue;

            GameObject item = (GameObject)Instantiate(itemPrefab);
            
            item.GetComponent<ItemUI>().item = itemGM;
            
            item.transform.SetParent(itemsMenu.transform);

            item.transform.localScale = Vector3.one;
            item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 0);
        }

        foreach (Spell spellGM in GameManager.instance.spells)
        {
            if (!spellGM.learned)
                continue;

            GameObject spell = (GameObject)Instantiate(spellPrefab);

            if (spellGM.spellName.Equals("Wind Slash"))
            {
                spellGM.effectValue = 2 * playerUnit.damage;
            }

            spell.GetComponent<SpellUI>().spell = spellGM;

            spell.transform.SetParent(magicMenu.transform);

            spell.transform.localScale = Vector3.one;
            spell.transform.position = new Vector3(spell.transform.position.x, spell.transform.position.y, 0);
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

    public IEnumerator UseItem(Item item)
    {
        if (!item.UseItem(playerUnit))
            yield return 0;

        OnBackButton();
        battleButtons.gameObject.SetActive(false);

        dialogueText.text = "You used " + item.itemName + "!";
        playerHUD.UpdateHUD(playerUnit.currentHP, playerUnit.currentMP);

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        
        yield return StartCoroutine(EnemyTurn());
    }

    public IEnumerator CastSpell(Spell spell)
    {
        if (playerUnit.currentMP < spell.mpCost)
            yield return 0;

        state = BattleState.ENEMYTURN;

        bool isDead = false;
        string addText = "";

        if (spell.type == SpellType.RESTORE)
        {
            spell.CastSpell(playerUnit);
        }
        else if (spell.type == SpellType.DAMAGE)
        {
            if (spell.spellName.Equals("Eartsplitter"))
            {
                float randomize = Random.Range(0f, 1f);
                if (randomize <= 0.15f)
                {
                    isDead = spell.CastSpell(enemyUnit, true);
                    addText = "\nCRITICAL STRIKE!";
                }
            }
            else
                isDead = spell.CastSpell(enemyUnit);
            
            enemyHUD.SetHP(enemyUnit.currentHP);
        }

        playerUnit.currentMP -= spell.mpCost;
        playerHUD.UpdateHUD(playerUnit.currentHP, playerUnit.currentMP);

        OnBackButton();
        battleButtons.gameObject.SetActive(false);

        dialogueText.text = "You cast " + spell.spellName + "!" + addText;

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            yield return StartCoroutine(EnemyTurn());
        }
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
            dialogueText.text = "You won the battle!\nYou earned " + enemyUnit.experience + " XP.";

            float randomize = Random.Range(0f, 1f);
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

            Invoke("ReturnToGameWorld", 5f);
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated.";

            Invoke("GameOver", 2f);
        }
    }

    private void PlayerTurn()
    {
        dialogueText.text = "Choose an action: ";

        attackButton.Select();

        battleButtons.SetActive(true);
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        state = BattleState.ENEMYTURN;

        battleButtons.SetActive(false);

        StartCoroutine(PlayerAttack());
    }

    public void OnMagicButton()
    {
        backButton.SetActive(true);
        descriptionText.SetActive(true);
        magicMenu.SetActive(true);
    }

    public void OnItemsButton()
    {
        backButton.SetActive(true);
        descriptionText.SetActive(true);
        itemsMenu.SetActive(true);
    }

    public void OnFleeButton()
    {

    }

    public void OnBackButton()
    {
        backButton.SetActive(false);
        
        if (itemsMenu.activeSelf)
            itemsMenu.SetActive(false);
        
        if (magicMenu.activeSelf)
            magicMenu.SetActive(false);
        
        descriptionText.SetActive(false);
    }

    private void GameOver()
    {
        //GameManager.instance.GameOver();
    }

    private void ReturnToGameWorld()
    {
        //sceneFader.FadeTo("PreviousExplorationStageFromPlayerPrefs");
    }
}
