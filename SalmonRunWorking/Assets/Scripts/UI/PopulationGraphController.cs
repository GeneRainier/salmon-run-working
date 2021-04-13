using System.Collections.Generic;
using static FishGenomeUtilities;

public class PopulationGraphController : BottomBarContentPage
{
    // Reference to the Post Run Stats Controller to update the output file
    public PostRunStatsPanelController updatePanel;
    
    // graph for successful vs. active vs. dead fish
    public HorizontalBarGraph populationGraph;

    // graph for female vs male fish
    public HorizontalBarGraph sexGraph;

    // graph for small vs medium vs large fish
    public HorizontalBarGraph sizeGraph;

    /**
     * Start is called before the first frame update
     */
    void Start()
    {
        // set up event calls
        GameEvents.onFishPopulationChanged.AddListener(UpdatePopulationData);
    }

    /**
     * Update UI to match updated population data
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
