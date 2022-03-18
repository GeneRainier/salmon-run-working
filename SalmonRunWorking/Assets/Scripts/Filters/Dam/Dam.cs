using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/**
 * A dam restricts fish access to upstream areas
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class Dam : TowerBase
{
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

    private BoxCollider dropOffBox;     //< Box where fish will be dropped off if the successfully pass the dam

    protected bool active = false;      //< Is this filter currently active

    /**
     * Start is called before the first frame update
     */
    protected override void Start()
    {
        // Get initialization values and set this towers basic values
        initValues = FindObjectOfType<ManagerIndex>();

        // Set all crossing rates to default rate on initialization
        smallCrossingRate = mediumCrossingRate = largeCrossingRate = defaultCrossingRate;
    }

    /**
     * Handle objects entering the collider
     */
    private void OnTriggerEnter(Collider other)
    {
        // Only care about triggers if the filter is active
        if (active)
        {
            // Check what hit us
            // Only care if it's a fish
            Fish f = other.gameObject.GetComponentInChildren<Fish>();
            if (f != null)
            {
                ApplyFilterEffect(f);
            }
        }
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
        smallCrossingRate = defaultCrossingRate + damLadder.smallCrossingRate;
        mediumCrossingRate = defaultCrossingRate + damLadder.mediumCrossingRate;
        largeCrossingRate = defaultCrossingRate + damLadder.largeCrossingRate;
    }

    #endregion

    #region TowerBase (Base Class) Implementation

    /**
     * Place the dam onto the game map
     * 
     * @param primaryHitInfo The information from the raycast originating from the camera.
     * @param secondaryHitInfo The information from the raycasts surrounding the tower being placed.
     */
    public override void Place(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
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
            initValues.damPresent = 1;
            towerManager.AddTower(this);
            turnPlaced = GameManager.Instance.Turn;
        }
    }

    /**
     * Figure out if we can place the dam at the location of a given raycast
     * 
     * @param primaryHitInfo The information from the raycast originating from the camera.
     * @param secondaryHitInfo The information from the raycasts surrounding the tower being placed.
     */
    public override bool PlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
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

    /**
     * Apply effects of this tower
     * 
     * The Dam has its own unique placement, but is a tower nonetheless
     */
    protected override void ApplyTowerEffect()
    {
        return;
    }

    /**
     * Determines whether a tower placement is valid
     * 
     * @param primaryHitInfo The raycast info from the main camera raycast
     * @param secondaryHitInfo The raycast info from the bounds of the tower
     * 
     * The Dam has its own unique placement, but is a tower nonetheless
     */
    protected override bool TowerPlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        return true;
    }

    /**
     * Places a tower into the environment   
     * 
     * @param primaryHitInfo The raycast info from the main camera raycast
     * @param secondaryHitInfo The raycast info from the bounds of the tower
     * 
     * The Dam has its own unique placement, but is a tower nonetheless
     */
    protected override void PlaceTower(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        return;
    }

    #endregion

    #region Dam Functionality

    /**
     * Apply the effect of the dam
     * 
     * Fish will attempt to cross the dam and may be able to pass or "get stuck" and die
     * 
     * @param fish The fish trying to pass by the dam
     */
    public void ApplyFilterEffect(Fish fish)
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
            }
            else if (sizeGenePair.momGene == FishGenome.B && sizeGenePair.dadGene == FishGenome.B)
            {
                crossingRate = largeCrossingRate;
            }
            else
            {
                crossingRate = mediumCrossingRate;
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
