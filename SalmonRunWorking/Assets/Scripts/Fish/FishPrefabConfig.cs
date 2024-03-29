﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * ScriptableObject containing references to all fish prefabs that can be used
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
 [CreateAssetMenu(fileName = "FishPrefabConfig", menuName = "Fish/Fish Prefab Config")]
public class FishPrefabConfig : ScriptableObject
{
    // Female fish prefabs of various sizes
    public GameObject smallFemale;
    public GameObject mediumFemale;
    public GameObject largeFemale;

    // Male fish prefabs of various sizes
    public GameObject smallMale;
    public GameObject mediumMale;
    public GameObject largeMale;

    /**
     * Determine which fish prefab should be used given a fish's genome
     * 
     * @param genome FishGenome The genome that determines which prefab we should use
     */
    public GameObject GetFishPrefab(FishGenome genome)
    {
        // Gameobject we will return at end
        GameObject toReturn;

        // Get the size gene for the fish
        FishGenePair sizeGenePair = genome[FishGenome.GeneType.Size];

        // Different prefabs for each sex
        if (genome.IsMale())
        {
            // Different prefabs for each male size
            if (sizeGenePair.momGene == FishGenome.b && sizeGenePair.dadGene == FishGenome.b)
            {
                toReturn = smallMale;
            }
            else if (sizeGenePair.momGene == FishGenome.B && sizeGenePair.dadGene == FishGenome.B)
            {
                toReturn = largeMale;
            }
            else
            {
                toReturn = mediumMale;
            }
        }
        else
        {
            // Different prefabs for each female size
            if (sizeGenePair.momGene == FishGenome.b && sizeGenePair.dadGene == FishGenome.b)
            {
                toReturn = smallFemale;
            }
            else if (sizeGenePair.momGene == FishGenome.B && sizeGenePair.dadGene == FishGenome.B)
            {
                toReturn = largeFemale;
            }
            else
            {
                toReturn = mediumFemale;
            }
        }

        return toReturn;
    }
}
