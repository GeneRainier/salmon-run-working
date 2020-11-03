using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    public void LoadSceneCreative()
    {
        SceneManager.LoadScene("CreativeModeDemoLevel");
    }

    public void LoadSceneLearning()
    {
        SceneManager.LoadScene("LearningModeDemoLevel");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
