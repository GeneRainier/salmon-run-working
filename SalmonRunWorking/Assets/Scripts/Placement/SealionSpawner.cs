﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SealionSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnLocations;
    [SerializeField] private GameObject sealionPrefab;
    [SerializeField] private GameObject sealionClone;

    [SerializeField] private bool locationInUse;
    
    [SerializeField] private int turnsBeforeShowing = 3;

    private DamPlacementLocation damPlacementLocation;

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

    private void Awake()
    {
        GameEvents.onTurnUpdated.AddListener(SpawnSealion);
        damPlacementLocation = GetComponent<DamPlacementLocation>();
    }

    private void SpawnSealion()
    {
        if (damPlacementLocation.inUse && GameManager.Instance.Turn >= damPlacementLocation.PlacementTurn + turnsBeforeShowing)
        {
            if (!locationInUse)
            {
                sealionClone = Instantiate(sealionPrefab, spawnLocations[0].transform.position, Quaternion.Euler(240, 0, 0));
                locationInUse = true;
            }
        }
    }
}
