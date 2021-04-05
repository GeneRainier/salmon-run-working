using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;



//types of upgrades
public enum UpgradeType
{
    SalmonLadderUp, FisherUp
}

public class UpgradeManager : MonoBehaviour
{
    // The ManagerIndex with initialization values for a given tower
    public ManagerIndex initializationValues;

    public Button upgradeButtonTray;

    //the three ladder swap buttons
    public Button originalLadderButton;
    public Button upgradeLadderOneButton;
    public Button upgradeLadderTwoButton;

    //the two ladder upgrade buttons
    public Button ladderUp1Button;
    public Button ladderUp2Button;

    public Image salmonLadder;
    public Image salmonRamp;
    public Image salmonElevator;


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

    // Reference to the TowerManager
    [SerializeField] private TowerManager theTowerManager;

    public TextMeshProUGUI purchaseText;

    void Start()
    {
        // Get initialization values and set this towers basic values
        initializationValues = FindObjectOfType<ManagerIndex>();

        upgradeButtonTray.interactable = false;

        upgradeLadderOne.SetActive(false);
        upgradeLadderTwo.SetActive(false);

        originalLadderButton.interactable = false;
        upgradeLadderOneButton.interactable = false;
        upgradeLadderTwoButton.interactable = false;

        ladderUp1Button.interactable = false;
        ladderUp2Button.interactable = false;

        //currentActiveLadder = originalLadderButton;
        //currentActiveLadder.interactable = false;
    }

    void Update()
    {
        //Debug.Log("SmallRate: " + smallRate);
        // if the dam has a ladder and if its PlaceState and if there are more upgrades available, then the button appears
        if (originalLadder)
        {
            if (ManagerIndex.MI.GameManager.PlaceState && !finalUpgrade)
            {
                upgradeButtonTray.interactable = true;

                ladderUp1Button.interactable = true;
                ladderUp2Button.interactable = true;

                //upgradeLadderOneButton.interactable = true;
                //originalLadderButton.interactable = true;
                //upgradeLadderTwoButton.interactable = true;

                //currentActiveLadder.interactable = false;
            }

            /*
            if (!isAFisherman)
            {
                upgradeSmallCatchButton.interactable = false;
                upgradeMediumCatchButton.interactable = false;
                upgradeLargeCatchButton.interactable = false;
            }

            //UpdateButtons();
        }

        if (isAFisherman)
        {
            if (ManagerIndex.MI.GameManager.PlaceState && (!smallRateMax || !mediumRateMax || !largeRateMax))
            {
                upgradeButtonTray.interactable = true;

                upgradeSmallCatchButton.interactable = true;
                upgradeMediumCatchButton.interactable = true;
                upgradeLargeCatchButton.interactable = true;
            }

            if (!originalLadder)
            {
                upgradeLadderOneButton.interactable = false;
                originalLadderButton.interactable = false;
                upgradeLadderTwoButton.interactable = false;
            }
            */
        }
        
        //Think we can get rid of this cause we always need to interact with it since upgrades are sidgradable
        if (finalUpgrade && (smallRateMax && mediumRateMax && largeRateMax))
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

        if (EventSystem.current.currentSelectedGameObject.name == ladderUp1Button.name)
        {
            //sets the right later active
            originalLadder.SetActive(false);
            upgradeLadderTwo.SetActive(false);
            upgradeLadderOne.SetActive(true);

            //tell upgrdeUI it was bought
            upgradeUI.Ladder1Bought();

            //buys the ladder if not already bought
            if (firstPurchase1)
            {
                upgradeUI.Purchase();
                firstPurchase1 = false;
                ladderUp1Button.interactable = false;
            }

            salmonRamp.enabled = true;
            salmonElevator.enabled = false;
            salmonLadder.enabled = false;

            //making the right buttons clickable
            upgradeLadderOneButton.interactable = false;
            originalLadderButton.interactable = true;
            if (firstPurchase2)
            {
                upgradeLadderTwoButton.interactable = false;
            }
            else
            {
                upgradeLadderTwoButton.interactable = true;
            }
            
        }
        else if (EventSystem.current.currentSelectedGameObject.name == ladderUp2Button.name)
        {
            //sets the right later active
            originalLadder.SetActive(false);
            upgradeLadderOne.SetActive(false);
            upgradeLadderTwo.SetActive(true);

            upgradeUI.Ladder2Bought();

            //buys the ladder if not already bought
            if (firstPurchase2)
            {
                upgradeUI.Purchase();
                firstPurchase2 = false;
                ladderUp2Button.interactable = false;
            }

            salmonRamp.enabled = false;
            salmonElevator.enabled = true;
            salmonLadder.enabled = false;

            //making the right buttons clickable
            originalLadderButton.interactable = true;
            upgradeLadderTwoButton.interactable = false;
            if (firstPurchase1)
            {
                upgradeLadderOneButton.interactable = false;
            }
            else
            {
                upgradeLadderOneButton.interactable = true;
            }

        }
        /*
        else if(EventSystem.current.currentSelectedGameObject.name == originalLadderButton.name)
        {
            //sets the right later active
            originalLadder.SetActive(true);
            upgradeLadderOne.SetActive(false);
            upgradeLadderTwo.SetActive(false);

            currentActiveLadder = originalLadderButton;

            //making the right buttons clickable
            upgradeLadderOneButton.interactable = true;
            originalLadderButton.interactable = false;
            upgradeLadderTwoButton.interactable = true;
        }
        */

    }

