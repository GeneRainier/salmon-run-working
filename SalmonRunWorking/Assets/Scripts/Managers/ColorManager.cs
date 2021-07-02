using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Handles setting up random colors for the fish to select from
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class ColorManager : MonoBehaviour
{
    [SerializeField] private List<ColorPreset> usablePresets = null;       //< List of presets for the fish to utilize

    // TODO: Select at least one from each, deplete initial list before drawing random, or choose statistically
    
    /*
     * Selects a random color preset from the list of options
     * 
     * @return ColorPreset The color preset selected for the fish
     */
    public ColorPreset GetRandomPreset()
    {
        return usablePresets[Random.Range(0, usablePresets.Count - 1)];
    }
}
