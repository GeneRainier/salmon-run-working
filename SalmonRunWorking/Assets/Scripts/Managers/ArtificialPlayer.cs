using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A class that is attached to the GameManager to allow for data gathering and playtesting without need for 
 * a developer to sit and play through a lengthy game.
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class ArtificialPlayer : MonoBehaviour
{
    public bool runArtificialPlayer;        //< Do we want to run a test with the artificial player or not?

    // PostRunStats script for quickly flipping through end of round functionality
    [SerializeField] private PostRunStatsPanelController statsScript;

    // Initial game start panels (Start Menu and Instructions in the UI Canvas)
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject instructionPanel;

    public GameObject anglerPrefab;       //< The prefab for an angler tower
    public GameObject rangerPrefab;       //< The prefab for the ranger tower
    [SerializeField] private GameObject damPrefab;          //< The prefab for the dam
    [SerializeField] private GameObject ladderPrefab;       //< The prefab for the salmon ladder

    [SerializeField] private List<Vector3> anglerPositions;     //< The positions the anglers may be placed by the artifical player
    [SerializeField] private List<Vector3> rangerPositions;     //< The positions the rangers may be placed by the artifical player
    [SerializeField] private Vector3 damLadderPosition;         //< The position the dam and ladder may be placed by the artifical player

    // Start is called before the first frame update
    void Start()
    {
        if (runArtificialPlayer == true)
        {
            // Start the initial set of commands to enter the round 1 place state
            BeginTest();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (runArtificialPlayer == true)
        {
            // If the round is running, all we want to do is set the speed to max and wait for the round to change
            if (GameManager.Instance.CompareState(typeof(RunState)))
            {
                GameManager.Instance.FastestSpeed();
            }
            // If we are in the place state, we want to place our next batch of towers
            else if (GameManager.Instance.CompareState(typeof(PlaceState)))
            {
                if (GameManager.Instance.Turn == 2)
                {
                    GameObject angler = Instantiate(anglerPrefab, anglerPositions[0], Quaternion.identity) as GameObject;
                    AnglerTower anglerScript = angler.GetComponentInChildren<AnglerTower>();
                    anglerScript.TowerActive = true;
                    anglerScript.Resume();
                    GameObject ranger = Instantiate(rangerPrefab, rangerPositions[0], Quaternion.identity) as GameObject;
                    RangerTower rangerScript = ranger.GetComponentInChildren<RangerTower>();
                    rangerScript.TowerActive = true;
                    rangerScript.Resume();
                }
                GameManager.Instance.PlayButton();
            }
            // If the round has ended, we want to click through the UI screens via our coroutine
            else if (GameManager.Instance.CompareState(typeof(RunStatsState)))
            {
                EndOfRound();
            }
            // If the population dies out (game over), click the restart button and start running the test again
            // TO DO: Ensure a new output file is created and appended to after a restart is declared
            // TO DO: Determine how to create lineup of tests to run? Or run a number of the same test and then the tester returns to input a new one?
            else if (GameManager.Instance.CompareState(typeof(EndState)))
            {

            }
        }
    }

    /*
     * Starts the game by clicking past the first set of UI screens
     */
    private void BeginTest()
    {
        // Deactivate the first few UI panels
        startPanel.SetActive(false);
        instructionPanel.SetActive(false);
        // Trigger the Start action of the game that runs in the instruction panel
        GameManager.Instance.StartButton();
        // Exit the first stats panel and enter the placement state
        statsScript.OnNextRunButton();
    }

    /*
     * Continues past a finished round by clicking through the UI screens
     */
    private void EndOfRound()
    {
        // Exit the stats panel and enter the placement state
        statsScript.OnNextRunButton();
    }

    /*
     * Restarts the game after a loss and begins the next test
     */
    private void EndOfGame()
    {
        
    }
}
