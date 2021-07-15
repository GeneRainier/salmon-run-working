/**
 * Game state during the actual running of the salmon
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class RunState : GameState
{
    /**
     * Handle entry into the Run state
     * 
     * @param oldState The game state we are exiting from
     */
    public override void Enter(GameState oldState)
    {
        // Set to normal speed by default
        GameManager.Instance.NormalSpeed();

        // Fire event to inform that run has started
        GameEvents.onStartRun.Invoke();
    }

    /**
     * Handle exiting the Run state
     */
    public override void ExitState()
    {
        // Fire event to inform that run has ended
        GameEvents.onEndRun.Invoke();
    }

    /**
     * Handle any updating actions that need to happen mid-state
     */
    public override void UpdateState()
    {
        
    }
}
