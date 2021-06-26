using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Handles fish evolution over generations
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class EvolutionManager : MonoBehaviour
{
    public static EvolutionManager Instance { get; private set; }       //< Singleton instance

    /**
     * Awake is called after the initialization of gameobjects prior to the start of the game. This is used as an Initialization function
     */
    private void Awake()
    {
        // Handle singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one EvolutionManager in the scene! Deleting...");
            Destroy(this);
        }
    }

    /**
     * Start is called before the first frame update
     */
    private void Start()
    {
        
    }
}
