using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * Class that handles the placement of the dam in the level including potential dam locations and visualizations for the player to see
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class DamPlacementLocation : MonoBehaviour
{
    private static List<DamPlacementLocation> allLocations = new List<DamPlacementLocation>();  //< List of all dam placement locations on the map

    public MeshRenderer mainMeshRenderer;           //< meshRenderer for the dam placement visual

    public MeshRenderer ladderMeshRenderer;         //< meshRenderer for the ladder placement visual

    // Drop off box for fish used by the dam once it is actually placed
    // Since the dam is being dropped in, we need to hold onto this until the dam is actually placed
    public BoxCollider dropOffBox;

    public bool inUse { get; private set; } = false;        //< Is this location currently being used?

    public bool HasLadder { get; private set; } = false;    //< Does this location currently have a ladder attached?

    // get and set can be defined here or we can defaults
    // do not have write getter and setter, 
    // bool so get public, and set private in this case. 
    // get; set; would have both as public

    private Dam currentDam;             //< Dam component currently being used at this location (if any)

    private int placementTurn = -1;     //< Keeping track of the turn the Dam is placed

    public int PlacementTurn => placementTurn;


    #region Major Monobehaviour functions

    /**
     * Awake is called after initialization of gameobjects prior to the start of the game. Used as an Initialization function
     */
    private void Awake()
    {
        // Add each placement location to our list as it initializes
        allLocations.Add(this);

        // Location will not be in use at the beginning
        inUse = false;

        // Ensure that we have visualizers for the dam and the ladder
        if (mainMeshRenderer == null || ladderMeshRenderer == null)
        {
            Debug.LogError("A MeshRenderer on a DamPlacementLocation is missing!");
        }

        // Turn the meshRenderers off by default
        mainMeshRenderer.enabled = false;
        ladderMeshRenderer.enabled = false;
    }

    #endregion

    #region Dams and Attachment

    /**
     * Attach a dam to this location
     * 
     * @param dam The dam being placed in the level
     */
    public void AttachDam(Dam dam)
    {
        if (!inUse)
        {

            // Position the dam within hierarchy and game space to match the dam location
            Transform transform1;
            (transform1 = dam.transform).SetParent(transform.parent);
            var transform2 = transform;
            transform1.position = transform2.position;
            transform1.rotation = transform2.rotation;
            transform1.localScale = transform2.localScale;

            dam.Activate(dropOffBox);

            // The location is now in use and cannot be used by any other dam
            inUse = true;

            // Keep track of the dam for use later
            currentDam = dam;
            
            //print("Turn attached");

            //placementTurn = GameManager.Instance.Turn;

        }
        else
        {
            Debug.Log("Trying to attach dam to in-use location -- this line should not be reached!");
        }

    }

    /**
     * Attach a ladder to the dam at this location (if there is one)
     * 
     * @param damLadder The dam ladder being placed in the level
     */
    public void AttachLadder(DamLadder damLadder)
    {
        // Can only attach if there is a dam here with no ladder
        if (!inUse || currentDam == null || HasLadder) return;
        
        // Put the ladder in the correct location and parent it to the current dam
        Transform transform2;
        (transform2 = damLadder.transform).SetParent(currentDam.transform);
        var transform1 = ladderMeshRenderer.transform;
        transform2.localPosition = transform1.localPosition;
        transform2.localRotation = transform1.localRotation;
        transform2.localScale = transform1.localScale;

        // Tell the dam that a ladder is being attached
        currentDam.AddLadder(damLadder);

        // This dam location now has a ladder attached to it
        HasLadder = true;

        print("Turn attached");

        placementTurn = GameManager.Instance.Turn;
    }

    /*
     * Removes the Dam by way of setting the inUse boolean to false
     */
    public void RemoveDam()
    {
        inUse = false;
    }

    /*
     * Removes the Ladder by way of setting the HasLadder boolean to false
     */
    public void RemoveLadder()
    {
        HasLadder = false;
    }

    #endregion

    #region Visualization (Static)

    /**
     * Activate visualization for all dam placement locations
     * 
     * @param True if we want to activate visualizations, false otherwise
     */
    public static void SetDamVisualizations(bool activate)
    {
        foreach (DamPlacementLocation placementLocation in allLocations)
        {
            Debug.Log(activate);
            // If the placement location is in use and we're trying to activate it, don't do so
            // because you shouldn't be able to place anything there
            if (!activate || !placementLocation.inUse)
            {
                placementLocation.mainMeshRenderer.enabled = activate;
            }
            
        }
    }

    /**
     * Activate visualization for all ladder placement locations
     * 
     * @param True if we want to activate visualizations, false otherwise
     */
    public static void SetLadderVisualizations(bool activate)
    {
        foreach (DamPlacementLocation placementLocation in allLocations)
        {
            // Only show visualizations where the placement location is in use but there is no ladder
            if (!activate || (placementLocation.inUse && !placementLocation.HasLadder))
            {
                placementLocation.ladderMeshRenderer.enabled = activate;
            }

        }
    }

    #endregion
}
