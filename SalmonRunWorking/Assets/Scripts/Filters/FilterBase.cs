﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A filter is an object in the game that applies an effect to a fish as it passes through.
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
[RequireComponent(typeof(Collider))]
public abstract class FilterBase : MonoBehaviour
{
    protected bool active = false;      //< Is this filter currently active

    private Collider myCollider;        //< The collider on the filter object

    /**
     * Awake is called after all gameObjects in the scene are initialized prior to the game starting
     */
    private void Awake()
    {
        // Get component refs
        myCollider = GetComponent<Collider>();
    }

    /**
     * Handle objects entering the collider
     */
    private void OnTriggerEnter(Collider other)
    {
        // Only care about triggers if the filter is active
        if (active)
        {
            // Check what hit us
            // Only care if it's a fish
            Fish f = other.gameObject.GetComponentInChildren<Fish>();
            if (f != null)
            {
                ApplyFilterEffect(f);
            }
        }
    }

    /**
     * Effects of the filter will be implemented here by child classes
     */
    protected abstract void ApplyFilterEffect(Fish fish);
}
