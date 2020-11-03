using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeftTopBarController : MonoBehaviour
{
    // turn counter text
    public TextMeshProUGUI turnCounterText;

    // coin text
    public TextMeshProUGUI coinText;

    //to trigger the sealion spawning
    public SealionSpawner triggerSpawn;

    [SerializeField] private TextMeshProUGUI timerText;
    private int timer;

    /**
     * Initialization function
     */
    private void OnEnable()
    {
        GameEvents.onTurnUpdated.AddListener(UpdateTopBarUI);
        GameEvents.onEndRun.AddListener(StopTimer);
        GameEvents.onStartRun.AddListener(StartTimer);
    }

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

    public void StartTimer()
    {
        timer = 0;
        StartCoroutine(Timer());
    }

    public void StopTimer()
    {
        StopAllCoroutines();
    }

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
