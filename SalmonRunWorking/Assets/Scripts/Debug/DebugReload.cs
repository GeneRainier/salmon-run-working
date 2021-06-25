using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * A small script for quickly being able to test the current level scene again.
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class DebugReload : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // If you want to reload the level without restarting the whole Unity Game Scene (hitting the play button again), hit R
        if (Application.isEditor && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
