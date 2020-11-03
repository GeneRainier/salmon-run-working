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

    public GameManager GameManager => gameManager;

    public ColorManager ColorManager => colorManager;

    public TimeManager TimeManager => timeManager;

    public MoneyManager MoneyManager => moneyManager;

    public MenuManager MenuManager => menuManager;

    public TowerManager TowerManager => towerManager;

    private void Awake()
    {
        if (!MI) MI = this;
        else Destroy(gameObject);
    }
}
