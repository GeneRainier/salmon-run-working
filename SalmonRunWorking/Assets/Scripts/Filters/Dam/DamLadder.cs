using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A type of Salmon Ladder to attach to a dam (a type of filter) to allow fish to pass by more easily.
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class DamLadder : MonoBehaviour, IDragAndDropObject
{
    public ManagerIndex initializationValues;   //< The ManagerIndex with initialization values for a given tower

    public int turnPlaced = 0;                  //< The turn this tower was placed in the level

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
    private void Awake()
    {
        // Get initialization values and set this towers basic values
        initializationValues = FindObjectOfType<ManagerIndex>();
    }

    #region IDragAndDropObject Implementation

    /**
     * Place the dam onto the game map
     */
    public void Place(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        // Can only place if we are there is a dam placement location somewhere in the hit object's hierarchy
        DamPlacementLocation placementLocation = primaryHitInfo.collider.transform.root.GetComponentInChildren<DamPlacementLocation>();
        if (placementLocation != null)
        {
            placementLocation.AttachLadder(this);

            // Initialize the pass rates
            smallCrossingRate = initializationValues.ladderSmallPassRate;
            mediumCrossingRate = initializationValues.ladderMediumPassRate;
            largeCrossingRate = initializationValues.ladderLargePassRate;
            Debug.Log("DamLadder.cs primaryHitInfo S=" + smallCrossingRate + "; M=" + mediumCrossingRate + "; L=" + largeCrossingRate);

            initializationValues.ladderCode = 1;
            turnPlaced = GameManager.Instance.Turn;
        }
    }

    /**
     * Figure out if we can place the dam at the location of a given raycast
     */
    public bool PlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
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

    #endregion
}
