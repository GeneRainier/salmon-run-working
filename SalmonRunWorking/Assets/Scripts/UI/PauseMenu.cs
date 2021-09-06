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
        // Call GameManager CreateSave function

    }
}
