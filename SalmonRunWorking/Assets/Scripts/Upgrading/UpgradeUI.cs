using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * UI behavior script for the Upgrade menus
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private UpgradeType upgradeType = UpgradeType.SalmonLadderUp;       //< The type of upgrade whose UI we are interacting with
    [SerializeField] private Button upgradeButton;          //< The button for this upgrade
    //[SerializeField] private Button originalLadderButton;

    //[SerializeField] private List<Button> button_list = new List<Button>();
    
    // The buttons for when each of the Salmon Ladders are active
    [SerializeField] private Button LadderUp1 = null;
    [SerializeField] private Button LadderUp2 = null;

    // Checks UpgradeManager to see if the ladders are bought
    public bool ladder1Bought;
    public bool ladder2Bought;

    private float upgradeCost;          //< The cost of the Upgrade

    /*
     * Start is called prior to the first frame update
     */
    void Start()
    {
        upgradeCost = ManagerIndex.MI.UpgradeManager.GetUpgradeCost(upgradeType);
        ladder1Bought = false;
        ladder2Bought = false;
    }

    public bool CanAfford => ManagerIndex.MI.MoneyManager.CanAfford(upgradeCost);

    /*
     * Indicates that the first Salmon Ladder variant has been purchased
     */
    public void Ladder1Bought()
    {
        ladder1Bought = true;
    }

    /*
     * Indicates that the second Salmon Ladder variant has been purchased
     */
    public void Ladder2Bought()
    {
        ladder2Bought = true;
    }

    /*
     * Has the Manager Index tell the MoneyManager to spend the amount of money needed to purchase the Upgrade
     */
    public void Purchase()
    {
        ManagerIndex.MI.MoneyManager.SpendMoney(upgradeCost);
    }

    /*
     * Update is called every frame update
     */
    public void Update()
    {
        // Checks to see if you can afford the upgrade, and if its already been bought its turned off
        if (!ladder1Bought)
        {
            LadderUp1.interactable = CanAfford ? true : false;
        }
        else
        {
            LadderUp1.interactable = false;
        }

        if (!ladder2Bought)
        {
            LadderUp2.interactable = CanAfford ? true : false;
        }
        else
        {
            LadderUp2.interactable = false;
        }

        //ManagerIndex.MI.UpgradeManager.upgradeSmallCatchButton.interactable = ManagerIndex.MI.UpgradeManager.smallRateMax ? false : true;

        if (ManagerIndex.MI.UpgradeManager.smallRateMax == true)
        {
            ManagerIndex.MI.UpgradeManager.upgradeSmallCatchButton.interactable = false;
        }

        if (ManagerIndex.MI.UpgradeManager.mediumRateMax == true)
        {
            ManagerIndex.MI.UpgradeManager.upgradeMediumCatchButton.interactable = false;
        }

        if (ManagerIndex.MI.UpgradeManager.largeRateMax == true)
        {
            ManagerIndex.MI.UpgradeManager.upgradeLargeCatchButton.interactable = false;
        }
    }

    /*
     * Sets a button as interactable based on if the player can afford an Upgrade or not
     */
    public void UpdateButton()
    {
        //upgradeButton.interactable = CanAfford ? true : false;

        /*
        if (ManagerIndex.MI.UpgradeManager.firstPurchase1)
        {
            ManagerIndex.MI.UpgradeManager.upgradeLadderOneButton.interactable = CanAfford ? true : false;
        }

        if (ManagerIndex.MI.UpgradeManager.firstPurchase2)
        {
            ManagerIndex.MI.UpgradeManager.upgradeLadderTwoButton.interactable = CanAfford ? true : false;
        }
        */
    }

}
