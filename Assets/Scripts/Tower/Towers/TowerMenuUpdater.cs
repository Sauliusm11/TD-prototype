using UnityEngine;
using TMPro;
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
    TMP_Text upgrade1ConfirmationButtonText;
    [SerializeField]
    TMP_Text upgrade2ConfirmationButtonText;
    [SerializeField]
    GameObject upgrade1Button;
    [SerializeField]
    GameObject upgrade2Button;
    [SerializeField]
    GameObject upgrade1ConfirmationButton;
    [SerializeField]
    GameObject upgrade2ConfirmationButton;
    Button upgrade1SpriteRenderer;
    Button upgrade2SpriteRenderer;
    TowerContainer.Upgrade nextUpgrade;
    TowerContainer.Upgrade alternateEliteUpgrade;
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
            if (upgrade1Button.activeInHierarchy)
            {
                SetButtonState(1, currentTower.IsUpgradeAvailable(false));
            }
            if (upgrade2Button.activeInHierarchy)
            {
                SetButtonState(2, currentTower.IsUpgradeAvailable(true));
            }
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
        UpdateBaseStatsDisplay(currentTower);
        //Full tower object name is "Tower (Clone)", want to show only "Tower"
        towerNameText.text = tower.name.Remove(tower.name.Length - 7, 7);
        sellPriceText.text = "Sell for: " + tower.GetSellCost().ToString();
        nextUpgrade = tower.GetUpgrade();

        //Upgrade button handling
        if (nextUpgrade.tier != -1)
        {
            upgrade1Button.SetActive(true);
            upgrade1ButtonText.text = "Tier " + nextUpgrade.tier + " upgrade for: " + nextUpgrade.cost;
            //TODO: Consider having elite tiers not hardcoded?
            upgrade2Button.SetActive(false);
            if (nextUpgrade.tier == 3)
            {
                upgrade2Button.SetActive(true);
                alternateEliteUpgrade = tower.GetSecondaryElite();
                upgrade1ButtonText.text = "Elite upgrade for: " + nextUpgrade.cost;
                upgrade2ButtonText.text = "Alternate elite upgrade for: " + alternateEliteUpgrade.cost;
            }
            tierText.text = "Tier: " + (nextUpgrade.tier - 1).ToString();
        }
        else
        {
            tierText.text = "Tier: max";
            upgrade1Button.SetActive(false);
            upgrade2Button.SetActive(false);
        }
    }
    /// <summary>
    /// Updates the current stats of the tower on the display
    /// </summary>
    /// <param name="tower">Upgrade handler of the tower to display</param>
    public void UpdateBaseStatsDisplay(UpgradeHandler tower)
    {
        attackDamageText.text = "Attack damage:" + tower.GetAttackDamage().ToString();
        attackSpeedText.text = "Fire rate:" + tower.GetAttackSpeed().ToString() + "/s";
        attackRangeText.text = "Attack range:" + tower.GetAttackRange().ToString();
        //Projectile speed is a hidden stat for now
    }
    /// <summary>
    /// Gets the sign of a number from a given value
    /// </summary>
    /// <param name="value">Number to determine the sign of</param>
    /// <returns>String containing "+" if positive empty string if negative</returns>
    string GetSign(float value)
    {
        if (value > 0)
        {
            return "+";
        }
        return string.Empty;
    }
    /// <summary>
    /// Appends a given string with the given stat change
    /// </summary>
    /// <param name="currentString">Currently displayed stat string</param>
    /// <param name="statChange">Added or removed value if the upgrade is confirmed</param>
    /// <returns>Formatted string displaying the change</returns>
    string UpdateStatPreview(string currentString, float statChange)
    {
        string sign = GetSign(statChange);
        return string.Format("{0} ({1}{2})", currentString, sign, statChange);
    }
    /// <summary>
    /// Method called when clicking the upgrade button in the tower menu.
    /// Shows the preview and enables the confirmation button
    /// (References do not show up, it is working)
    /// </summary>
    public void ActivateConfirmation1()
    {
        DisableConfirmations();
        upgrade1ConfirmationButton.SetActive(true);
        upgrade1ConfirmationButtonText.text = "Confirm upgrade for:" + nextUpgrade.cost;
        attackDamageText.text = UpdateStatPreview(attackDamageText.text, nextUpgrade.attackDamage);
        attackSpeedText.text = UpdateStatPreview(attackSpeedText.text, nextUpgrade.attackSpeed);
        attackRangeText.text = UpdateStatPreview(attackRangeText.text, nextUpgrade.attackRange);
        currentTower.EnablePreviewRangeIndicator();
        currentTower.UpdatePreviewRangeIndicator(nextUpgrade.attackRange);
    }
    /// <summary>
    /// Method called when clicking the alterante elite upgrade button in the tower menu.
    /// Shows the preview and enables the confirmation button
    /// (References do not show up, it is working)
    /// </summary>
    public void ActivateConfirmation2()
    {
        DisableConfirmations();
        upgrade2ConfirmationButton.SetActive(true);
        upgrade2ConfirmationButtonText.text = "Confirm upgrade for:" + alternateEliteUpgrade.cost;
        attackDamageText.text = UpdateStatPreview(attackDamageText.text, alternateEliteUpgrade.attackDamage);
        attackSpeedText.text = UpdateStatPreview(attackSpeedText.text, alternateEliteUpgrade.attackSpeed);
        attackRangeText.text = UpdateStatPreview(attackRangeText.text, alternateEliteUpgrade.attackRange);
        currentTower.EnablePreviewRangeIndicator();
        currentTower.UpdatePreviewRangeIndicator(alternateEliteUpgrade.attackRange);
    }
    /// <summary>
    /// Disables all active confirmation buttons and turns off the preview
    /// </summary>
    public void DisableConfirmations()
    {
        upgrade1ConfirmationButton.SetActive(false);
        upgrade2ConfirmationButton.SetActive(false);
        if (currentTower != null)
        {
            currentTower.DisablePreviewRangeIndicator();
            UpdateBaseStatsDisplay(currentTower);
        }
    }
}
