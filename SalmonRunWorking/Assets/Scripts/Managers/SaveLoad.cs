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

    /*
     * Creates a Save object and stores the object in the list of saves
     */
    public void SaveGame()
    {

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
    public class Save
    {

    }
}
