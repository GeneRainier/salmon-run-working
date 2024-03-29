﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class that holds layer-related utilities and string constants for layer names.
 * 
 * UPDATE THIS IF YOU UPDATE THE LAYER NAMES.
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class Layers
{
    // The custom layers that have been created for the project
    public const string IGNORE_RAYCAST_LAYER_NAME = "Ignore Raycast";
    public const string TERRAIN_LAYER_NAME = "Terrain";
    public const string FLOOR_LAYER_NAME = "Floor";
    public const string FISH_LAYER_NAME = "Fish";
    public const string PLACED_OBJECTS = "PlacedObjects";

    /**
     * Move a gameobject and all of its children onto a given layer
     * 
     * Uses breadth-first search with a queue to move through all child objects
     * 
     * @param root The root parent GameObject
     * @param layer The layer to switch the parent and all children to
     */
    public static void MoveAllToLayer(GameObject root, int layer)
    {
        // Start a queue for BFS
        Queue<Transform> allTransforms = new Queue<Transform>();
        allTransforms.Enqueue(root.transform);

        // Go until no more children
        while (allTransforms.Count > 0)
        {
            // Take first element out of queue, change the layer, add its children to the queue
            Transform dequeued = allTransforms.Dequeue();
            dequeued.gameObject.layer = layer;
            foreach (Transform child in dequeued.transform)
            {
                allTransforms.Enqueue(child);
            }
        }
    }
}
