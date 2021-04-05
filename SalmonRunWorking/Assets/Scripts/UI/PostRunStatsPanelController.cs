using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static FishGenomeUtilities;


public class PostRunStatsPanelController : PanelController
{
    // The Game Manager
    [SerializeField] private GameManager gameManager;

    // The ManagerIndex with initialization values for a given tower
    public ManagerIndex initializationValues;

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

    private void Awake()
    {
        // Get initialization values and set this towers basic values
        initializationValues = FindObjectOfType<ManagerIndex>();
    }

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
                parentSmallText.SetText(smallDescriptor, FishGenomeUtilities.smallParent);
                parentMediumText.SetText(mediumDescriptor, FishGenomeUtilities.mediumParent);
                parentLargeText.SetText(largeDescriptor, FishGenomeUtilities.largeParent);
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

            //AppendOutputFile(previousTurn, parentGenomes, offspringGenomes);
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

           //CreateOutputFile(previousTurn, parentGenomes, offspringGenomes);
        }
    }

    /**
     * Handle instance where there are no offpsring in the new generation
     */
    private void OnNoOffspring()
    {
        noOffspring = true;
    }

    /*
     * Create the initial lines of the output table
     */
    private void CreateOutputFile(int previousTurn, List<FishGenome> parentGenomes, List<FishGenome> offspringGenomes)
    {
        // Create a txt file for post run information
        string infoText = "Angler Radius, " + initializationValues.anglerRadius + "\nAngler Small Catch Rate, " + initializationValues.anglerSmallCatchRate +
            "\nAngler Medium Catch Rate, " + initializationValues.anglerMediumCatchRate + "\nAngler Large Catch Rate, " + initializationValues.anglerLargeCatchRate +
            "\nRanger Radius, " + initializationValues.rangerRadius + "\nRanger Small Catch Modifier, " + initializationValues.rangerSmallModifier +
            "\nRanger Medium Catch Modifier, " + initializationValues.rangerMediumModifier + "\nRanger Large Catch Modifier, " + initializationValues.rangerLargeModifier +
            "\nDefault Dam Pass Rate, " + initializationValues.defaultDamPassRate + "\nLadder Small Pass Rate, " + initializationValues.ladderSmallPassRate +
            "\nLadder Medium Pass Rate, " + initializationValues.ladderMediumPassRate + "\nLadder Large Pass Rate, " + initializationValues.ladderLargePassRate +
            "\nRamp Small Pass Rate, " + initializationValues.rampSmallPassRate + "\nRamp Medium Pass Rate, " + initializationValues.rampMediumPassRate +
            "\nRamp Large Pass Rate, " + initializationValues.rampLargePassRate + "\nLift Small Pass Rate, " + initializationValues.liftSmallPassRate +
            "\nLift Medium Pass Rate, " + initializationValues.liftMediumPassRate + "\nLift Large Pass Rate, " + initializationValues.liftLargePassRate +
            "\nStarting Money, " + initializationValues.startingMoney + "\nSealion Appearance Time, " + initializationValues.sealionAppearanceTime + 
            "\nNesting Sites, " + initializationValues.nestingSites + "\n" +
            "\n" +
            "turn, offSh, offMd, offLg, offMa, offFe, parSh, parMd, parLg, angB_, angU_, angBR, angUR, rang, dam, ladder\n" +
            previousTurn + ", " + FindSmallGenomes(offspringGenomes).Count + ", " + FindMediumGenomes(offspringGenomes).Count + ", " + FindLargeGenomes(offspringGenomes).Count +
            ", " + FindMaleGenomes(offspringGenomes).Count + ", " + FindFemaleGenomes(offspringGenomes).Count + ", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0\n";
        System.IO.File.WriteAllText(string.Format(Application.dataPath + "/salmonrun.txt", DateTime.Now.ToString("YYYY_MDD_HHMM")), infoText);
    }

    /*
     * Append another line to the output file
     */
    private void AppendOutputFile(int previousTurn, List<FishGenome> parentGenomes, List<FishGenome> offspringGenomes)
    {
        // Create a txt file for post run information
        string infoText = previousTurn + ", " + FindSmallGenomes(offspringGenomes).Count + ", " + FindMediumGenomes(offspringGenomes).Count + ", " + FindLargeGenomes(offspringGenomes).Count +
            ", " + FindMaleGenomes(offspringGenomes).Count + ", " + FindFemaleGenomes(offspringGenomes).Count + ", " + FindSmallGenomes(parentGenomes).Count + ", " + 
            FindMediumGenomes(parentGenomes).Count + ", " + FindLargeGenomes(parentGenomes).Count + ", " + initializationValues.lowerAnglerCount + ", " + 
            initializationValues.upperAnglerCount + ", " + initializationValues.lowerManagedAnglerCount + ", " + initializationValues.upperManagedAnglerCount + 
            ", " + initializationValues.rangerCount + ", " + initializationValues.damPresent + ", " + initializationValues.ladderCode + "\n";
        System.IO.File.AppendAllText(string.Format(Application.dataPath + "/salmonrun.txt", DateTime.Now.ToString("YYYY_MDD_HHMM")), infoText);
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
