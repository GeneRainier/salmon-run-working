using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Managers visualization of placement locations for placeable objects in the game.
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class PlacementVisualizationManager : MonoBehaviour
{
    public static PlacementVisualizationManager Instance { get; private set; }      //< Singleton instance

    /**
     * Awake is called after the initialization of every gameObject prior to the game Starting. Used as an Initialization function
     */
    private void Awake()
    {
        // Manage singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one PlacementVisualizationManager in the scene! Deleting...");
            Destroy(this);
        }
    }

    /**
     * Turn a subset of visualizations on or off
     * 
     * @param type System.Type The type of the object we are displaying visualizations for
     * @param activate bool True if we want to activate the visualizations, false if we want to deactivate
     */
    public void DisplayVisualization(System.Type type, bool activate)
    {
        // Look through different types to figure out which visualizations to turn on and off
        if (type == typeof(Dam))
        {
            // Show placement locations for dams
            DamPlacementLocation.SetDamVisualizations(activate);
        }
        else if (type == typeof(DamLadder))
        {
            // Show placement locations for dam ladders
            DamPlacementLocation.SetLadderVisualizations(activate);
        }
    }
}
