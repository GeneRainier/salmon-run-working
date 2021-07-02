using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/*
 * Authors: Benjamin Person (Editor 2020)
 */

// Types of upgrades
public enum UpgradeType
{
    SalmonLadderUp, FisherUp
}

/*
 * The operating manager script for providing upgrades for towers
 */
public class UpgradeManager : MonoBehaviour
{
    public ManagerIndex initializationValues;           //< The ManagerIndex with initialization values for a given tower

    public Button upgradeButtonTray;            //< The UI button used to open the upgrade menu

    // The three ladder swap buttons
    public Button originalLadderButton;
    public Button upgradeLadderOneButton;
    public Button upgradeLadderTwoButton;

    // The two ladder upgrade buttons
    public Button ladderUp1Button;
    public Button ladderUp2Button;

    // Images to represent each of the Salmon Ladders in the Ladder submenu
    public Image salmonLadder;
    public Image salmonRamp;
    public Image salmonElevator;


    // The three fisherman catch rate buttons
    public Button upgradeSmallCatchButton;
    public Button upgradeMediumCatchButton;
    public Button upgradeLargeCatchButton;

    private bool finalUpgrade => numberOfUpgrades >= 2;         //< Bool tracking if an upgrade has been purchased for a tower

    private int numberOfUpgrades = 1;           //< The number of upgrades that can be purchased for a tower

    // Check to see if you have already bought them
    public bool firstPurchase1 = true;
    public bool firstPurchase2 = true;

    public UpgradeUI upgradeUI;         //< Reference to the UpgradeUI script

    // Upgrade prefabs
    private GameObject originalLadder;
    public GameObject upgradeLadderOne;
    public GameObject upgradeLadderTwo;

    public GameObject OriginalLadder
    {
        get => originalLadder;
        set => originalLadder = value;
    }

    // Fisherman catch rate values
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

    // Bool to see if maximum catch rate has been reached
    public bool smallRateMax = false;
    public bool mediumRateMax = false;
    public bool largeRateMax = false;

    public bool isAFisherman = false;       //< Will be true if you have a fisherman

    private GameObject firstLadder;         //< The initial ladder a player places into the game scene
    
    [SerializeField] private List <Upgrade> upgrades = null;       //< List of Upgrades that can be purchased

    [SerializeField] private TowerManager theTowerManager = null;  //< Reference to the TowerManager

    public TextMeshProUGUI purchaseText;        //< The text associated with purchasing an upgrade

    /*
     * Start is called prior to the first frame update
     */
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

    /*
     * Update is called every frame update
     */
    void Update()
    {
        //Debug.Log("SmallRate: " + smallRate);
        // If the dam has a ladder and if its PlaceState and if there are more upgrades available, then the button appears
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

    /*
     * Upgrades the Salmon Ladder to whatever variant the player has selected
     */
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
            // Sets the right later active
            originalLadder.SetActive(false);
            upgradeLadderTwo.SetActive(false);
            upgradeLadderOne.SetActive(true);

            // Tell upgrdeUI it was bought
            upgradeUI.Ladder1Bought();

            // Buys the ladder if not already bought
            if (firstPurchase1)
            {
                upgradeUI.Purchase();
                firstPurchase1 = false;
                ladderUp1Button.interactable = false;
            }

            salmonRamp.enabled = true;
            salmonElevator.enabled = false;
            salmonLadder.enabled = false;

            // Making the right buttons clickable
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
            // Sets the right later active
            originalLadder.SetActive(false);
            upgradeLadderOne.SetActive(false);
            upgradeLadderTwo.SetActive(true);

            upgradeUI.Ladder2Bought();

            // Buys the ladder if not already bought
            if (firstPurchase2)
            {
                upgradeUI.Purchase();
                firstPurchase2 = false;
                ladderUp2Button.interactable = false;
            }

            salmonRamp.enabled = false;
            salmonElevator.enabled = true;
            salmonLadder.enabled = false;

            // Making the right buttons clickable
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

    /*
     * Switches out a Salmon Ladder for for another variant the player has purchased before
     */
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

    /*
     * Upgrades the catch rates for all the fishermen in the scene
     */
    public void UpgradeButtonFisherman()
    {
        if (EventSystem.current.currentSelectedGameObject.name == upgradeSmallCatchButton.name)
        {
            Debug.Log("Entered Fishermen Upgrade");
            // Should be able to get rid of this first if statement if the UpgradeUI works properly and turns it off when max is hit
            if (smallRate < 1.0f)
            {
                Debug.Log("If is good");
                upgradeUI.Purchase();
                smallRate += 0.1f;
                if (smallRate == 1.0f)
                {
                    smallRateMax = true;
                }
                foreach (AnglerTower towers in theTowerManager.GetAnglers())
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

    /*
     * This makes the "purchased!" text pop up and then fade after a few seconds. Will throw in a particle effect in here as well.
     */
    public void PurchasedPopUp()
    {
        StartCoroutine(MyCoroutine());
    }

    /*
     * Coroutine to enable the purchased upgrade menu text
     */
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

    /*
     * Retrieves an upgrade from the list of potential upgrades
     * 
     * @param upgradeType The kind of upgrade we are searching for
     * @return Upgrade The upgrade we are searching for
     */
    public Upgrade GetUpgrade(UpgradeType upgradeType)
    {
        return upgrades.Find(upgrade => upgrade.UpgradeType == upgradeType);
    }

    /*
     * Retrieves the cost of a given upgrade type
     * 
     * @param upgradeType The kind of upgrade we are searching for
     * @return float The cost of the kind of upgrade
     */
    public float GetUpgradeCost(UpgradeType upgradeType)
    {
        return upgrades.Find(upgrade => upgrade.UpgradeType == upgradeType).Cost;
    }

    // This is what is giving an error when I add other stuff.
    //THIS FUNCTION DOESNT DO ANYTHING NOW
    /*
     * Updates each of the buttons associated with upgrading a tower
     */
    public void UpdateButtons()
    {
        upgrades.ForEach(upgrade => upgrade.UpgradeUI.UpdateButton());
    }
}

/*
 * Class that describes an actual upgrade rather than a collection of an UpgradeType
 */
[Serializable]
public class Upgrade
{
    [SerializeField] private UpgradeType upgradeType = UpgradeType.SalmonLadderUp;       //< What kind of Upgrade this is
    [SerializeField] private UpgradeUI upgradeUI = null;           //< The UI associated with this singular upgrade
    //[SerializeField] private bool enabled;
    [SerializeField] private float cost = 0.0f;                    //< The cost of this upgrade

    public UpgradeType UpgradeType => upgradeType;          //< A reference to the upgradeType of this Upgrade

    public UpgradeUI UpgradeUI => upgradeUI;                //< A reference to the UpgradeUI of this Upgrade

    public float Cost => cost;                              //< A reference to the cost of this Upgrade

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
