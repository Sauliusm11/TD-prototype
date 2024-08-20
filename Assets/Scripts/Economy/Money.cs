using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Money tracking class
/// Should only be attached to the MoneyHandler object
/// </summary>
public class Money : MonoBehaviour
{
    int money;
    TMP_Text moneyDisplay;    
    // Start is called before the first frame update
    void Start()
    {
        ResetMoney();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Reset current money
    /// TODO: will be used by load level method
    /// </summary>
    public void ResetMoney()
    {
        money = 300;
        UpdateMoneyDisplay();
    }
    /// <summary>
    /// Checks if the player has enough money for a purchase
    /// </summary>
    /// <param name="cost">Cost of the purchase</param>
    /// <returns>True if the player has enough money, false if not</returns>
    public bool HasEnoughMoney(int cost)
    {
        return money >= cost;
    }
    /// <summary>
    /// Removes a given amount of money
    /// </summary>
    /// <param name="cost">Cost of the purchase</param>
    /// <returns>If the purchase was sucessful</returns>
    public bool RemoveMoney(int cost)
    {
        if(HasEnoughMoney(cost))
        {
            money -= cost;
            UpdateMoneyDisplay();
            return true;
        }
        return false;

    }
    /// <summary>
    /// Adds a given amount of money 
    /// </summary>
    /// <param name="gains">Money to add</param>
    public void AddMoney(int gains)
    {
        money += gains;
        UpdateMoneyDisplay();
    }
    /// <summary>
    /// Update the money display text in the ui
    /// </summary>
    void UpdateMoneyDisplay()
    {
        if (moneyDisplay == null)
        {
            moneyDisplay = GameObject.Find("MoneyCount").GetComponent<TMP_Text>();
        }
        moneyDisplay.text = money.ToString();
    }
}
