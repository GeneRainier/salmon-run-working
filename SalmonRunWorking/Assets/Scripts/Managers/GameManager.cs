using System;
using UnityEngine;

/**
 * Script that controls the overall game flow.
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public partial class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }        //< Singleton instance of the manager

    private int m_turn;         //< What turn of the game we are currently on
    public int Turn
    {
        get { return m_turn; }
        set
        {
            m_turn = value;
            GameEvents.onTurnUpdated.Invoke();
        }
    }

    private GameState currentState;         //< Current state of the game

    private GameState prevState;            //< Name of the previous game state

    public FishSchool school;               //< Fish school in the game

    private TimeManager timeManager;        //< Time manager script

    public PauseMenu pauseMenu;            //< The pause menu in the level

    #region Major MonoBehavior Functions

    /**
     * Awake is called after the initialization of the gameObjects prior to the game start. This is used as an Initialization function
     */
    private void Awake()
    {
        // Set as singleton or delete if there's already a singleton
        if (!Instance) Instance = this;
        else
        {
            Debug.LogError("More than one game manager in the scene! Deleting second manager...");
            Destroy(gameObject);
        }
    }

    /**
     * Called before the first frame update
     */
    private void Start()
    {
        // Get refs to other components
        timeManager = ManagerIndex.MI.TimeManager;

        // Set to initial state
        Turn = 1;
        timeManager.Pause();
    }

    /**
     * Called each frame update
     */
    private void Update()
    {
        // Do any update-level operations required in the current state
        currentState?.UpdateState();

        // Check for keyboard input
        if (!Application.isEditor || !Input.GetKeyDown(KeyCode.K)) return;
        switch (currentState?.GetType().Name)
        {
            case nameof(RunState):
                // For now, this will skip to the run stats state (in case one of the fish gets stuck or something like that
                school.KillAllActive();
                break;
        }
    }

    #endregion

    #region State Management

    /**
     * Change from one state to another
     * 
     * @param newState The state we are entering into
     */
    public void SetState(GameState newState)
    {
        // If there is a previous state, exit it properly
        currentState?.ExitState();

        // Hold on to old state
        prevState = currentState;

        // Update the state
        currentState = newState;

        // Enter the new state
        currentState.Enter(prevState);

        Debug.Log($"Game State -> {currentState.GetType().Name}");
    }

    /**
     * Go back to the previous state
     */
    public void RevertState()
    {
        SetState(prevState);
    }

    /**
     * Get the current game state
     */
    public string CurrentStateName => currentState.GetType().Name;

    public bool Started => currentState != null;        //< Has the game started yet and entered its first state
    public bool Running => CompareState(typeof(RunState));  //< Checks if the game is in a certain state
    public bool PlaceState => CompareState(typeof(PlaceState)); //< Checks to see if the game is in the place state for tower placement

    /*
     * Checks whether the game is in the given state
     * 
     * @param type The state type the game is currently in
     */
    public bool CompareState(Type type)
    {
        return Started && currentState.GetType() == type;
    }

    #endregion

    #region UI Functions

    /**
     * Start the game from the intro panel
     */
    public void StartButton()
    {
        // Only does anything if we're at the very beginning
        // If so, enter the place state
        if (currentState != null) return;

        SetState(new RunStatsState());
        //SetState(new PlaceState());
    }

    /**
     * Pause the game
     */
    public void PauseButton()
    {
        // Can only pause during the run itself
        if (!CompareState(typeof(RunState))) return;

        // Pause the game
        Pause();
    }

    /**
     * React to player pressing the "Play" button
     */
    public void PlayButton()
    {
        // What the play button should do is based on what the current state is
        // but first, make sure there is a current state
        if (currentState == null) return;
        switch (CurrentStateName)
        {
            case nameof(PlaceState):
                SetState(new RunState());
                break;
            case nameof(RunState):
            default:
                NormalSpeed();
                break;
        }

    }

    /**
     * React to player pressing the "Stop" button
     */
    public void StopButton()
    {
        //timeManager.Pause();
        //currentState(EndGame.Reason.ManualQuit);
        SetState(new EndState(EndGame.Reason.ManualQuit));
    }

    /**
     * Pause the game
     */
    public void Pause()
    {
        timeManager.Pause();
    }

    /**
     * Put game at normal speed
     */
    public void NormalSpeed()
    {
        timeManager.NormalTime();
    }

    /**
     * Put game into faster speed
     */
    public void FasterSpeed()
    {
        // Only make diff speed available during the run
        if (!CompareState(typeof(RunState))) return;
        timeManager.FasterTime();
    }

    /**
     * Put game into fastest speed
     */
    public void FastestSpeed()
    {
        // Only make diff speed available during the run
        if (!CompareState(typeof(RunState))) return;
        timeManager.FastestTime();
    }

    #endregion
}
