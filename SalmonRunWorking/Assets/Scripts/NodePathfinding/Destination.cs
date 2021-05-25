﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    public Vector3 destinationPosition;     //< The position of this destination
    public List<Destination> destinations;  //< A list of potential destinations to go to from here
    public bool finalDestination = false;   //< Whether or not this destination is the final location in the path

    private int fishLayerMask = 1 << 8;     //< The layer that the fish are located on

    void Awake()
    {
        destinationPosition = this.transform.position;
    }

    
}