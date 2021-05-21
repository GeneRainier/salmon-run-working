using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSpawner : MonoBehaviour
{
    public List<Destination> initialDestinations;   //< List of the initial destinations pathing object can move towards
    [SerializeField] private GameObject moverPrefab;    //< A prefab that will be spawned in to path through the level
    [SerializeField] private int numberOfSpawns;            //< Number of objects to spawn
    [SerializeField] private float delay;                     //< The delay in seconds between spawns

    private void Start()
    {
        StartCoroutine("Spawn");
    }

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
