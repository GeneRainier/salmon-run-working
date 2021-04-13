using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/**
 * A dam restricts fish access to upstream areas
 */
public class Dam : FilterBase, IDragAndDropObject
{
    // The ManagerIndex with initialization values for a given tower
    public ManagerIndex initializationValues;

    // default crossing rate for all fish
    [Range(0f, 1f)]
    public float defaultCrossingRate;
    // initialized in Unity
    // Project -> Assets -> Prefabs -> Filters -> Dam -> DamLadder
    // (make sure to double click on Dam cube symbol. 

    // crossing rates for small, medium, and large fish
    [SerializeField] private float smallCrossingRate;
    [SerializeField] private float mediumCrossingRate;
    [SerializeField] private float largeCrossingRate;

    // is there a ladder currently attached to the dam?
    private bool hasLadder;

    // box where fish will be dropped off if the successfully pass the dam
    private BoxCollider dropOffBox;

    /**
     * Start is called before the first frame update
     */
    void Start()
    {
        // Get initialization values and set this towers basic values
        initializationValues = FindObjectOfType<ManagerIndex>();
        //defaultCrossingRate = initializationValues.defaultDamPassRate;
        // set all crossing rates to default rate on initialization
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
     * Add a ladder to this dam
     * 
     * Set a flag so we can apply ladder effects later
     */
    public void AddLadder(DamLadder damLadder)
    {
        Debug.Log("damLadder S=" + damLadder.smallCrossingRate + "; M=" + damLadder.mediumCrossingRate + "; L=" + damLadder.largeCrossingRate);
        // set crossing rates for fish to ones supplied by the ladder
        //smallCrossingRate = defaultCrossingRate + damLadder.smallCrossingRate;
        //mediumCrossingRate = defaultCrossingRate + damLadder.mediumCrossingRate;
        //largeCrossingRate = defaultCrossingRate + damLadder.largeCrossingRate;

        //smallCrossingRate = defaultCrossingRate + 0.2F;
        //mediumCrossingRate = defaultCrossingRate + 0.7F;
        //largeCrossingRate = defaultCrossingRate + 0.7F;

        // set flag so we know we have a ladder
        hasLadder = true;
    }

    #endregion

    #region IDragAndDropObject Implementation

    /**
     * Place the dam onto the game map
     */
    public void Place(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        if (!ManagerIndex.MI.TowerManager.GetTower(TowerType.Dam).CanAfford())
        {
            Debug.Log("Insufficient Funds!");
            return;
        }
        
        // can only place if we are over a dam placement location
        DamPlacementLocation placementLocation = primaryHitInfo.collider.gameObject.GetComponent<DamPlacementLocation>();
        if (placementLocation != null)
        {
            placementLocation.AttachDam(this);
            initializationValues.damPresent = 1;
        }

    }

    /**
     * Figure out if we can place the dam at the location of a given raycast
     */
    public bool PlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        // must have hit something
        if (!primaryHitInfo.collider) return false;
        
        DamPlacementLocation placementLocation = primaryHitInfo.collider.gameObject.GetComponent<DamPlacementLocation>();

        // thing we hit must be a dam placement location
        if (placementLocation != null)
        {
            // only return true if the placement location is not already in use
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
     */
    protected override void ApplyFilterEffect(Fish fish)
    {
        // only let it through if it hasn't been flagged as stuck
        if (!fish.IsStuck())
        {
            // chance between fish getting past the dam and being caught/getting stuck depends on what size the fish is
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
                // based on the crossing rate we figured out, roll for a crossing
                // if we pass, put the fish past the dam
                if (Random.Range(0f, 1f) <= crossingRate)
                {
                    fish.transform.position = GetRandomDropOff(fish.transform.position.z);
                    break;
                }
                // if we have not expended our total tries (based on damPassCounter in fish), increment, wait, and try again
                else if (fish.damPassCounter < 2)
                {
                    fish.damPassCounter++;
                    Invoke("DamPassCooldown", 6.0f);
                }
                // if it didn't make it, make it permanently stuck (so it can't try repeated times)
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
     * @param z float The z value of the point -- don't want to change the object's z so just pass it in
     */
    private Vector3 GetRandomDropOff(float z)
    {
        return new Vector3(
            Random.Range(dropOffBox.bounds.min.x, dropOffBox.bounds.max.x),
            Random.Range(dropOffBox.bounds.min.y, dropOffBox.bounds.max.y),
            z
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
