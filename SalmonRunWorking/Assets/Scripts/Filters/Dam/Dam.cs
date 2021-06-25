using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/**
 * A dam restricts fish access to upstream areas
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class Dam : FilterBase, IDragAndDropObject
{
    public ManagerIndex initializationValues;       //< The ManagerIndex with initialization values for a given tower

    // Default crossing rate for all fish
    [Range(0f, 1f)]
    public float defaultCrossingRate;
    // Initialized in Unity
    // Project -> Assets -> Prefabs -> Filters -> Dam -> DamLadder
    // (make sure to double click on Dam cube symbol. 

    // Crossing rates for small, medium, and large fish
    [SerializeField] private float smallCrossingRate;
    [SerializeField] private float mediumCrossingRate;
    [SerializeField] private float largeCrossingRate;

    private bool hasLadder;             //< Is there a ladder currently attached to the dam?

    private BoxCollider dropOffBox;     //< Box where fish will be dropped off if the successfully pass the dam

    /**
     * Start is called before the first frame update
     */
    void Start()
    {
        // Get initialization values and set this towers basic values
        initializationValues = FindObjectOfType<ManagerIndex>();
        //defaultCrossingRate = initializationValues.defaultDamPassRate;

        // Set all crossing rates to default rate on initialization
        smallCrossingRate = mediumCrossingRate = largeCrossingRate = defaultCrossingRate;
        Debug.Log("Dam Class: defaultCrossingRate=" + defaultCrossingRate);
    }

    #region Dam Operation

    /**
     * Activate the dam
     * 
     * @param dropOffBox BoxCollider the box where succesfully passing fish will be dropped off
     */
    public void Activate(BoxCollider dropOffBox)
    {
        this.dropOffBox = dropOffBox;
        active = true;
    }

    /**
     * Add a ladder to this dam and set a flag so we can apply ladder effects later
     * 
     * @param damLadder The Ladder being added to the dam
     */
    public void AddLadder(DamLadder damLadder)
    {
        // Set crossing rates for fish to ones supplied by the ladder

        // If these lines are run then I get an error in UpgradeManager line 132
        // try 2
        smallCrossingRate = defaultCrossingRate + damLadder.smallCrossingRate;
        mediumCrossingRate = defaultCrossingRate + damLadder.mediumCrossingRate;
        largeCrossingRate = defaultCrossingRate + damLadder.largeCrossingRate;

        //smallCrossingRate = defaultCrossingRate + 0.2F;
        //mediumCrossingRate = defaultCrossingRate + 0.7F;
        //largeCrossingRate = defaultCrossingRate + 0.7F;

        Debug.Log("damLadder S=" + damLadder.smallCrossingRate + "; M=" + damLadder.mediumCrossingRate + "; L=" + damLadder.largeCrossingRate);

        // Set flag so we know we have a ladder
        hasLadder = true;
    }

    #endregion

    #region IDragAndDropObject Implementation

    /**
     * Place the dam onto the game map
     * 
     * @param primaryHitInfo The information from the raycast originating from the camera.
     * @param secondaryHitInfo The information from the raycasts surrounding the tower being placed.
     */
    public void Place(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        if (!ManagerIndex.MI.TowerManager.GetTower(TowerType.Dam).CanAfford())
        {
            Debug.Log("Insufficient Funds!");
            return;
        }
        
        // Can only place if we are over a dam placement location
        DamPlacementLocation placementLocation = primaryHitInfo.collider.gameObject.GetComponent<DamPlacementLocation>();
        if (placementLocation != null)
        {
            placementLocation.AttachDam(this);
            initializationValues.damPresent = 1;
        }

    }

    /**
     * Figure out if we can place the dam at the location of a given raycast
     * 
     * @param primaryHitInfo The information from the raycast originating from the camera.
     * @param secondaryHitInfo The information from the raycasts surrounding the tower being placed.
     */
    public bool PlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        // Must have hit something
        if (!primaryHitInfo.collider) return false;
        
        DamPlacementLocation placementLocation = primaryHitInfo.collider.gameObject.GetComponent<DamPlacementLocation>();

        // The thing we hit must be a dam placement location
        if (placementLocation != null)
        {
            // Only return true if the placement location is not already in use
            return !placementLocation.inUse;
        }

        return false;
    }

    #endregion

    #region Base Class (FilterBase) Implementation

    /**
     * Apply the effect of the dam
     * 
     * Fish will attempt to cross the dam and may be able to pass or "get stuck" and die
     * 
     * @param fish The fish trying to pass by the dam
     */
    protected override void ApplyFilterEffect(Fish fish)
    {
        // Only let it through if it hasn't been flagged as stuck
        if (!fish.IsStuck())
        {
            // Chance between fish getting past the dam and being caught/getting stuck depends on what size the fish is
            float crossingRate;

            Debug.Log("Dam.ApplyFilterEffect: SMcr =" + smallCrossingRate + "; MDcr = " + mediumCrossingRate + "; LGcr = " + largeCrossingRate);

            FishGenePair sizeGenePair = fish.GetGenome()[FishGenome.GeneType.Size];
            if (sizeGenePair.momGene == FishGenome.b && sizeGenePair.dadGene == FishGenome.b)
            {
                crossingRate = smallCrossingRate;
                // Debug.Log("bbCrossR=" + smallCrossingRate);
            }
            else if (sizeGenePair.momGene == FishGenome.B && sizeGenePair.dadGene == FishGenome.B)
            {
                crossingRate = largeCrossingRate;
                // Debug.Log("BBCrossR=" + largeCrossingRate);
            }
            else
            {
                crossingRate = mediumCrossingRate;
                // Debug.Log("BbCrossR=" + mediumCrossingRate);
            }

            while (!fish.IsStuck())
            {
                // Based on the crossing rate we figured out, roll for a crossing
                // If we pass, put the fish past the dam
                if (Random.Range(0f, 1f) <= crossingRate)
                {
                    fish.transform.position = GetRandomDropOff(fish.transform.position.y);
                    break;
                }
                // If we have not expended our total tries (based on damPassCounter in fish), increment, wait, and try again
                else if (fish.damPassCounter < 2)
                {
                    fish.damPassCounter++;
                    Invoke("DamPassCooldown", 6.0f);
                }
                // If it didn't make it, make it permanently stuck (so it can't try repeated times)
                else
                {
                    fish.SetStuck(true);
                    break;
                }
            }
        }
    }

    /**
     * Get a random point within the drop off collider
     * 
     * @param y float The y value of the point -- don't want to change the object's y so just pass it in
     */
    private Vector3 GetRandomDropOff(float y)
    {
        return new Vector3(
            Random.Range(dropOffBox.bounds.min.x, dropOffBox.bounds.max.x),
            y,
            Random.Range(dropOffBox.bounds.min.z, dropOffBox.bounds.max.z)
        );
    }

    /*
     * An empty function used to act as a cooldown for the next time a fish can try to pass the dam
     */
    private void DamPassCooldown()
    {

    }

    #endregion
}
