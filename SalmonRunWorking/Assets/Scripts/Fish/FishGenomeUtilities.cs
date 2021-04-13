﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Provides functionality for creating a new generation of fish from the old generation
 */
public class FishGenomeUtilities : MonoBehaviour
{
    // End of Round Parent Size Counters
    public static int smallParent = 0;
    public static int mediumParent = 0;
    public static int largeParent = 0;

    /**
     * Create a new, random generation of fish for the initial group of salmon
     * 
     * @param generationSize int The number of fish that should be in this generation
     * @param lockSexRatio bool True if we want to lock to half males, half females (or as close as is possible), false if we want random
     * @param lockSizeRatio bool True if we want to lock to set ratios of sizes, false if we want random
     */
    public static List<FishGenome> MakeNewGeneration(int generationSize, bool lockSexRatio, bool lockSizeRatio)
    {
        // create a list to hold the new genomes
        List<FishGenome> newGeneration = new List<FishGenome>();

        for (int i = 0; i < generationSize; i++)
        {
            FishGenePair[] genes = new FishGenePair[FishGenome.Length];

            // figure out the sex gene pair
            FishGenePair sexPair;

            // the mom gene will always be an x
            sexPair.momGene = FishGenome.X;

            // how we determine the dad gene depends on whether we want equal number of males and females or if we want it to be random
            if (lockSexRatio)
            {
                // want to lock sex ratio, so odd fish will be one sex and even fish will be the other
                sexPair.dadGene = i % 2 == 0 ? FishGenome.X : FishGenome.Y;
            }
            else
            {
                // dad gene determined by pseudorandom chance
                sexPair.dadGene = Random.Range(0, 2) == 0 ? FishGenome.X : FishGenome.Y;
            }

            // add the sex gene to our list of genomes
            genes[(int)FishGenome.GeneType.Sex] = sexPair;

            // figure out the size gene pair
            FishGenePair sizePair;

            // how we determine the size genes is dependant on whether we want to lock to specific ratios or do random
            // if we're locking values, we use i % 4 to get 25% big, 50% medium, 25% small
            // if we aren't just do a random value
            int value = lockSizeRatio ? i % 4 : Random.Range(0, 4);
            switch (value)
            {
                case 0:
                default:
                    sizePair.momGene = FishGenome.B;
                    sizePair.dadGene = FishGenome.B;
                    break;
                case 1:
                    sizePair.momGene = FishGenome.B;
                    sizePair.dadGene = FishGenome.b;
                    break;
                case 2:
                    sizePair.momGene = FishGenome.b;
                    sizePair.dadGene = FishGenome.B;
                    break;
                case 3:
                    sizePair.momGene = FishGenome.b;
                    sizePair.dadGene = FishGenome.b;
                    break;
            }

            // add the size gene to our list of genomes
            genes[(int)FishGenome.GeneType.Size] = sizePair;

            // create a genome out of our genes and add it to the list
            FishGenome genome = new FishGenome(genes);
            newGeneration.Add(genome);
        }

        return newGeneration;
    }

