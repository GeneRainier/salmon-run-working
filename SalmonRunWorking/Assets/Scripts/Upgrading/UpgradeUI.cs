using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private UpgradeType upgradeType;
    [SerializeField] private Button upgradeButton;
    //[SerializeField] private Button originalLadderButton;

    //[SerializeField] private List<Button> button_list = new List<Button>();
    [SerializeField] private Button LadderUp1;
    [SerializeField] private Button LadderUp2;

    //checks UpgradeManager to see if the ladders are bought
    public bool ladder1Bought;
    public bool ladder2Bought;


    private float upgradeCost;

    void Start()
    {
        upgradeCost = ManagerIndex.MI.UpgradeManager.GetUpgradeCost(upgradeType);
        ladder1Bought = false;
        ladder2Bought = false;
    }

    public bool CanAfford => ManagerIndex.MI.MoneyManager.CanAfford(upgradeCost);

    public void Ladder1Bought()
    {
        ladder1Bought = true;
    }

    public void Ladder2Bought()
    {
        ladder2Bought = true;
    }

    public void Purchase()
    {
        ManagerIndex.MI.MoneyManager.SpendMoney(upgradeCost);
    }

    public void Update()
    {
        //checks to see if you can afford the upgrade, and if its already been bought its turned off
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
