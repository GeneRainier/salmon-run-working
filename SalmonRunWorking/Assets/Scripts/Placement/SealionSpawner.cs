using System.Collections;
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

    // The ManagerIndex with initialization values for a given tower
    public ManagerIndex initializationValues;

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
        // Get initialization values and set this towers basic values
        initializationValues = FindObjectOfType<ManagerIndex>();
        GameEvents.onTurnUpdated.AddListener(SpawnSealion);
        damPlacementLocation = GetComponent<DamPlacementLocation>();
        turnsBeforeShowing = initializationValues.sealionAppearanceTime;
    }

    public void Update()
    {
        if (Input.GetKeyDown("m"))
        {
            sealionClone = Instantiate(sealionPrefab, spawnLocations[0].transform.position, Quaternion.Euler(270, 0, 0));
            locationInUse = true;
        }
    }

    private void SpawnSealion()
    {
        if (damPlacementLocation.inUse && GameManager.Instance.Turn >= damPlacementLocation.PlacementTurn + turnsBeforeShowing)
        {
            if (!locationInUse)
            {
                sealionClone = Instantiate(sealionPrefab, spawnLocations[0].transform.position, Quaternion.Euler(270, 0, 0));
                locationInUse = true;
            }
        }
    }
}
