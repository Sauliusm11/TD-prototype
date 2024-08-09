using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public void ResetMoney()
    {
        money = 300;
        UpdateMoneyDisplay();
    }
    public bool HasEnoughMoney(int cost)
    {
        return money >= cost;
    }
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
    public void AddMoney(int gains)
    {
        money += gains;
        UpdateMoneyDisplay();
    }
    void UpdateMoneyDisplay()
    {
        if (moneyDisplay == null)
        {
            moneyDisplay = GameObject.Find("MoneyCount").GetComponent<TMP_Text>();
        }
        moneyDisplay.text = money.ToString();
    }
}
