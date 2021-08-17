using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * A script which handles basic input commands and click functions for the pause menu in a given level
 * 
 * Authors: Benjamin Person
 */
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel = null;      //< Reference to the panel that displays all the Pause Menu UI elements

    [SerializeField] private GameManager theGameManager = null;     //< Reference to the GameManager in the scene

    [SerializeField] private TowerManager theTowerManager = null;     //< Reference to the TowerManager in the scene

    [SerializeField] private FishSchool school = null;          //< The fish school present in the level

    [SerializeField] private DragAndDropIcon damIcon = null;    //< The UI Icon representing the dam tower option
    [SerializeField] private DragAndDropIcon ladderIcon = null;    //< The UI Icon representing the ladder tower option

    [SerializeField] private DamPlacementLocation placementLocation = null;       //< The placement location for the dam and ladder to manipulate placement visualizations

    public Slider turnSlider = null;      //< Reference to the slider to select a turn to revert back to
    [SerializeField] private Text sliderText = null;        //< The text next to the slider showing what value the slider is set to

    /**
      * Called before the first frame update
      */
    private void Start()
    {
        // Get references to the existing managers
        theGameManager = FindObjectOfType<GameManager>();
        theTowerManager = FindObjectOfType<TowerManager>();
        school = FindObjectOfType<FishSchool>();
    }

    /**
     * Called each frame update
     */
    void Update()
    {
        // If the player hits escape, we want to toggle the pause menu on or off
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(!pausePanel.activeSelf);
        }
    }

    /*
     * Restart the current level when the corresponding pause menu button is pressed
     */
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /*
     * Adjust the Text of the Pause Menu slider when choosing what turn to revert the level to
     */
    public void AdjustSliderText()
    {
        sliderText.text = "Turn:\n" + turnSlider.value;
    }

    /*
     * Revert the level to the turn set in the turn slider
     */
    public void RevertTurn()
    {
        // For every tower, if the tower was placed after the selected turn, get rid of it
        foreach (TowerBase tower in theTowerManager.GetTowers())
        {
            if (tower.turnPlaced > turnSlider.value)
            {
                Destroy(tower.transform.root.gameObject);
            }
        }

        // Remove any active fish in the scene
        school.DestroyAllActive();

        // Remove the dam and ladder if they were not placed on the given turn
        Dam theDam = FindObjectOfType<Dam>();
        DamLadder theLadder = FindObjectOfType<DamLadder>();
        if (theDam != null)
        {
            if (theDam.turnPlaced > turnSlider.value)
            {
                damIcon.hasBeenPurchased = false;
                PlacementVisualizationManager.Instance.DisplayVisualization(theDam.GetType(), true);
                placementLocation.RemoveDam();
                Destroy(theDam.transform.gameObject);
            }
        }

        if (theLadder != null)
        {
            if (theLadder.turnPlaced > turnSlider.value)
            {
                ladderIcon.hasBeenPurchased = false;
                PlacementVisualizationManager.Instance.DisplayVisualization(theLadder.GetType(), true);
                placementLocation.RemoveLadder();
                Destroy(theLadder.transform.gameObject);
            }
        }

        // Set the corresponding generation of salmon for the selected turn


        // Set the turn counter to indicate the proper turn
        theGameManager.Turn = (int) turnSlider.value;

        // Set the GameState to the place state and reset the stats
        theGameManager.SetState(new RunStatsState());
    }
}
