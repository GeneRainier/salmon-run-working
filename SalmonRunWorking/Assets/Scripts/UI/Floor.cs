using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * Script attached to whatever object is considered the "floor" for the game.
 * 
 * Currently, this is used for the drag & drop UI to get the location on the ground that the current mouse position corresponds to.
 * However, this class may also be used for other purposes at some point.
 * 
 * Author: Jacob Cousineau, Benjamin Person (Editor 2020)
 */
public class Floor : MonoBehaviour 
{
    public static Floor Instance { get; private set; }      //< Singleton instance to be used for access from other scripts

    private new BoxCollider collider;           //< Ground collider

    /**
     * Awake is called after the initialization of gameobjects prior to the start of the game. This is used as an Initialization Function
     */
    private void Awake()
    {
        // Create the singleton reference
        if (Instance == null)
        {
            Instance = this;
        }
        // If another reference already exists, log an error and delete this version -- we only want one ground in the scene at any time
        else
        {
            Debug.LogError("More than one ground script in the scene!");
            Destroy(this);
        }
    }

    /**
     * Called before initial update
     */
    private void Start()
    {
        // Generate a collider for the ground
        collider = gameObject.AddComponent<BoxCollider>();
    }
}
