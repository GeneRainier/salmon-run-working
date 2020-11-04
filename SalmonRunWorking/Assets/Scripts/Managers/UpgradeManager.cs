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

    //the three ladder buttons
    public Button originalLadderButton;
    public Button upgradeLadderOneButton;
    public Button upgradeLadderTwoButton;

    //the three fisherman catch rate buttons
    public Button upgradeSmallCatchButton;
    public Button upgradeMediumCatchButton;
    public Button upgradeLargeCatchButton;

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

    //fisherman catch rate values
    private float smallRate;
    private float mediumRate;
    private float largeRate;

    public float SmallRate
    {
        get => smallRate;
        set => smallRate = value;
    }

    public float MediumRate
    {
        get => mediumRate;
        set => mediumRate = value;
    }

    public float LargeRate
    {
        get => largeRate;
        set => largeRate = value;
    }

    //bool to see if maximum catch rate has been reached
    public bool smallRateMax = false;
    public bool mediumRateMax = false;
    public bool largeRateMax = false;

    //will be true if you have a fisherman
    public bool isAFisherman = false;

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
            }

            UpdateButtons();
        }
        else if(isAFisherman)
        {
            if (ManagerIndex.MI.GameManager.PlaceState && (!smallRateMax || !mediumRateMax || !largeRateMax))
            {
                upgradeButtonTray.interactable = true;
            }
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

    public void UpgradeButtonFisherman()
    {
        if (EventSystem.current.currentSelectedGameObject.name == upgradeSmallCatchButton.name)
        {
            //should be able to get rid of this first if statement if the UpgradeUI works properly and turns it off when max is hit
            if (smallRate < 1.0f)
            {
                upgradeUI.Purchase();
                smallRate += 0.1f;
                if (smallRate == 1.0f)
                {
                    smallRateMax = true;
                }
                Debug.Log(smallRate);
            }
        }

        if (EventSystem.current.currentSelectedGameObject.name == upgradeMediumCatchButton.name)
        {
            if (mediumRate < 1.0f)
            {
                upgradeUI.Purchase();
                mediumRate += 0.1f;
                if (mediumRate == 1.0f)
                {
                    mediumRateMax = true;
                }
            }
        }

        if (EventSystem.current.currentSelectedGameObject.name == upgradeLargeCatchButton.name)
        {
            if (largeRate < 1.0f)
            {
                upgradeUI.Purchase();
                largeRate += 0.1f;
                if (largeRate == 1.0f)
                {
                    largeRateMax = true;
                }
            }
        }
    }

    //Stuff to manage the upgrades, WHAT IS FIRST????
    public Upgrade GetUpgrade(UpgradeType upgradeType)
    {
        return upgrades.Find(upgrade => upgrade.UpgradeType == upgradeType);
    }

    //upgrade type refers to the type of tower you are upgrading
    public float GetUpgradeCost(UpgradeType upgradeType)
    {
        return upgrades.Find(upgrade => upgrade.UpgradeType == upgradeType).Cost;
    }

    //this this is what is giving an error when I add other stuff.
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
