using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerIndex : MonoBehaviour
{
    public static ManagerIndex MI;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private ColorManager colorManager;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private TowerManager towerManager;
    [SerializeField] private UpgradeManager upgradeManager;

    // Variables meant as initializations on start up
    // Boolean tracking whether or not to make the output file
    public bool makeOutputFile = false;

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
    public int sealionAppearanceTime;       /// This int represents number of rounds until a sealion appears

    // Sealion Catch Rates
    public float sealionFemaleCatchRate;
    public float sealionMaleCatchRate;

    public int nestingSites;    //< The number of nesting sites available at the fish destination

    // Miscellaneous Output File Counters
    public int upperAnglerCount = 0;
    public int lowerAnglerCount = 0;
    public int upperManagedAnglerCount = 0;
    public int lowerManagedAnglerCount = 0;
    
    public int rangerCount = 0;

    public int damPresent = 0;
    public int ladderCode = 0;

    public int sealionPresent = 0;

    // Costs
    //public float anglerCost;
    //public float rangerCost;
    //public float boxLadderCost;
    //public float streamLadderCost;
    //public float elevatorLadderCost;
    //public float ladderChangeCost;

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
