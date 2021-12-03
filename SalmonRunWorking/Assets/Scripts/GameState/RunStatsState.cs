using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Game state where statistics are displayed (presumably following the Run state)
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class RunStatsState : GameState
{
    /**
     * Handle entry into the RunStats state
     * 
     * @param oldState The game state we are exiting from
     */
    public override void Enter(GameState oldState)
    {
        // invoke event
        GameEvents.onStartRunStats.Invoke();
    }

    /**
     * Handle exiting the RunStats state
     */
    public override void ExitState()
    {
        // Increment turn
        GameManager.Instance.Turn++;
        // Increment max Pause Menu slider option
        GameManager.Instance.pauseMenu.turnSlider.maxValue++;
        GameManager.Instance.pauseMenu.turnSlider.minValue = 1;
        GameManager.Instance.pauseMenu.turnSlider.value = GameManager.Instance.pauseMenu.turnSlider.maxValue;
    }

    /**
     * Handle any updating actions that need to happen mid-state
     */
    public override void UpdateState()
    {

    }
}
