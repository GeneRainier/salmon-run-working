using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static FishGenomeUtilities;

/*
 * Class that manages the end of run information that appears at the end of each wave
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class PostRunStatsPanelController : PanelController
{
    [SerializeField] private GameManager gameManager;       //< The Game Manager

    public ManagerIndex initializationValues;               //< The ManagerIndex with initialization values for a given tower

    // Descriptor text for each field on the panel
    public string smallDescriptor;
    public string mediumDescriptor;
    public string largeDescriptor;
    public string femaleDescriptor;
    public string maleDescriptor;

    // Counts for the surviving fish for the output file
    public int survivingSmallDescriptor = 0;
    public int survivingMediumDescriptor = 0;
    public int survivingLargeDescriptor = 0;
    public int survivingFemaleDescriptor = 0;
    public int survivingMaleDescriptor = 0;

    // TO DO: Connect SetStuck Function in Fish to this
    public int stuckFish = 0;                   //< Count for fish who died due to being stuck

    public TextMeshProUGUI titleText;           //< Panel's title, which displays the turn number

    // Text for parent data
    public UIElement parentSmallText;
    public UIElement parentMediumText;
    public UIElement parentLargeText;

    // Text for offspring data
    public UIElement offspringSmallText;
    public UIElement offspringMediumText;
    public UIElement offspringLargeText;
    public UIElement offspringFemaleText;
    public UIElement offspringMaleText;

    private bool noOffspring;               //< Flag for case where there are no offspring left and we have to do something different when leaving the panel

    /*
     * Awake is called after the initialization of gameobjects prior to the start of the game. This is used as an Initialization function
     */
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
        // Subscribe to events
        GameEvents.onNewGeneration.AddListener(OnNewGeneration);
        GameEvents.onStartRunStats.AddListener(Activate);

        // Set inactive by default
        Deactivate();
    }

    /**
     * Handle pressing of Next Run button
     */
    public void OnNextRunButton()
    {
        // Deactivate the panel
        Deactivate();

        Debug.Log("OnNextRunButton() noOffspring = " + noOffspring);
        // If there is fish in the next generation, just move on to place state
        if (!noOffspring) GameManager.Instance.SetState(new PlaceState());

        // Otherwise, go to end panel
        else GameManager.Instance.SetState(new EndState(EndGame.Reason.NoOffspring));
    }

    /**
     * Handle event of a new generation
     * 
     * @param parentGenomes The list of genomes associated with the parent generation of salmon
     * @param offspringGenome The list of genomes associated with the children generation of salmon
     */
    private void OnNewGeneration(List<FishGenome> parentGenomes, List<FishGenome> offspringGenomes)
    {
        UpdatePanelData(parentGenomes, offspringGenomes);

        // TODO: this probably should not be handled here in this way
        // Something else should have the responsibility of checking if the game should be over -- I'm just trying to work quickly
        if (offspringGenomes.Count == 0) OnNoOffspring();
    }

    /**
     * Update the data that will be displayed on the panel
     * 
     * @param parentGenomes The list of genomes associated with the parent generation of salmon
     * @param offspringGenome The list of genomes associated with the children generation of salmon
     */
    private void UpdatePanelData(List<FishGenome> parentGenomes, List<FishGenome> offspringGenomes)
    {
        int previousTurn = GameManager.Instance.Turn - 1;
        
        // Display the previous turn (because that's what
        titleText.text = $"Turn {previousTurn} Summary";

        // If a full turn has been done, do the typical update
        if (previousTurn > 0)
        {
            // Parent genome can be null if this is the initial generation and there are no parents
            // Have to treat these two cases differently
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

            // Comment out the line below before webGL build
            // Update data file based on population data
            AppendOutputFile(previousTurn, parentGenomes, offspringGenomes);
        }
        // Otherwise, do the first-turn specific update
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

            // Comment out the line below before webGL build  
            // Creates data file based on game parameters 
            CreateOutputFile(previousTurn, parentGenomes, offspringGenomes);
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
     * 
     * @param previousTurn The turn number we are on
     * @param parentGenomes The list of genomes associated with the parent generation of salmon
     * @param offspringGenome The list of genomes associated with the children generation of salmon
     */
    private void CreateOutputFile(int previousTurn, List<FishGenome> parentGenomes, List<FishGenome> offspringGenomes)
    {
        // Create a txt file for post run information
        //string infoText = "Angler Radius, " + initializationValues.anglerRadius + "\nAngler Small Catch Rate, " + initializationValues.anglerSmallCatchRate +
        //    "\nAngler Medium Catch Rate, " + initializationValues.anglerMediumCatchRate + "\nAngler Large Catch Rate, " + initializationValues.anglerLargeCatchRate +
        //    "\nRanger Radius, " + initializationValues.rangerRadius + "\nRanger Small Catch Modifier, " + initializationValues.rangerSmallModifier +
        //    "\nRanger Medium Catch Modifier, " + initializationValues.rangerMediumModifier + "\nRanger Large Catch Modifier, " + initializationValues.rangerLargeModifier +
        //    "\nDefault Dam Pass Rate, " + initializationValues.defaultDamPassRate + "\nLadder Small Pass Rate, " + initializationValues.ladderSmallPassRate +
        //    "\nLadder Medium Pass Rate, " + initializationValues.ladderMediumPassRate + "\nLadder Large Pass Rate, " + initializationValues.ladderLargePassRate +
        //    "\nRamp Small Pass Rate, " + initializationValues.rampSmallPassRate + "\nRamp Medium Pass Rate, " + initializationValues.rampMediumPassRate +
        //    "\nRamp Large Pass Rate, " + initializationValues.rampLargePassRate + "\nLift Small Pass Rate, " + initializationValues.liftSmallPassRate +
        //    "\nLift Medium Pass Rate, " + initializationValues.liftMediumPassRate + "\nLift Large Pass Rate, " + initializationValues.liftLargePassRate +
        //    "\nStarting Money, " + initializationValues.startingMoney + "\nSealion Appearance Time, " + initializationValues.sealionAppearanceTime + 
        //    "\nSealion Male Catch Rate, " + initializationValues.sealionMaleCatchRate + "\nSealion Female Catch Rate, " + initializationValues.sealionFemaleCatchRate +
        //    "\nNesting Sites, " + initializationValues.nestingSites + "\n" +
        //    "\n" + "turn = Turn number\noffSh / offMd / offLg = Short / Medium / Large offspring\noffMa / offFe = Male / Female Offspring\n" +
        //    "parSh / parMd / parLg = Short / Medium / Large Parents\nsurMa / surFe = Male / Female Survivors\nsurSh / surMd / surLg = Short / Medium / Large Survivors\n" +
        //    "angB_ / angU_ = Angler Count Down / Upstream\nangBR / ang UR = Angler count Down / Upstream affected by Rangers\nrang = Number of Rangers\n" +
        //    "dam = Dam Present\nladder = Ladder Present on Dam\nslPres = Sealion Present\n" +
        //    "\n" +
        //    "turn, offSh, offMd, offLg, offMa, offFe, parSh, parMd, parLg, surMa, surFe, surSh, surMd, surLg, angB_, angU_, angBR, angUR, rang, dam, ladder, slPres\n" +
        //    previousTurn + ", " + FindSmallGenomes(offspringGenomes).Count + ", " + FindMediumGenomes(offspringGenomes).Count + ", " + FindLargeGenomes(offspringGenomes).Count +
        //    ", " + FindMaleGenomes(offspringGenomes).Count + ", " + FindFemaleGenomes(offspringGenomes).Count + ", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0\n";
        //System.IO.File.WriteAllText(string.Format(Application.dataPath + "/salmonrun.txt", DateTime.Now.ToString("YYYY_MDD_HHMM")), infoText);
    }

    /*
     * Append another line to the output file
     * 
     * @param previousTurn The turn number we are on
     * @param parentGenomes The list of genomes associated with the parent generation of salmon
     * @param offspringGenome The list of genomes associated with the children generation of salmon
     */
    private void AppendOutputFile(int previousTurn, List<FishGenome> parentGenomes, List<FishGenome> offspringGenomes)
    {
        // Create a txt file for post run information
        //string infoText = previousTurn + ", " + FindSmallGenomes(offspringGenomes).Count + ", " + FindMediumGenomes(offspringGenomes).Count + ", " + FindLargeGenomes(offspringGenomes).Count +
        //    ", " + FindMaleGenomes(offspringGenomes).Count + ", " + FindFemaleGenomes(offspringGenomes).Count + ", " + FindSmallGenomes(parentGenomes).Count + ", " + 
        //    FindMediumGenomes(parentGenomes).Count + ", " + FindLargeGenomes(parentGenomes).Count + ", " + survivingMaleDescriptor + ", " +
        //    survivingFemaleDescriptor + ", " + survivingSmallDescriptor + ", " + survivingMediumDescriptor + ", " + survivingLargeDescriptor + ", "
        //    + initializationValues.lowerAnglerCount + ", " + initializationValues.upperAnglerCount + ", " + initializationValues.lowerManagedAnglerCount + ", " + 
        //    initializationValues.upperManagedAnglerCount + ", " + initializationValues.rangerCount + ", " + initializationValues.damPresent + ", " + 
        //    initializationValues.ladderCode + ", " + initializationValues.sealionPresent + "\n";
        //System.IO.File.AppendAllText(string.Format(Application.dataPath + "/salmonrun.txt", DateTime.Now.ToString("YYYY_MDD_HHMM")), infoText);
    }
}

/*
 * Class to describe a singular UI element in the post run panel
 */
[Serializable]
public class UIElement
{
    [SerializeField] private TextMeshProUGUI label = null;     //< The title label of the UI element
    [SerializeField] private TextMeshProUGUI value = null;     //< The content of the UI element
    
    public const string divider = ": ";         //< A simple text divider to separate values in the post run panel

    /*
     * Sets the text in the UI element to be equal to the necessary label and value
     * 
     * @param lable The title of this section of the panel
     * @param value The content of this panel section
     */
    public void SetText(string label = "N/A", int value = -1)
    {
        if (label != "N/A") label += divider;
        this.label.text = label;
        this.value.text = value == -1 ? "" : value.ToString();
    }
}
