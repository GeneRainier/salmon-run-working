using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script that defines a destination node for a moving object (fish) to move towards
 */
public class Destination : MonoBehaviour
{
    public Vector3 destinationPosition;     //< The position of this destination
    public List<Destination> destinations;  //< A list of potential destinations to go to from here
    public bool finalDestination = false;   //< Whether or not this destination is the final location in the path

    /*
     * Awake is called after the initialization of gameobjects prior to the start of the game. This is used as an Initialization Function
     */
    void Awake()
    {
        destinationPosition = this.transform.position;
    }    
}
