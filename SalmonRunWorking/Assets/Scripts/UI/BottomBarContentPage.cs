using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class for the bottom UI bar
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class BottomBarContentPage : MonoBehaviour
{
    public string pageTitle;        //< Title of the page

    private bool active;        //< True if this content page is currently active
    
    /*
     * Getters and Setters for the Bottom UI bar activation
     */
    public bool Active
    {
        get
        {
            return active;
        }
        set
        {
            active = value;

            // Set all children to the setter value
            SetActiveUtils.SetChildrenActiveRecursively(gameObject, active);
        }
    }
}
