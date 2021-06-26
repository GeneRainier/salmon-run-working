using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class that handles switching between UI panels
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class PanelSwitching : MonoBehaviour
{
    public RectTransform theRectTransform;      //< The RectTransform of a panel to acquire its dimension and position properties

    /*
     * Brings a certain menu tab forward on button press
     */
    public void OnButtonPress ()
    {
        Debug.Log("buttonPressed");

        //theRectTransform = transform as RectTransform; // Cast it to RectTransform
        theRectTransform.SetAsLastSibling(); // Make the panel show on top.
    }
}
