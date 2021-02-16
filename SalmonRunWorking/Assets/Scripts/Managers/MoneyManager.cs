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

    // The ManagerIndex with initialization values for a given tower
    public ManagerIndex initializationValues;

    // TODO: Buffer Transactions, Afford Calculations
    private bool transacting;

    private void Start()
    {
        // Get initialization values and set this towers basic values
        initializationValues = FindObjectOfType<ManagerIndex>();
        startingFunds = initializationValues.startingMoney;
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
