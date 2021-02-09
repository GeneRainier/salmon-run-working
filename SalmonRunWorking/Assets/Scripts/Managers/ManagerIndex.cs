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
    // Angler
    public int anglerRadius;
    public float defaultSmallCatchRate;
    public float defaultMediumCatchRate;
    public float defaultLargeCatchRate;

    // Ranger
    public int rangerRadius;
    public float defaultSmallModifier;
    public float defaultMediumModifier;
    public float defaultLargeModifier;
    public float defaultSuccessRate;

    // Pass Rates and Money
    public float defaultDamPassRate;
    public float defaultLadderSmallPassRate;
    public float defaultLadderMediumPassRate;
    public float defaultLadderLargePassRate;
    public float startingMoney;
    public int sealionAppearanceTime;       /// This int represents number of rounds until a sealion appears

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
