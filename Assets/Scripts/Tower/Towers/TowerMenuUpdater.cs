using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Class responsible for updating the tower menu with statistics of the currently selected tower
/// </summary>
public class TowerMenuUpdater : MonoBehaviour
{
    [SerializeField]
    TMP_Text sellPriceText;
    [SerializeField]
    TMP_Text attackDamageText;
    [SerializeField]
    TMP_Text attackSpeedText;
    [SerializeField]
    TMP_Text attackRangeText;
    [SerializeField]
    TMP_Text towerNameText;
    [SerializeField]
    TMP_Text tierText;
    [SerializeField]
    TMP_Text upgrade1ButtonText;
    [SerializeField]
    TMP_Text upgrade2ButtonText;
    [SerializeField]
    GameObject upgrade1Button;
    [SerializeField]
    GameObject upgrade2Button;
    Button upgrade1SpriteRenderer;
    Button upgrade2SpriteRenderer;
    TowerContainer.Upgrade nextUpgrade;
    UpgradeHandler currentTower;

    private void Start()
    {
        upgrade1SpriteRenderer = upgrade1Button.GetComponent<Button>();
        upgrade2SpriteRenderer = upgrade2Button.GetComponent<Button>();
    }
    private void FixedUpdate()
    {
        //Money can always be changing, so we check for updates in update
        if (currentTower != null)
        {
            SetButtonState(1, currentTower.IsUpgradeAvailable());
            SetButtonState(2, currentTower.IsSecondaryUpgradeAvailable());
        }
    }
    /// <summary>
    /// Changes the interactable state of the given button
    /// </summary>
    /// <param name="buttonNumber">1 - regular upgrade; 2 - alternate elite upgrade</param>
    /// <param name="state">New state</param>
    private void SetButtonState(int buttonNumber, bool state)
    {
        switch (buttonNumber)
        {
            case 1:
                upgrade1SpriteRenderer.interactable = state;
                break;
            case 2:
                upgrade2SpriteRenderer.interactable = state;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Update all tower menu fields with the given tower's statistics
    /// </summary>
    /// <param name="tower">Tower upgrade handler</param>
    public void UpdateTowerMenu(UpgradeHandler tower)
    {
        currentTower = tower;
        //Full tower object name is "Tower (Clone)", want to show only "Tower"
        int cloneTextLength = 7;
        towerNameText.text = tower.name[..^cloneTextLength];
        sellPriceText.text = "Sell for: " + tower.GetSellCost().ToString();
        nextUpgrade = tower.GetUpgrade();

        //Upgrade button handling
        if (nextUpgrade.tier != -1)
        {
            upgrade1Button.SetActive(true);
            upgrade1ButtonText.text = "Tier " + nextUpgrade.tier + " upgrade for: " + nextUpgrade.cost;
            //TODO: Consider having elite tiers not hardcoded?
            if (nextUpgrade.tier == 3)
            {
                upgrade2Button.SetActive(true);
                upgrade1ButtonText.text = "Elite upgrade for: " + nextUpgrade.cost;
                upgrade2ButtonText.text = "Alternate elite upgrade for: " + tower.GetSecondaryElite().cost;
            }
            else
            {
                upgrade2Button.SetActive(false);
            }
            tierText.text = "Tier: " + (nextUpgrade.tier - 1).ToString();
        }
        else
        {
            tierText.text = "Tier: max";
            upgrade1Button.SetActive(false);
            upgrade2Button.SetActive(false);
        }

        attackDamageText.text = "Attack damage:" + tower.GetAttackDamage().ToString();
        attackSpeedText.text = "Fire rate:" + tower.GetAttackSpeed().ToString() + "/s";
        attackRangeText.text = "Attack range:" + tower.GetAttackRange().ToString();
        //Projectile speed is a hidden stat for now

    }
}
