using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Script that handles main menu buttons that load game scenes
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class MainMenuButtons : MonoBehaviour
{
    /*
     * Loads the game mode version of the level
     */
    public void LoadSceneCreative()
    {
        SceneManager.LoadScene("CreativeModeDemoLevel");
    }

    /*
     * Loads the simulation mode version of the level
     */
    public void LoadSceneLearning()
    {
        SceneManager.LoadScene("LearningModeDemoLevel");
    }

    /*
     * Exits the application
     */
    public void QuitGame()
    {
        Application.Quit();
    }
}
