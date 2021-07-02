using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Scriptable Object that contains all of the potential color presets a tower can select from
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
[CreateAssetMenu(menuName = "SalmonRun/ColorPreset")]
public class ColorPreset : ScriptableObject
{
    [SerializeField] private Color clothColor = Color.black;      //< The clothing color a tower can choose
    //[SerializeField] private Color hairColor;
    //[SerializeField] private Color skinColor;
    [SerializeField] private Color hatColor = Color.white;        //< The hat color a tower can choose

    public Color ClothColor => clothColor;

    //public Color HairColor => hairColor;

    //public Color SkinColor => skinColor;

    public Color HatColor => hatColor;

    /*
     * Acquires the colors for each of the texture elements the tower has chosen
     * 
     * @return Color[] A list of all of the necessary colors
     */
    public Color[] GetValues()
    {
        return new Color[]
        {
            clothColor, /*hairColor, skinColor,*/ hatColor
        };
    }
}