    public void SwapLadderButtons()
    {
        if (EventSystem.current.currentSelectedGameObject.name == upgradeLadderOneButton.name && ManagerIndex.MI.GameManager.PlaceState)
        {
            originalLadder.SetActive(false);
            upgradeLadderTwo.SetActive(false);
            upgradeLadderOne.SetActive(true);

            salmonRamp.enabled = true;
            salmonElevator.enabled = false;
            salmonLadder.enabled = false;

            initializationValues.ladderCode = 10;

            if (!firstPurchase2)
            {
                upgradeLadderTwoButton.interactable = true;
            }
            upgradeLadderOneButton.interactable = false;
            originalLadderButton.interactable = true;
        }
        else if (EventSystem.current.currentSelectedGameObject.name == upgradeLadderTwoButton.name && ManagerIndex.MI.GameManager.PlaceState)
        {
            originalLadder.SetActive(false);
            upgradeLadderTwo.SetActive(true);
            upgradeLadderOne.SetActive(false);

            salmonRamp.enabled = false;
            salmonElevator.enabled = true;
            salmonLadder.enabled = false;

            initializationValues.ladderCode = 100;

            if (!firstPurchase1)
            {
                upgradeLadderOneButton.interactable = true;
            }
            upgradeLadderTwoButton.interactable = false;
            originalLadderButton.interactable = true;
        }
        else if (EventSystem.current.currentSelectedGameObject.name == originalLadderButton.name && ManagerIndex.MI.GameManager.PlaceState)
        {
            originalLadder.SetActive(true);
            upgradeLadderTwo.SetActive(false);
            upgradeLadderOne.SetActive(false);

            salmonRamp.enabled = false;
            salmonElevator.enabled = false;
            salmonLadder.enabled = true;

            initializationValues.ladderCode = 1;

            if (!firstPurchase1)
            {
                upgradeLadderOneButton.interactable = true;
            }
            
            if (!firstPurchase2)
            {
                upgradeLadderTwoButton.interactable = true;
            }
            originalLadderButton.interactable = false;
        }
    }

    /* Note in regards to UpgradeButtonFisherman as it relates to GetUpgrade and GetUpgradeCost
     * As of this moment, the syste is set up to upgrade ALL Fishermen towers at once as opposed to one at a time. The Manager is going to
     * set the rate as needed when the player hits the upgrade button and then all Fishermen will have the new rate from then on. Changing
     * it to an individual system may not be too difficult, but I am not going to change that until we come to an agreement on how we would
     * like upgrades to work.
     * 
     * To fix this, I added a list of Anglers to the TowerManager and had the UpgradeManager loop through each of the towers in that list
     * to directly change their currentRates as this code was not in any way changing the nature of the Angler towers / FishermanTower script.
     * This code is not complete. It only affects EXISTING towers, so towers instantiated after this is called will NOT get the upgrade until
     * the button is hit again. I will change this code further after we agree on whether we want to have individual or system wide upgrades.
     */

    public void UpgradeButtonFisherman()
    {
        if (EventSystem.current.currentSelectedGameObject.name == upgradeSmallCatchButton.name)
        {
            Debug.Log("Entered Fishermen Upgrade");
            //should be able to get rid of this first if statement if the UpgradeUI works properly and turns it off when max is hit
            if (smallRate < 1.0f)
            {
                Debug.Log("If is good");
                upgradeUI.Purchase();
                smallRate += 0.1f;
                if (smallRate == 1.0f)
                {
                    smallRateMax = true;
                }
                foreach (FishermanTower towers in theTowerManager.GetAnglers())
                {
                    towers.SetSmallCatchRate(smallRate);
                    Debug.Log("Set tower rate");
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

    //this makes the "purchased!" text pop up and then fade after a few seconds. Will throw in a particle effect in here as well.
    public void PurchasedPopUp()
    {
        StartCoroutine(MyCoroutine());
    }

    IEnumerator MyCoroutine()
    {
        purchaseText.enabled = true;
        yield return new WaitForSeconds(3f);
        purchaseText.enabled = false;
    }
        /* General Note on GetUpgrade and GetUpgradeCost
         * These two functions are using a C# lambda expression. Lambdas are constructed with a left-hand side input (which DOES NOT require an explicit type)
         * and a right-hand side output statement. In these cases, the two lambdas are not being given an explicit type, so GetUpgrade is piecing together
         * that GetUpgrade should return an Upgrade type item and GetUpgradeCost should return a float. These are both being stored in a local variable being
         * given those corresponding types -- upgrade.
         * 
         * upgrades is a vector of items of type Upgrade, so they should be searching for upgrade, which is declared and initialized in these two functions, in upgrades
         * and then returning the appropriate attribute of that item.
         * 
         * Originally, these used upgrades.First. First is a array library function which grabs the first item in the list (in this case upgrades). While that worked
         * for the sake of testing, we want these functions to find the correct upgrade in upgrades, so we will use Find instead which finds the element that matches its
         * input.
         */

        //Stuff to manage the upgrades
    public Upgrade GetUpgrade(UpgradeType upgradeType)
    {
        return upgrades.Find(upgrade => upgrade.UpgradeType == upgradeType);
    }

    //upgrade type refers to the type of tower you are upgrading
    public float GetUpgradeCost(UpgradeType upgradeType)
    {
        return upgrades.Find(upgrade => upgrade.UpgradeType == upgradeType).Cost;
    }

    //this is what is giving an error when I add other stuff.
    //THIS FUNCTION DOESNT DO ANYTHING NOW
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
