using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class to create a simple spawner that creates a specified number of gameobjects based on a prefab. Used in conjunction with Movement.cs for testing
 * movement methods
 * 
 * Authors: Benjamin Person
 */
public class GenericSpawner : MonoBehaviour
{
    public List<Destination> initialDestinations;   //< List of the initial destinations pathing object can move towards
    [SerializeField] private GameObject moverPrefab = null;    //< A prefab that will be spawned in to path through the level
    [SerializeField] private int numberOfSpawns = 100;            //< Number of objects to spawn
    [SerializeField] private float delay = 2.0f;                     //< The delay in seconds between spawns

    /*
     * Start is called prior to the first frame update
     */
    private void Start()
    {
        StartCoroutine("Spawn");
    }

    /*
     * Coroutine to spawn the specified number of fish over a specified length of time
     */
    private IEnumerator Spawn()
    {
        int i = 0;
        while (i < numberOfSpawns)
        {
            Instantiate(moverPrefab, this.gameObject.transform);
            i++;
            yield return new WaitForSeconds(delay);
        }
    }
}
