using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    [Header("Starting Funds")] 
    [SerializeField] private float startingFunds = 30f;
    
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI moneyText;
    
    [Header("Tax Rates")]
    [SerializeField] private float catchTaxRate = 1.0f;
    
    static private float bank;
    
    // TODO: Buffer Transactions, Afford Calculations
    private bool transacting;

    private void Start()
    {
        bank = startingFunds;
        UpdateText();
    }

    public bool CanAfford(float amount)
    {
        return amount <= bank;
    }

    public void AddFunds(float amountToAdd)
    {
        bank += amountToAdd;
        UpdateText();
    }

    public void AddCatch(float weight)
    {
        AddFunds(catchTaxRate * weight);
    }

    public void SpendMoney(float amount)
    {
        bank -= amount;
        if (bank < 0)
        {
            bank = 0;
        }

        UpdateText();
    }

    public void UpdateText()
    {
        moneyText.text = "Money: $ " + bank.ToString("F2");
    }
}
