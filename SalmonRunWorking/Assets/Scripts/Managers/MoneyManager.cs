using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * Manager script that handles how much money the player has and how they spend it
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class MoneyManager : MonoBehaviour
{
    [Header("Starting Funds")] 
    [SerializeField] private float startingFunds = 30.0f;       //< The amount of money the player starts with
    
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI moneyText;         //< The text that shows how much money the player has
    
    [Header("Tax Rates")]
    [SerializeField] private float catchTaxRate = 1.0f;         //< The amount of tax each fish catch is subject to
    
    static private float bank;          //< Reference to the amount of money the player has "in the bank"

    public ManagerIndex initializationValues;       //< The ManagerIndex with initialization values for a given tower

    // TODO: Buffer Transactions, Afford Calculations
    private bool transacting;           //< Is the purchase still being processed

    /*
     * Start is called prior to the first frame update
     */
    private void Start()
    {
        // Get initialization values and set this towers basic values
        initializationValues = FindObjectOfType<ManagerIndex>();
        startingFunds = initializationValues.startingMoney;
        bank = startingFunds;
        UpdateText();
    }

    /*
     * Checks if the player can afford a tower
     * 
     * @param amount The amount of money the tower costs
     * @return bool True if the player has enough money to purchase the tower
     */
    public bool CanAfford(float amount)
    {
        return amount <= bank;
    }

    /*
     * Adds money to the player's bank
     * 
     * @param amountToAdd The amount of money the player gains
     */
    public void AddFunds(float amountToAdd)
    {
        bank += amountToAdd;
        UpdateText();
    }

    /*
     * Adds cash to the player's bank based on the weight of the caught fish
     * 
     * @param weight The weight of the caught fish
     */
    public void AddCatch(float weight)
    {
        AddFunds(catchTaxRate * weight);
    }

    /*
     * Removes cash based on what the player has purchased
     * 
     * @param amount The amount of the money the player has spent
     */
    public void SpendMoney(float amount)
    {
        bank -= amount;
        if (bank < 0)
        {
            bank = 0;
        }

        UpdateText();
    }

    /*
     * Updates the money text based on how much money is in the bank
     */
    public void UpdateText()
    {
        moneyText.text = "Money: $ " + bank.ToString("F0");
    }
}
