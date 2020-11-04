using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



//types of upgrades
public enum UpgradeType
{
    SalmonLadderUp, FisherUp
}

public class UpgradeManager : MonoBehaviour
{
    public Button upgradeButtonTray;

    //the three buttons
    public Button originalLadderButton;
    public Button upgradeLadderOneButton;
    public Button upgradeLadderTwoButton;

    private bool finalUpgrade => numberOfUpgrades >= 2;

    private int numberOfUpgrades;

    //check to see if you have already bought them
    public bool firstPurchase1 = true;
    public bool firstPurchase2 = true;

    public UpgradeUI upgradeUI;

    //upgrade prefabs
    private GameObject originalLadder;
    public GameObject upgradeLadderOne;
    public GameObject upgradeLadderTwo;

    public GameObject OriginalLadder
    {
        get => originalLadder;
        set => originalLadder = value;
    }

    private GameObject firstLadder;
    
    [SerializeField] private List <Upgrade> upgrades;

    void Start()
    {
        upgradeButtonTray.interactable = false;
        upgradeLadderOne.SetActive(false);
        upgradeLadderTwo.SetActive(false);
        originalLadderButton.interactable = false;
    }

    void Update()
    {

        // if the dam has a ladder and if its PlaceState and if there are more upgrades available, then the button appears
        if (originalLadder)
        {
            if (ManagerIndex.MI.GameManager.PlaceState && !finalUpgrade)
            {
                upgradeButtonTray.interactable = true;
                //upgradeButtonColor.color = enabledColor;
            }

            UpdateButtons();
        }

        if (firstPurchase1)
        {
            
        }

        
        if (finalUpgrade)
        {
            upgradeButtonTray.interactable = false;
            //upgradeButtonColor.color = disabledColor;
        }
        
    }

    public void UpgradeButtonLadder()
    {
        /*
        numberOfUpgrades += 1;
        upgradeUI.Purchase();

        switch (numberOfUpgrades)
        {
            case 1:
                originalLadder.SetActive(false);
                //firstLadder.SetActive(false);
                upgradeLadderOne.SetActive(true);
                break;
            case 2:
                upgradeLadderOne.SetActive(false);
                upgradeLadderTwo.SetActive(true);
                break;
        }
        */
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        Debug.Log(EventSystem.current.currentSelectedGameObject == upgradeLadderOneButton);

        if (EventSystem.current.currentSelectedGameObject.name == upgradeLadderOneButton.name)
        {
            //sets the right later active
            originalLadder.SetActive(false);
            upgradeLadderTwo.SetActive(false);
            upgradeLadderOne.SetActive(true);

            //buys the ladder if not already bought
            if (firstPurchase1)
            {
                upgradeUI.Purchase();
                firstPurchase1 = false;
            }

            //making the right buttons clickable
            upgradeLadderOneButton.interactable = false;
            originalLadderButton.interactable = true;
            upgradeLadderTwoButton.interactable = true;
        }
        else if (EventSystem.current.currentSelectedGameObject.name == upgradeLadderTwoButton.name)
        {
            //sets the right later active
            originalLadder.SetActive(false);
            upgradeLadderOne.SetActive(false);
            upgradeLadderTwo.SetActive(true);

            //buys the ladder if not already bought
            if (firstPurchase2)
            {
                upgradeUI.Purchase();
                firstPurchase2 = false;
            }

            //making the right buttons clickable
            upgradeLadderOneButton.interactable = true;
            originalLadderButton.interactable = true;
            upgradeLadderTwoButton.interactable = false;

        }
        else if(EventSystem.current.currentSelectedGameObject.name == originalLadderButton.name)
        {
            //sets the right later active
            originalLadder.SetActive(true);
            upgradeLadderOne.SetActive(false);
            upgradeLadderTwo.SetActive(false);

            //making the right buttons clickable
            upgradeLadderOneButton.interactable = true;
            originalLadderButton.interactable = false;
            upgradeLadderTwoButton.interactable = true;
        }

    }

    //Stuff to manage the upgrades
    public Upgrade GetUpgrade(UpgradeType upgradeType)
    {
        return upgrades.First(upgrade => upgrade.UpgradeType == upgradeType);
    }

    public float GetUpgradeCost(UpgradeType upgradeType)
    {
        return upgrades.First(upgrade => upgrade.UpgradeType == upgradeType).Cost;
    }

    public void UpdateButtons()
    {
        upgrades.ForEach(upgrade => upgrade.UpgradeUI.UpdateButton());
    }
}

[Serializable]
public class Upgrade
{
    [SerializeField] private UpgradeType upgradeType;
    [SerializeField] private UpgradeUI upgradeUI;
    //[SerializeField] private bool enabled;
    [SerializeField] private float cost;

    public UpgradeType UpgradeType => upgradeType;

    public UpgradeUI UpgradeUI => upgradeUI;

    public float Cost => cost;

    /*
    public void Enable()
    {
        enabled = true;
    }

    public void Disable()
    {
        enabled = false;
    }
    */
}
