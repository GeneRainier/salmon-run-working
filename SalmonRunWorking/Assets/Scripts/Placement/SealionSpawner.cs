﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script that handles the spawning of sealions after a dam or other river filter has been placed
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class SealionSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnLocations = null;        //< List of locations for the sealion to spawn at
    [SerializeField] private GameObject sealionPrefab = null;          //< The prefab for the sealion tower
    [SerializeField] private GameObject sealionClone = null;           //< GameObject sealion to instantiate

    [SerializeField] private bool locationInUse;                //< Trait of each location sealions can spawn for whether it has spawned sealions or not
    
    [SerializeField] private int turnsBeforeShowing = 3;        //< How long the sealion takes before appearing after a filter is placed

    private DamPlacementLocation damPlacementLocation;          //< The location a dam can be placed in the level

    public ManagerIndex initializationValues;                   //< The ManagerIndex with initialization values for a given tower

    /*
    private void Update()
    {
        if (GetComponent<DamPlacementLocation>().inUse)
        {
            if (locationInUse == false)
            {
                Invoke("SpawnSealion", 5f);
            }
        }
        //myObject.GetComponent<SealionSpawner>().SpawnSealion();
        //just here to save the code
    }
    */

    /*
     * Awake is called after the initialization of gameobjects prior to the start of the game. This is used as an Initialization Function
     */
    private void Awake()
    {
        // Get initialization values and set this towers basic values
        initializationValues = FindObjectOfType<ManagerIndex>();
        GameEvents.onTurnUpdated.AddListener(SpawnSealion);
        damPlacementLocation = GetComponent<DamPlacementLocation>();
        turnsBeforeShowing = initializationValues.sealionAppearanceTime;
    }

    /*
     * Update is called every frame update
     */
    public void Update()
    {
        // Pressing m spawns a sealion for testing purposes
        if (Input.GetKeyDown("m"))
        {
            sealionClone = Instantiate(sealionPrefab, spawnLocations[0].transform.position, Quaternion.Euler(270, 0, 0));
            locationInUse = true;
        }
    }

    /*
     * Spawns a sealion at any of the potential spawn locations in the level
     */
    private void SpawnSealion()
    {
        if (damPlacementLocation.HasLadder && GameManager.Instance.Turn >= damPlacementLocation.PlacementTurn + turnsBeforeShowing)
        {
            if (!locationInUse)
            {
                sealionClone = Instantiate(sealionPrefab, spawnLocations[0].transform.position, Quaternion.Euler(270, 0, 0));
                locationInUse = true;
            }
        }
    }
}
