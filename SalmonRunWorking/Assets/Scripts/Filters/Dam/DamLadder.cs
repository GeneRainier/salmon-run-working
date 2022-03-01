using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A type of Salmon Ladder to attach to a dam (a type of filter) to allow fish to pass by more easily.
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class DamLadder : TowerBase
{
    // Rates at which small, medium, and large fish should be able to pass a dam with a ladder installed
    [Range(0f, 1f)]
    public float smallCrossingRate = 0.3F;
    [Range(0f, 1f)]
    public float mediumCrossingRate = 0.9F;
    [Range(0f, 1f)]
    public float largeCrossingRate= 0.99F;
    /* Initialized in Unity
     * Project -> Assets -> Prefabs -> Filters -> Dam -> DamLadder
     * (make sure to double click on DamLadder cube symbol. 
     */

    /**
     * Awake is called after all gameObjects in the scene are initialized prior to the game starting
     */
    protected override void Awake()
    {
        // Get initialization values and set this towers basic values
        initValues = FindObjectOfType<ManagerIndex>();

        // Get the Tower Manager
        // NOTE: Due to how the Scene is set up, the Ladder needs to grab the Manager since it will not necessarily grab it from the base class
        towerManager = FindObjectOfType<TowerManager>();
    }

    #region TowerBase (Base Class / IDragAndDropObject) Implementation

    /**
     * Place the dam onto the game map
     */
    public override void Place(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        // Can only place if we are there is a dam placement location somewhere in the hit object's hierarchy
        DamPlacementLocation placementLocation = primaryHitInfo.collider.transform.root.GetComponentInChildren<DamPlacementLocation>();
        if (placementLocation != null)
        {
            placementLocation.AttachLadder(this);

            // Initialize the pass rates
            smallCrossingRate = initValues.initSets[initValues.setToUse].ladderSmallPassRate;
            mediumCrossingRate = initValues.initSets[initValues.setToUse].ladderMediumPassRate;
            largeCrossingRate = initValues.initSets[initValues.setToUse].ladderLargePassRate;
            Debug.Log("DamLadder.cs primaryHitInfo S=" + smallCrossingRate + "; M=" + mediumCrossingRate + "; L=" + largeCrossingRate);

            initValues.ladderCode = 1;
            towerManager.AddTower(this);
            turnPlaced = GameManager.Instance.Turn;
        }
    }

    /**
     * Figure out if we can place the dam at the location of a given raycast
     */
    public override bool PlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        // Must have hit something
        if (primaryHitInfo.collider)
        {
            // Can only place if we are there is a dam placement location somewhere in the hit object's hierarchy
            DamPlacementLocation placementLocation = primaryHitInfo.collider.transform.root.GetComponentInChildren<DamPlacementLocation>();

            // Thing we hit must be a dam placement location
            if (placementLocation != null)
            {
                // Only return true if the placement location is not already in use
                return (placementLocation.inUse && !placementLocation.HasLadder);
            }
        }

        return false;
    }

    /**
     * Apply effects of this tower
     * 
     * The Dam has its own unique placement, but is a tower nonetheless
     */
    protected override void ApplyTowerEffect()
    {
        return;
    }

    /**
     * Determines whether a tower placement is valid
     * 
     * @param primaryHitInfo The raycast info from the main camera raycast
     * @param secondaryHitInfo The raycast info from the bounds of the tower
     * 
     * The Dam has its own unique placement, but is a tower nonetheless
     */
    protected override bool TowerPlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        return true;
    }

    /**
     * Places a tower into the environment   
     * 
     * @param primaryHitInfo The raycast info from the main camera raycast
     * @param secondaryHitInfo The raycast info from the bounds of the tower
     * 
     * The Dam has its own unique placement, but is a tower nonetheless
     */
    protected override void PlaceTower(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        return;
    }

    #endregion
}
