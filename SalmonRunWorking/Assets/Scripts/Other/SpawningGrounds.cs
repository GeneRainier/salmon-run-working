using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Attached to final destination nodes in a fish path to despawn the fish and track the traits of the fish that have succeeded in their run
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class SpawningGrounds : MonoBehaviour
{
    public ManagerIndex initializationValues;       //< The ManagerIndex with initialization values for a given tower

    public int numNestingSights;        //< How many fish this spawning grounds has the capacity for
    // initialized in Assets -> Prefabs -> Art -> Fish -> Old -> EndOfLevel
    // if you don't want to initialize in Unity, make it private

    // How many males and females have been taken in
    private int males;
    private int females;

    /**
     * Start is called before the first frame update
     */
    void Start()
    {
        // Get initialization values and set this towers basic values
        initializationValues = FindObjectOfType<ManagerIndex>();

        numNestingSights = initializationValues.nestingSites;

        // Subscribe to onEndRun event
        GameEvents.onEndRun.AddListener(Clear);
    }

    /**
     * Update is called once per frame
     */
    void Update()
    {
        
    }

    /**
     * Handle trigger collisions with other objects
     */
    private void OnTriggerEnter(Collider other)
    {
        // Figure out if the thing that hit us is actually a fish
        Fish fish = other.GetComponentInChildren<Fish>();
        if (fish != null)
        {
            // Need to check if this is a male or female
            bool isMale = fish.GetGenome().IsMale();

            // Check if there is a nesting sight available for this fish
            // If so, tell the fish it has reached the spawning grounds
            if (isMale && males < numNestingSights)
            {
                fish.ReachSpawningGrounds();
                males++;
            }
            else if (!isMale && females < numNestingSights)
            {
                // If so, it has officially reached the spawning grounds
                fish.ReachSpawningGrounds();
                females++;
            }
            else
            {
                fish.Killed();
            }
        }
    }

    /**
     * Clear the spawning ground of all fish that are currently here
     */
    private void Clear()
    {
        males = 0;
        females = 0;
    }
}
