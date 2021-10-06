using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script controls autosaves between each turn of the game and provides the capability to save the game state at the start 
 * of each turn and to load a prior turn.
 * 
 * Author: Benjamin Person (2021)
 */
public class SaveLoad : MonoBehaviour
{
    private List<Save> saves;           //< List of the game states of each turn that has happened so far (index 0 is turn 1)
    private GameManager gameManager;    //< Reference to the gameManager which speaks with the TowerManager

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    /*
     * Creates a Save object and stores the object in the list of saves
     */
    public void SaveGame()
    {
        Save currentTurn = new Save();

        // Loop through every tower and save that data in the currentTurn's save
        foreach (TowerBase tower in gameManager.GetTowerList())
        {
            // Determine the type of Tower we are saving and save its data accordingly to our Save structure
            if (tower is AnglerTower)
            {

            }
            else if (tower is RangerTower)
            {

            }
            else if (tower is SealionTower)
            {

            }
            else if (tower is Dam)
            {

            }
            else if (tower is DamLadder)
            {

            }
        }

        // Save the current generation of salmon


        //Push the currentTurn's data to the list of saves
        saves.Add(currentTurn);
    }

    /*
     * Loads the game state of the turn in the list of saves to revert the game to
     * 
     * @param turn The turn number from the Pause Menu UI slider that we want to revert the game to
     */
    public void LoadGame(int turn)
    {

    }

    /*
     * Class that describes the contents of a single turn's game state at the moment of saving
     */
    [System.Serializable]
    public class Save
    {
        public int turn;        //< The turn number associated with this saved game

        /* Salmon Generation Data */
        // Currently this is being accomplished by way of getting each count of each size and gender combo, and loading in a new genome list at time of loading
        public int smallMale = 0;       //< The number of small male fish from this turn
        public int mediumMale = 0;      //< The number of medium male fish from this turn
        public int largeMale = 0;       //< The number of large male fish from this turn
        public int smallFemale = 0;     //< The number of small female fish from this turn
        public int mediumFemale = 0;    //< The number of medium female fish from this turn
        public int largeFemale = 0;     //< The number of large female fish from this turn

        /* General Tower Info */
        public List<float[]> towerPositions = new List<float[]>();      //< The serialized list of where every tower is
        public List<float[]> towerRotations = new List<float[]>();      //< The serialized list of the rotations of every tower
        public List<int> towerTypes = new List<int>();                  //< The serialized list of what type of tower each tower is

        /* Angler Data */
        public List<int> anglerPlaced = new List<int>();                                    //< The turn the angler was placed into the level
        public List<int> caughtFish = new List<int>();                                      //< The number of fish this angler has currently caught
        public List<float[]> anglerCatchRates = new List<float[]>();    //< The serialized list of each anglers' size catch rates
        public List<float> anglerCatchReset = new List<float>();        //< The serialized list of each anglers' catch reset time

        /* Ranger Data */
        public List<int> rangerPlaced = new List<int>();                                       //< The turn the ranger was placed into the level
        public List<float[]> rangerRegulateRates = new List<float[]>();    //< The serialized list of each rangers' regulation rates for each size fish
        public List<float> rangerRegulateReset = new List<float>();        //< The serialized list of each rangers' regulation reset time

        /* Dam Data */
        public int damPlaced = 0;                                          //< The turn the dam was placed into the level
        // The Dams position and type is already tracked in the General Tower Lists

        /* Ladder Data */
        public int ladderType = 0;        //< The type of Salmon Ladder currently placed at the dam

        /* Sealion Data */
        public List<int> sealionAppeared = new List<int>();                                  //< The turn the sealion was placed into the level
        public List<float[]> sealionCatchRates = new List<float[]>();    //< The serialized list of each sealions' size catch rates
        public List<float> sealionCatchReset = new List<float>();        //< The serialized list of each sealions' catch reset time
    }
}
