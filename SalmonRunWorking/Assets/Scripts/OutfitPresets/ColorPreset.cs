using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SalmonRun/ColorPreset")]
public class ColorPreset : ScriptableObject
{
    [SerializeField] private Color clothColor;
    //[SerializeField] private Color hairColor;
    //[SerializeField] private Color skinColor;
    [SerializeField] private Color hatColor;

    public Color ClothColor => clothColor;

    //public Color HairColor => hairColor;

    //public Color SkinColor => skinColor;

    public Color HatColor => hatColor;

    public Color[] GetValues()
    {
        return new Color[]
        {
            clothColor, /*hairColor, skinColor,*/ hatColor
        };
    }
}
