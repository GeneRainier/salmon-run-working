using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Authors: Benjamin Person (Editor 2020)
 */

/*
 * Types of Towers in the game
 */
public enum TowerType
{
    Angler, Ranger, Dam, 
    Ladder, SeaLion, Truck, 
    Tunnel
}

/*
 * Manager script that handles how towers interact with one another and with purchase and placement
 */
public class TowerManager : MonoBehaviour
{
    [SerializeField] private List<Tower> towers;            //< A list of all the towers in the scene
    [SerializeField] private List<FishermanTower> anglers;  //< A list of all the fishermen in the scene

    /*
     * Gets the first tower in the list of towers with a given type
     * 
     * @param towerType The type of tower we are trying to obtain
     * @return Tower A single reference to a tower
     */
    public Tower GetTower(TowerType towerType)
    {
        return towers.First(tower => tower.TowerType == towerType);
    }

    /*
     * Gets the cost of a particular kind of tower based on the tower's type
     * 
     * @param towerType The type of tower we want to check the cost of
     * @return float The cost of placing the tower
     */
    public float GetTowerCost(TowerType towerType)
    {
        return towers.First(tower => tower.TowerType == towerType).Cost;
    }

    /*
     * Loops through each tower in the towers list and updates the color
     */
    public void UpdateColors()
    {
        towers.ForEach(tower => tower.TowerUI.UpdateColor());
    }

    /*
     * Adds a fishermen to the list of fisherment for the purposes of applying upgrades
     * 
     * @param angler The fisherman that has just been placed in the scene which needs to be added to the list
     */
    public void AddAngler(FishermanTower angler)
    {
        anglers.Add(angler);
    }

    /*
     * Obtains the list of existing fishermen
     * 
     * @return List<FishermentTower> A list of scripts for each of the existing fishermen
     */
    public List<FishermanTower> GetAnglers()
    {
        return anglers;
    }
}

/*
 * Class that summarizes the properties of a single tower
 */
[Serializable]
public class Tower
{
    public ManagerIndex initializationValues;           //< The ManagerIndex with initialization values for a given tower

    [SerializeField] private TowerType towerType;       //< The type of tower this is
    [SerializeField] private TowerUI towerUI;           //< The UI functions this tower can take use of
    [SerializeField] private bool enabled;              //< Whether or not the tower's effects are currently active or not
    [SerializeField] private float cost;                //< The cost of placing this tower

    public TowerType TowerType => towerType;            //< Reference to the towerType for this singular tower
    
    public TowerUI TowerUI => towerUI;                  //< Reference to the TowerUI for this singular tower

    public float Cost => cost;                          //< Reference to the cost of this singular tower

    /*
     * Sets the tower to be enabled
     */
    public void Enable()
    {
        enabled = true;
    }

    /*
     * Sets the tower to be disabled
     */
    public void Disable()
    {
        enabled = false;
    }

    /*
     * Checks whether or not the tower can currently be purchased by the player
     * 
     * @return bool True if the player has enough money to purchase the tower
     */
    public bool CanAfford()
    {
        return enabled && ManagerIndex.MI.MoneyManager.CanAfford(cost);
    }
}