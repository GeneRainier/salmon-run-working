/**
 * Abstract base class for a state that the game can be in
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public abstract class GameState
{
    /**
     * Action take on state entry
     * 
     * @param oldState The game state we are exiting from
     */
    public abstract void Enter(GameState oldState);

    /**
     * Action taken repeatedly during a state
     */
    public abstract void UpdateState();

    /**
     * Action taken on state exit
     */
    public abstract void ExitState();
}
