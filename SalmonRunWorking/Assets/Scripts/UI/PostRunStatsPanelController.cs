using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static FishGenomeUtilities;


public class PostRunStatsPanelController : PanelController
{
    // descriptor text for each field on the panel
    public string smallDescriptor;
    public string mediumDescriptor;
    public string largeDescriptor;
    public string femaleDescriptor;
    public string maleDescriptor;

    // panel's title, which displays the turn number
    public TextMeshProUGUI titleText;

    // text for parent data
    public UIElement parentSmallText;
    public UIElement parentMediumText;
    public UIElement parentLargeText;

    // text for offspring data
    public UIElement offspringSmallText;
    public UIElement offspringMediumText;
    public UIElement offspringLargeText;
    public UIElement offspringFemaleText;
    public UIElement offspringMaleText;

    // flag for case where there are no offspring left and we have to do something different when leaving the panel
    private bool noOffspring;

    /**
     * Called before the first frame update
     */
    private void Start()
    {
        // subscribe to events
        GameEvents.onNewGeneration.AddListener(OnNewGeneration);
        GameEvents.onStartRunStats.AddListener(Activate);

        // set inactive by default
        Deactivate();
    }

    /**
     * Handle pressing of Next Run button
     */
    public void OnNextRunButton()
    {
        // deactivate the panel
        Deactivate();

        // if there is fish in the next generation, just move on to place state
        if (!noOffspring) GameManager.Instance.SetState(new PlaceState());

        // otherwise, go to end panel
        else GameManager.Instance.SetState(new EndState(EndGame.Reason.NoOffspring));
    }

    /**
     * Handle event of a new generation
     */
    private void OnNewGeneration(List<FishGenome> parentGenomes, List<FishGenome> offspringGenomes)
    {
        UpdatePanelData(parentGenomes, offspringGenomes);

        // TODO: this probably should not be handled here in this way
        // something else should have the responsibility of checking if the game should be over -- I'm just trying to work quickly
        if (offspringGenomes.Count == 0) OnNoOffspring();
    }

    /**
     * Update the data that will be displayed on the panel
     */
    private void UpdatePanelData(List<FishGenome> parentGenomes, List<FishGenome> offspringGenomes)
    {
        int previousTurn = GameManager.Instance.Turn - 1;
        
        // display the previous turn (because that's what
        titleText.text = $"Turn {previousTurn} Summary";

        // if a full turn has been done, do the typical update
        if (previousTurn > 0)
        {
            // parent genome can be null if this is the initial generation and there are no parents
            // have to treat these two cases differently
            if (parentGenomes != null)
            {
                parentSmallText.SetText(smallDescriptor, FindSmallGenomes(parentGenomes).Count);
                parentMediumText.SetText(mediumDescriptor, FindMediumGenomes(parentGenomes).Count);
                parentLargeText.SetText(largeDescriptor, FindLargeGenomes(parentGenomes).Count);
            }
            else
            {
                Debug.LogError("Error -- no parent genomes! should not happen!");
            }

            offspringSmallText.SetText(smallDescriptor, FindSmallGenomes(offspringGenomes).Count);
            offspringMediumText.SetText(mediumDescriptor, FindMediumGenomes(offspringGenomes).Count);
            offspringLargeText.SetText(largeDescriptor, FindLargeGenomes(offspringGenomes).Count);
            offspringFemaleText.SetText(femaleDescriptor, FindFemaleGenomes(offspringGenomes).Count);
            offspringMaleText.SetText(maleDescriptor, FindMaleGenomes(offspringGenomes).Count);
        }
        // otherwise, do the first-turn specific update
        else if (previousTurn == 0)
        {
            parentSmallText.SetText();
            parentMediumText.SetText();
            parentLargeText.SetText();

            offspringSmallText.SetText(smallDescriptor, FindSmallGenomes(offspringGenomes).Count);
            offspringMediumText.SetText(mediumDescriptor, FindMediumGenomes(offspringGenomes).Count);
            offspringLargeText.SetText(largeDescriptor, FindLargeGenomes(offspringGenomes).Count);
            offspringFemaleText.SetText(femaleDescriptor, FindFemaleGenomes(offspringGenomes).Count);
            offspringMaleText.SetText(maleDescriptor, FindMaleGenomes(offspringGenomes).Count);
        }
    }

    /**
     * Handle instance where there are no offpsring in the new generation
     */
    private void OnNoOffspring()
    {
        noOffspring = true;
    }
}

[Serializable]
public class UIElement
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI value;
    
    public const string divider = ": ";

    public void SetText(string label = "N/A", int value = -1)
    {
        if (label != "N/A") label += divider;
        this.label.text = label;
        this.value.text = value == -1 ? "" : value.ToString();
    }
}
