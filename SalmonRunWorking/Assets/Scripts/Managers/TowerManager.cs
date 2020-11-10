using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TowerType
{
    Angler, Ranger, Dam, 
    Ladder, SeaLion, Truck, 
    Tunnel
}

public class TowerManager : MonoBehaviour
{
    [SerializeField] private List<Tower> towers;
    [SerializeField] private List<FishermanTower> anglers;

    public Tower GetTower(TowerType towerType)
    {
        return towers.First(tower => tower.TowerType == towerType);
    }

    public float GetTowerCost(TowerType towerType)
    {
        return towers.First(tower => tower.TowerType == towerType).Cost;
    }

    public void UpdateColors()
    {
        towers.ForEach(tower => tower.TowerUI.UpdateColor());
    }

    public void AddAngler(FishermanTower angler)
    {
        anglers.Add(angler);
    }

    public List<FishermanTower> GetAnglers()
    {
        return anglers;
    }
}

[Serializable]
public class Tower
{
    [SerializeField] private TowerType towerType;
    [SerializeField] private TowerUI towerUI;
    [SerializeField] private bool enabled;
    [SerializeField] private float cost;

    public TowerType TowerType => towerType;

    public TowerUI TowerUI => towerUI;

    public float Cost => cost;

    public void Enable()
    {
        enabled = true;
    }

    public void Disable()
    {
        enabled = false;
    }

    public bool CanAfford()
    {
        return enabled && ManagerIndex.MI.MoneyManager.CanAfford(cost);
    }
}