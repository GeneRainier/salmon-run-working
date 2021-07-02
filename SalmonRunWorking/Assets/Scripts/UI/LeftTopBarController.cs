using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * Script that controls the upper left UI bar which includes money and turn timers
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class LeftTopBarController : MonoBehaviour
{
    public TextMeshProUGUI turnCounterText;         //< Turn counter text

    public TextMeshProUGUI coinText;                //< Coin text

    public SealionSpawner triggerSpawn;             //< To trigger the sealion spawning

    [SerializeField] private TextMeshProUGUI timerText = null;     //< Text that lists the current time in the round
    private int timer;                              //< The timer value of the current round

    /**
     * Called when a GameObject is enabled. This is used as an Initialization function
     */
    private void OnEnable()
    {
        GameEvents.onTurnUpdated.AddListener(UpdateTopBarUI);
        GameEvents.onEndRun.AddListener(StopTimer);
        GameEvents.onStartRun.AddListener(StartTimer);
    }

    /*
     * Called when a GameObject is disabled
     */
    private void OnDisable()
    {
        GameEvents.onTurnUpdated.RemoveListener(UpdateTopBarUI);
        GameEvents.onEndRun.RemoveListener(StopTimer);
        GameEvents.onStartRun.RemoveListener(StartTimer);
    }

    /**
     * Update the UI for the top bar
     * 
     * Implementation of function from IUpdatableUI interface
     */
    private void UpdateTopBarUI()
    {
        turnCounterText.text = $"Turn: {GameManager.Instance.Turn}";
    }

    /*
     * Begins the round timer
     */
    public void StartTimer()
    {
        timer = 0;
        StartCoroutine(Timer());
    }

    /*
     * Stops the round timer
     */
    public void StopTimer()
    {
        StopAllCoroutines();
    }

    /*
     * Increments the round timer after being started, until the coroutine is halted
     */
    private IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitUntil(() => !ManagerIndex.MI.TimeManager.IsState(TimeManager.TimeState.Paused));
            timer++;
            timerText.text = $"Time: {timer/60:D2}:{timer%60:D2}";
            yield return new WaitForSecondsRealtime(1f);
        }
    }
}
