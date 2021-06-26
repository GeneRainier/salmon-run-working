using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * Class that defines the various states the game can be in
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class GameEvents : MonoBehaviour
{
    public static UnityEvent onStartRun = new UnityEvent();     //< Event for starting a "run" state

    public static UnityEvent onEndRun = new UnityEvent();       //< Event for ending a "run" state

    public static UnityEvent onStartRunStats = new UnityEvent();    //< Event for beginning the "run stats" state

    // Event for ending the game
    [System.Serializable]
    public class EndGameEvent : UnityEvent<EndGame.Reason> { };
    public static EndGameEvent onEndGame = new EndGameEvent();

    public static UnityEvent onTurnUpdated = new UnityEvent();      //< Called when turn gets changed

    // Called when a new generation of fish has been created
    [System.Serializable]
    public class NewGenerationEvent : UnityEvent<List<FishGenome>, List<FishGenome>> { };
    public static NewGenerationEvent onNewGeneration = new NewGenerationEvent();

    // Called when a change in fish population has occurred
    [System.Serializable]
    public class PopulationEvent : UnityEvent<List<FishGenome>, List<FishGenome>, List<FishGenome>> { };
    public static PopulationEvent onFishPopulationChanged = new PopulationEvent();
}
