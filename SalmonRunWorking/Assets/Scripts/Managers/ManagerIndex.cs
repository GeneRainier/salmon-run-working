using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A manager class that tracks the actions of the other manager scripts and tracks values for printing output files
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class ManagerIndex : MonoBehaviour
{
    public static ManagerIndex MI;          //< Reference to the ManagerIndex

    // References to the other manager scripts
    [SerializeField] private GameManager gameManager = null;
    [SerializeField] private ColorManager colorManager = null;
    [SerializeField] private TimeManager timeManager = null;
    [SerializeField] private MoneyManager moneyManager = null;
    [SerializeField] private MenuManager menuManager = null;
    [SerializeField] private TowerManager towerManager = null;
    [SerializeField] private UpgradeManager upgradeManager = null;

    // Variables meant as initializations on start up
    // Boolean tracking whether or not to make the output file
    public bool makeOutputFile = false;

    [Header("Output File Tracker Values")]
    // Miscellaneous Output File Counters
    public int upperAnglerCount = 0;
    public int lowerAnglerCount = 0;
    public int upperManagedAnglerCount = 0;
    public int lowerManagedAnglerCount = 0;

    public int rangerCount = 0;

    public int damPresent = 0;
    public int ladderCode = 0;

    public int sealionPresent = 0;

    [SerializeField] public List<initialValues> initSets;  //< The list of all the potential initialization settings we have created
    public int setToUse = 0;                               //< The index of the initialization set we are going to use for this round of the game

    public GameManager GameManager => gameManager;

    public ColorManager ColorManager => colorManager;

    public TimeManager TimeManager => timeManager;

    public MoneyManager MoneyManager => moneyManager;

    public MenuManager MenuManager => menuManager;

    public TowerManager TowerManager => towerManager;

    public UpgradeManager UpgradeManager => upgradeManager;

    private void Awake()
    {
        if (!MI) MI = this;
        else Destroy(gameObject);
    }
}

[System.Serializable]
public struct initialValues
{
    // Angler
    public int anglerRadius;
    public float anglerSmallCatchRate;
    public float anglerMediumCatchRate;
    public float anglerLargeCatchRate;

    // Ranger
    public int rangerRadius;
    public float rangerSmallModifier;
    public float rangerMediumModifier;
    public float rangerLargeModifier;
    public float rangerSuccessRate;

    // Pass Rates and Money
    public float defaultDamPassRate;
    public float ladderSmallPassRate;
    public float ladderMediumPassRate;
    public float ladderLargePassRate;

    public float rampSmallPassRate;
    public float rampMediumPassRate;
    public float rampLargePassRate;

    public float liftSmallPassRate;
    public float liftMediumPassRate;
    public float liftLargePassRate;

    public float startingMoney;
    public int sealionAppearanceTime;       //< This int represents number of rounds until a sealion appears

    // Sealion Catch Rates
    public float sealionFemaleCatchRate;
    public float sealionMaleCatchRate;

    public int nestingSites;    //< The number of nesting sites available at the fish destination

    // Costs
    //public float anglerCost;
    //public float rangerCost;
    //public float boxLadderCost;
    //public float streamLadderCost;
    //public float elevatorLadderCost;
    //public float ladderChangeCost;
}
