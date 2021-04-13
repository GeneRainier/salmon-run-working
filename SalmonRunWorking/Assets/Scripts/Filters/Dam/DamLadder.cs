using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamLadder : MonoBehaviour, IDragAndDropObject
{
    // The ManagerIndex with initialization values for a given tower
    public ManagerIndex initializationValues;

    // rate at which small, medium, and large fish should be able to pass a dam with a ladder installed
    [Range(0f, 1f)]
    public float smallCrossingRate = 0.3F;
    [Range(0f, 1f)]
    public float mediumCrossingRate = 0.9F;
    [Range(0f, 1f)]
    public float largeCrossingRate= 0.99F;
    // initialized in Unity
    // Project -> Assets -> Prefabs -> Filters -> Dam -> DamLadder
    // (make sure to double click on DamLadder cube symbol. 

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
        // can only place if we are there is a dam placement location somewhere in the hit object's hierarchy
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
        }
    }

    /**
     * Figure out if we can place the dam at the location of a given raycast
     */
    public bool PlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        // must have hit something
        if (primaryHitInfo.collider)
        {
            // can only place if we are there is a dam placement location somewhere in the hit object's hierarchy
            DamPlacementLocation placementLocation = primaryHitInfo.collider.transform.root.GetComponentInChildren<DamPlacementLocation>();

            // thing we hit must be a dam placement location
            if (placementLocation != null)
            {
                // only return true if the placement location is not already in use
                return (placementLocation.inUse && !placementLocation.HasLadder);
            }
        }

        return false;
    }

    #endregion
}
