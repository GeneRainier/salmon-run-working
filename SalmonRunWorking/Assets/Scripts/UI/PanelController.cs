using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Abstract base class for a panel in the game
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public abstract class PanelController : MonoBehaviour
{
    /**
     * Activate the panel
     */
    public void Activate()
    {
        gameObject.SetActive(true);
    }

    /**
     * Deactivate the panel
     */
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
