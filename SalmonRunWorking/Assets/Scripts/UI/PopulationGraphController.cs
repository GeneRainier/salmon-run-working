using System.Collections.Generic;
using static FishGenomeUtilities;

/*
 * Controller script to adjust the population UI counter graphs on the fly as the salmon population changes
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class PopulationGraphController : BottomBarContentPage
{
    public PostRunStatsPanelController updatePanel;         //< Reference to the Post Run Stats Controller to update the output file

    public HorizontalBarGraph populationGraph;              //< Graph for successful vs. active vs. dead fish

    public HorizontalBarGraph sexGraph;                     // Graph for female vs male fish

    public HorizontalBarGraph sizeGraph;                    //< Graph for small vs medium vs large fish

    /**
     * Start is called before the first frame update
     */
    void Start()
    {
        // Set up event calls
        GameEvents.onFishPopulationChanged.AddListener(UpdatePopulationData);
    }

    /**
     * Update UI to match updated population data
     * 
     * @param activeGenomes The fish in the scene who are still running
     * @param successfulGenomes The fish that have finished a run
     * @param deadGenomes The fish which have been caught by a tower or stuck by a filter
     */
    private void UpdatePopulationData(List<FishGenome> activeGenomes, List<FishGenome> successfulGenomes, List<FishGenome> deadGenomes)
    {
        populationGraph.UpdateGraph(successfulGenomes.Count, activeGenomes.Count, deadGenomes.Count);

        int numMales = FindMaleGenomes(successfulGenomes).Count + FindMaleGenomes(activeGenomes).Count;
        int numFemales = FindFemaleGenomes(successfulGenomes).Count + FindFemaleGenomes(activeGenomes).Count;
        sexGraph.UpdateGraph(numFemales, numMales);
        updatePanel.survivingMaleDescriptor = numMales;
        updatePanel.survivingFemaleDescriptor = numFemales;

        int numSmall = FindSmallGenomes(successfulGenomes).Count + FindSmallGenomes(activeGenomes).Count;
        int numMedium = FindMediumGenomes(successfulGenomes).Count + FindMediumGenomes(activeGenomes).Count;
        int numLarge = FindLargeGenomes(successfulGenomes).Count + FindLargeGenomes(activeGenomes).Count;
        sizeGraph.UpdateGraph(numSmall, numMedium, numLarge);
        updatePanel.survivingSmallDescriptor = numSmall;
        updatePanel.survivingMediumDescriptor = numMedium;
        updatePanel.survivingLargeDescriptor = numLarge;
    }
}
