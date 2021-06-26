using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// NOTE: So far this method does nothing but declare stuff. Someday it will write a data file.
 
/*
 * Method to collect data about the version and data at the end of each turn. 
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class SalmonRunStats : Object
{
    /*
     * Struct that denotes all of the values to track for a data file
     */
    public struct endOfTurnStatisticsStructure
    {
        List<int> parentSmall;
        List<int> parentMedium;
        List<int> parentLarge;
        List<int> offspringSmall;
        List<int> offspringMedium;
        List<int> offspringLarge;
        List<int> offspringFemale;
        List<int> offspringMale;
        List<int> barSurvive;
        List<int> barFemale;
        List<int> barMale;
        List<int> barSmall;
        List<int> barMedium;
        List<int> barLarge;
        List<int> numAnglerPlain;
        List<int> numAnglerRanger;
        List<int> numDam;
        List<int> numLadder;
    }

    /*
     * Struct for the parameters set in the ManagerIndex to note in the data file
     */
    public struct versionParameterStructure
    {
        string Version;           //DemoLevel>Canvas_Updated>StartPanel>Text2
        // Project: Assets>Prefabs>Towers>FishermanTower; Hierachy: FishermanTower>TokenBase (angler instead of fisherman)
        int anglerEffectRadius;
        int anglerTimePerApplyEffect;
        int anglerDefaultCatchRate;
        int anglerNumFlashesPerCatch;
        float anglerDefaultSmallCatchRate;
        float anglerDefaultMediumCatchRate;
        float anglerDefaultLargeCatchRate;
        // Project: Assets>Prefabs>Towers>RangerTower; Hierachy: RangerTower>TokenBase 
        int rangerEffectRadius;
        int rangerTimePerApplyEffect;
        float rangerSlowdownEffectSmall;
        float rangerSlowdownEffectMedium;
        float rangerSlowdownEffectLarge;
        int rangerRegulationSuccessRate;
        int rangerNumFlashesPerCatch;
        float damDefaultCrossingRate;    //Project; Assets>Prefabs>Filters>Dam
        //Project; Assets>Prefabs>Filters>DamLadder
        float ladderSmallDefaultCrossingRate;
        float ladderMediumDefaultCrossingRate;
        float ladderLargeDefaultCrossingRate;
        int schoolNumNestSights;    //not really in school look in EndOfLevel which is in three different spots
        // Project: Assets>_Scenes>MapV2>DemoLevel; Hierarchy: DemoLevel>School
        int schoolInitialNumFish;
        int schoolMinOffspring;
        int schoolMaxOffspring;
        float schoolTimeBetweenWaves;
        int schoolFishPerWave;
    }

    public versionParameterStructure versionData;           //< Declared versionParameterStructure object
    public endOfTurnStatisticsStructure endOfTurnStats;     //< Declared endOfTurnStats object
}

/**
 * Controls panel that pops up when ending the game
 */
public class EndGamePanelController : PanelController
{
    public TextMeshProUGUI titleText;       //< Panel title text

    public TextMeshProUGUI turnText;        //< Panel turn text

    public TextMeshProUGUI messageText;     //< Panel message text

    public GameObject confirmButton;        //< Affirmative button

    public GameObject cancelButton;         //< Cancel button

    public TextMeshProUGUI confirmButtonText;   //< Affirmative button text

    public TextMeshProUGUI cancelButtonText;    //< Cancel button text

    /**
     * Start is called before the first frame update
     */
    private void Start()
    {
        // Subscribe to events
        GameEvents.onEndGame.AddListener(OnEndOfGameEvent);

        // Deactivated on start
        Deactivate();
    }

    /**
     * Handle end of game event
     * 
     * @param reason The reason the game is ending (extinction, quitting, etc.)
     */
    private void OnEndOfGameEvent(EndGame.Reason reason)
    {
        UpdatePanel(reason);
        Activate();
    }

    /**
     * Update panel text/data based on what the reason for ending the game is
     * 
     * @param reason The reason the game is ending (extinction, quitting, etc.)
     */
    private void UpdatePanel(EndGame.Reason reason)
    {
        // Do all common updates
        UpdatePanelCommon();

        // Do anything specific to the reason for ending the game
        switch(reason)
        {
            case EndGame.Reason.ManualQuit:
                titleText.text = "Quit?";
                messageText.text = "Are you sure?";
                confirmButton.SetActive(true);
                confirmButtonText.text = "Yes";
                cancelButton.SetActive(true);
                cancelButtonText.text = "Go Back";
                break;
            case EndGame.Reason.NoOffspring:
                titleText.text = "Game Over";
                messageText.text = "All fish have died! Press button to restart. Thanks for playing!";
                confirmButton.SetActive(true);
                confirmButtonText.text = "Restart";
                cancelButton.SetActive(false);
                break;
        }
    }

    /**
     * Update any panel text/data that works commonly across all reasons for ending the game
     */
    private void UpdatePanelCommon()
    {
        turnText.text = "Turns Lasted: " + (GameManager.Instance.Turn - 1).ToString();
    }

    /**
     * Handle pressing of end game button
     */
    public void OnConfirmButton()
    {
        EndGame.Quit();
    }

    /**
     * Handle pressing of cancel button
     */
    public void OnCancelButton()
    {
        Deactivate();
        GameManager.Instance.RevertState();
    }
}
