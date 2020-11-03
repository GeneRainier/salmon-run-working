﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Game state during the phase designated for users to place towers
 */
public class PlaceState : GameState
{
    /**
     * Handle entry into the Place state
     */
    public override void Enter(GameState oldState)
    {
        // pause the game
        GameManager.Instance.PauseButton();
    }

    /**
     * Handle exiting the Place state
     */
    public override void ExitState()
    {
        
    }

    /**
     * Handle any updating actions that need to happen mid-state
     */
    public override void UpdateState()
    {

    }
}
