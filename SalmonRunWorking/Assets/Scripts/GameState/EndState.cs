using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Game state during the phase designated for ending of the game
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class EndState : GameState
{
    private EndGame.Reason reason;          //< Reason why we are quitting

    /**
     * Constructor
     * 
     * @param reason The reason for ending the game
     */
    public EndState(EndGame.Reason reason)
    {
        this.reason = reason;
    }

    /**
     * Handle entry into the Place state
     * 
     * @param oldState The game state we are exiting from
     */
    public override void Enter(GameState oldState)
    {
        // Pause the game
        GameManager.Instance.PauseButton();

        // Fire game end event
        GameEvents.onEndGame.Invoke(reason);
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