    /**
     * Given a list of potential parent fish, create a new generation of fish
     * 
     * @param potentialParents Fish The list of potential parents
     * @param minOffspring The minimum amount of offspring generated by any given pairing
     * @param maxOffspring The maximum amount of offspring generate by any given pairing
     */
    public static List<FishGenome> MakeNewGeneration(List<FishGenome> potentialParents, int minOffspring, int maxOffspring)
    {
        // Reset parent counts for this generation
        smallParent = 0;
        mediumParent = 0;
        largeParent = 0;
        
        // create a list to put the new generation in
        List<FishGenome> newGeneration = new List<FishGenome>();

        // find all the females
        List<FishGenome> females = FindFemaleGenomes(potentialParents);
        // find all the males
        List<FishGenome> males = FindMaleGenomes(potentialParents);

        // determine which list is shorter
        int shortestLength = Mathf.Min(females.Count, males.Count);

        // Referencable gene pairs for the male and female parent fish for the sake of counting at the end of a round
        List<FishGenome> smallMalePairs = FindSmallGenomes(males);
        List<FishGenome> mediumMalePairs = FindMediumGenomes(males);
        List<FishGenome> largeMalePairs = FindLargeGenomes(males);
        List<FishGenome> smallFemalePairs = FindSmallGenomes(females);
        List<FishGenome> mediumFemalePairs = FindMediumGenomes(females);
        List<FishGenome> largeFemalePairs = FindLargeGenomes(females);

        // loop (shortest list of males and females) times
        // each time, generate a certain number of offspring from the ith male and ith female
        Debug.Log("Before Repro Loop: minOffspring=" + minOffspring + ";  maxOffspring=" + maxOffspring);
        for (int i = 0; i < shortestLength; i++)
        {
            // determine how many offspring this pairing will make
            int numOffspring = Random.Range(minOffspring, maxOffspring + 1);
            for (int offspring = 0; offspring < numOffspring; offspring++)
            {
                // add each fish to the new generation
                newGeneration.Add(new FishGenome(females[i], males[i]));
            }

            // Count up the parents by size for the post run panel
            if (smallMalePairs.Contains(males[i]))
            {
                smallParent++;
            }
            else if (mediumMalePairs.Contains(males[i]))
            {
                mediumParent++;
            }
            else
            {
                largeParent++;
            }

            if (smallFemalePairs.Contains(females[i]))
            {
                smallParent++;
            }
            else if (mediumFemalePairs.Contains(females[i]))
            {
                mediumParent++;
            }
            else
            {
                largeParent++;
            }
        }

        return newGeneration;
    }

    /**
     * Get a list of all male fish genomes from a list of fish genomes
     * 
     * @param genomeList List<FishGenome> The list of fish to be looked through
     * 
     * @return List<FishGenome> A list of all male fish within the original list
     */
    public static List<FishGenome> FindMaleGenomes(List<FishGenome> genomeList)
    {
        return genomeList.Where(fish => fish.IsMale()).ToList();
    }

    /**
     * Get a list of all female fish genomes from a list of fish genomes
     * 
     * @param genomeList List<FishGenome> The list of fish to be looked through
     * 
     * @return List<FishGenome> A list of all female fish within the original list
     */
    public static List<FishGenome> FindFemaleGenomes(List<FishGenome> genomeList)
    {
        return genomeList.Where(fish => !fish.IsMale()).ToList();
    }

    /**
     * Get a list of all small fish genomes from a list of fish genomes
     * 
     * @param genomeList List<FishGenome> The list of fish to be looked through
     * 
     * @return List<FishGenome> A list of all small fish within the original list
     */
    public static List<FishGenome> FindSmallGenomes(List<FishGenome> genomeList)
    {
        return genomeList.Where((fish) => {
            FishGenePair sizeGenePair = fish[FishGenome.GeneType.Size];

            return sizeGenePair.momGene == FishGenome.b && sizeGenePair.dadGene == FishGenome.b;
        }).ToList();
    }


    /**
     * Get a list of all medium fish genomes from a list of fish genomes
     * 
     * @param genomeList List<FishGenome> The list of fish to be looked through
     * 
     * @return List<FishGenome> A list of all medium fish within the original list
     */
    public static List<FishGenome> FindMediumGenomes(List<FishGenome> genomeList)
    {
        return genomeList.Where((fish) => {
            FishGenePair sizeGenePair = fish[FishGenome.GeneType.Size];

            return sizeGenePair.momGene != sizeGenePair.dadGene;
        }).ToList();
    }

    /**
     * Get a list of all large fish genomes from a list of fish genomes
     * 
     * @param genomeList List<FishGenome> The list of fish to be looked through
     * 
     * @return List<FishGenome> A list of all large fish within the original list
     */
    public static List<FishGenome> FindLargeGenomes(List<FishGenome> genomeList)
    {
        return genomeList.Where((fish) => {
            FishGenePair sizeGenePair = fish[FishGenome.GeneType.Size];

            return sizeGenePair.momGene == FishGenome.B && sizeGenePair.dadGene == FishGenome.B;
        }).ToList();
    }
}
