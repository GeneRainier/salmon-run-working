using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private UpgradeType upgradeType;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button originalLadderButton;
    [SerializeField] private Button upgradeLadderOneButton;
    [SerializeField] private Button upgradeLadderTwoButton;

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

    public void UpdateButton()
    {
        upgradeButton.interactable = CanAfford ? true : false;

        /*
        if (ManagerIndex.MI.UpgradeManager.firstPurchase1)
        {
            upgradeLadderOneButton.interactable = CanAfford ? true : false;
        }

        if (ManagerIndex.MI.UpgradeManager.firstPurchase2)
        {
            upgradeLadderTwoButton.interactable = CanAfford ? true : false;
        }
        */
       
    }

}
