using System.Collections.Generic;
using UnityEngine;

/**
 * Manages pace the game runs at
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class TimeManager : MonoBehaviour
{
    // States representing each possible time rate the game can run at
    public enum TimeState
    {
        Paused,
        NormalSpeed,
        FasterSpeed,
        FastestSpeed
    }

    private TimeState timeState;            //< Time rate the game is currently running at

    private List<IPausable> pausableObjects;    //< List of pausable objects in the game

    [Range(0f, 10f)]
    public float fasterTimeScale;           //< Slightly sped up time scale

    [Range(0f, 10f)]
    public float fastestTimeScale;          //< Majorly sped up time scale

    #region Major Monobehaviour Functions

    /**
     * Awake is called after the initialization of every gameObject prior to the game Starting. Used as an Initialization function
     */
    private void Awake()
    {
        // Initialize list of pausables
        pausableObjects = new List<IPausable>();
    }

    #endregion

    #region Pausable Objects

    /**
     * Register a pausable object with the manager
     * 
     * @param pausable The object being paused
     */
    public void RegisterPausable(IPausable pausable)
    {
        if (timeState == TimeState.Paused)
        {
            pausable.Pause();
        }
        else
        {
            pausable.Resume();
        }

        pausableObjects.Add(pausable);
    }

    #endregion

    #region Time Functions

    /**
     * Pause the game
     */
    public void Pause()
    {
        timeState = TimeState.Paused;

        Time.timeScale = 0;

        foreach(IPausable p in pausableObjects)
        {
            p.Pause();
        }
    }

    /**
     * Make the game run at normal time
     */
    public void NormalTime()
    {
        Resume();

        timeState = TimeState.NormalSpeed;

        Time.timeScale = 1;
    }

    /**
     * Make the game run at a slightly faster speed
     */
    public void FasterTime()
    {
        Resume();

        timeState = TimeState.FasterSpeed;

        Time.timeScale = fasterTimeScale;
    }

    /**
     * Make the game run at a very fast speed
     */
    public void FastestTime()
    {
        Resume();

        timeState = TimeState.FastestSpeed;

        Time.timeScale = fastestTimeScale;
    }

    /**
     * Unpause all paused objects if necessary
     */
    private void Resume()
    {
        if (timeState != TimeState.Paused) return;
        foreach (IPausable p in pausableObjects)
        {
            p.Resume();
        }
    }

    /*
     * Checks if the current state matches with a particular checked state
     * 
     * @param timeState The speed of the game we are comparing against
     */
    public bool IsState(TimeState timeState)
    {
        return this.timeState == timeState;
    }

    #endregion
}
