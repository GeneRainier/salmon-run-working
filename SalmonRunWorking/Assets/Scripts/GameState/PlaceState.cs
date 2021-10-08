using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Game state during the phase designated for users to place towers
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class PlaceState : GameState
{
    /**
     * Handle entry into the Place state
     * 
     * @param oldState The game state we are exiting
     */
    public override void Enter(GameState oldState)
    {
        // Pause the game
        GameManager.Instance.PauseButton();
        //GameManager.Instance.
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
