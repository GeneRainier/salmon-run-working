using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private UpgradeType upgradeType;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button originalLadderButton;

    [SerializeField] private List<Button> button_list = new List<Button>();

    private float upgradeCost;

    void Start()
    {
        upgradeCost = ManagerIndex.MI.UpgradeManager.GetUpgradeCost(upgradeType);
    }

    public bool CanAfford => ManagerIndex.MI.MoneyManager.CanAfford(upgradeCost);

    public void Purchase()
    {
        ManagerIndex.MI.MoneyManager.SpendMoney(upgradeCost);
    }

    public void Update()
    {

        foreach (Button upgradeButton in button_list)
        {
            upgradeButton.interactable = CanAfford ? true : false;
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
