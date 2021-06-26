using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Interface for objects that can be paused and resumed by the game
 * 
 * This is used rather than a simple use of Time.timeScale because some objects (e.g. the camera motion) should never be paused, while others (like fish & towers) should be
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public interface IPausable
{
    void Pause();

    void Resume();
}
