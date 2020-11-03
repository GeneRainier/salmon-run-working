using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    [SerializeField] private List<ColorPreset> usablePresets;

    // TODO: Select at least one from each, deplete initial list before drawing random, or choose statistically
    
    public ColorPreset GetRandomPreset()
    {
        return usablePresets[Random.Range(0, usablePresets.Count - 1)];
    }
}
