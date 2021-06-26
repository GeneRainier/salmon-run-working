using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * Class that describes a sealion
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
[RequireComponent(typeof(LineRenderer))]
public class SealionTower : TowerBase
{
    [Range(0f, 1f)]
    public float defaultCatchRate;          //< Default rate of success of a fish catch attempt

    // Default rates of success of fish catch attempt for small, medium and large fish
    public float defaultFemaleCatchRate = 0.1F;
    public float defaultMaleCatchRate = 0.1F;

    // Current rates of success of fish catch attempt for small, medium, and large fish
    [SerializeField] private float currentFemaleCatchRate;
    [SerializeField] private float currentMaleCatchRate;

    private List<Fish> caughtFish;          //< Fish that have been caught by this sealion

    public Material flashMaterial;          //< Material that will enable when a fish is caught by the sealion

    public int numFlashesPerCatch;          //< How many times the fish will flash in and out to show it is being caught

    [SerializeField] private TowerManager theTowerManager;      // Reference to the Tower Manager

    /*
     * Awake is called after the initialization of gameobjects prior to the start of the game
     */
    protected override void Awake()
    {
        // Base class awake
        base.Awake();

        defaultFemaleCatchRate = initializationValues.sealionFemaleCatchRate;
        defaultMaleCatchRate = initializationValues.sealionMaleCatchRate;

        // Set current catch rates
        currentFemaleCatchRate = defaultCatchRate * defaultFemaleCatchRate;
        currentMaleCatchRate = defaultCatchRate * defaultMaleCatchRate;

        initializationValues.sealionPresent = 1;

        //Debug.Log("cScr=" + currentSmallCatchRate + "; cMcr=" + currentMediumCatchRate + "; cLcr=" + currentLargeCatchRate);
    }

    /*
     * Start is called before the first frame update
     */
    protected override void Start()
    {
        TowerActive = true;
        paused = false;

        base.Start();

        caughtFish = new List<Fish>();

    }


    /*
     * Triggers the coroutine to alter the sealion catch rates
     * 
     * @param smallEffect The effect on the small catch rate
     * @param mediumEffect The effect on the medium catch rate
     * @param largeEffect The effect on the large catch rate
     * @param length The amount of time it takes for the catch to trigger
     */
    public void AffectCatchRate(float smallEffect, float mediumEffect, float largeEffect, float length)
    {
        StartCoroutine(AffectCatchRateCoroutine(smallEffect, largeEffect, length));
    }

    /*
     * Enacts the effect of a tower over the area within range
     */
    protected override void ApplyTowerEffect()
    {

        //Debug.Log("do Sealion Stuff");

        // Get all fish that aren't already being caught
        Collider[] fishColliders = Physics.OverlapSphere(transform.position, GetEffectRadius(), LayerMask.GetMask(Layers.FISH_LAYER_NAME))
            .Where(fishCollider => !fishCollider.GetComponent<Fish>()?.beingCaught ?? true).ToArray();

        // Select one of the fish
        if (fishColliders.Length <= 0) return;

        //Fish fish = fishColliders[Random.Range(0, fishColliders.Length)].GetComponent<Fish>();


        foreach (Collider fishCollider in fishColliders)
        {
            Fish fish = fishCollider.GetComponent<Fish>();

            if (fish)
            {
                TryCatchFish(fish);

            }
            else
            {
                Debug.LogError("Error with selecting random fish to catch -- should not happen!");
            }
        }
    }

    /*
     * Triggers the coroutine for a sealion to attempt to catch a given fish
     * 
     * @param f The fish being caught
     */
    private void TryCatchFish(Fish f)
    {
        StartCoroutine(TryCatchFishCoroutine(f));
    }

    /*
     * The sealion attempts to catch a given fish
     * 
     * @param fish The fish the sealion is catching
     */
    private IEnumerator TryCatchFishCoroutine(Fish fish)
    {
        // How likely we are to catch fish is dependent on what size the fish is
        // Determine that now
        float catchRate;
        float weight;
        FishGenePair sizeGenePair = fish.GetGenome()[FishGenome.GeneType.Size];
        switch (sizeGenePair.momGene)
        {
            case FishGenome.X when sizeGenePair.dadGene == FishGenome.X:
                catchRate = currentFemaleCatchRate;
                weight = 2f;  //4
                Debug.Log("bbCatchR=" + catchRate + "; weight=" + weight);
                break;
            case FishGenome.X when sizeGenePair.dadGene == FishGenome.Y:
                catchRate = currentMaleCatchRate;
                weight = 9f;  //15
                Debug.Log("BBCatchR=" + catchRate + "; weight=" + weight);
                break;
            default:
                catchRate = currentMaleCatchRate;
                //weight = 6f;  //9
                //Debug.Log("BbCatchR=" + catchRate + "; weight=" + weight);
                break;
        }
        Debug.Log("SeaLionCatch: MaleCR=" + currentMaleCatchRate + "FemCR=" + currentFemaleCatchRate);

        // Figure out whether the fish will be caught or not
        bool caught = Random.Range(0f, 1f) <= catchRate;

        // Handle fish being caught
        if (caught)
        {
            // Tell the fish that it is being caught
            fish.StartCatch();

            float timeToWait = (float)timePerApplyEffect / numFlashesPerCatch / 2f;

            // Make the fish flash  for a bit
            SkinnedMeshRenderer fishRenderer = fish.GetComponentInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < numFlashesPerCatch; i++)
            {
                Material oldMaterial = fishRenderer.material;
                fishRenderer.material = flashMaterial;
                yield return new WaitForSeconds(timeToWait);
                Destroy(fishRenderer.material);
                fishRenderer.material = oldMaterial;
                yield return new WaitForSeconds(timeToWait);
            }

            // Actually catch the fish
            fish.Catch();
            caughtFish.Add(fish);

        }
        // Fish escaped -- just wait for end of action
        else
        {
            yield return new WaitForSeconds(timePerApplyEffect);
        }
    }

    /*
     * Triggers the coroutine to alter the sealion catch rates
     * 
     * @param smallEffect The effect on the small catch rate
     * @param mediumEffect The effect on the medium catch rate
     * @param largeEffect The effect on the large catch rate
     * @param length The amount of time it takes for the catch to trigger
     */
    private IEnumerator AffectCatchRateCoroutine(float femaleEffect, float maleEffect, float length)
    {
        currentFemaleCatchRate += femaleEffect;
        currentMaleCatchRate += maleEffect;

        yield return new WaitForSeconds(length);

        currentFemaleCatchRate -= femaleEffect;
        currentMaleCatchRate -= maleEffect;
    }

    /*
     * TowerBase abstract function implementation
     * THIS IS ALL JUST HERE BECAUSE TOWERBASE REQUIRES IT, IT DOESN'T ACTUALLY DO ANYTHING
     */
    protected override bool TowerPlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        int correctLayer = LayerMask.NameToLayer(Layers.TERRAIN_LAYER_NAME);

        // For placement to be valid, primary raycast must have hit a gameobject on the Terrain layer
        if (primaryHitInfo.collider && primaryHitInfo.collider.gameObject.layer == correctLayer)
        {
            // Secondary raycasts must also hit gameobjects on the Terrain layer at approximately the same z-pos as the primary raycast
            return secondaryHitInfo.TrueForAll(hitInfo => hitInfo.collider &&
                                                            hitInfo.collider.gameObject.layer == correctLayer &&
                                                            Mathf.Abs(hitInfo.point.z - primaryHitInfo.point.z) < 1f);
        }
        
        // If one of these conditions was not met, return false
        return false;
    }

    /**
     * Position the fisherman at the correct location using the information from a raycast
     * 
     * @param primaryHitInfo RaycastHit The results of the primary raycast that was done
     * @param secondaryHitInfo List RaycastHit The results of any secondary raycasts that were done
     */
    protected override void PlaceTower(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        transform.position = primaryHitInfo.point;
        theTowerManager = FindObjectOfType<TowerManager>();
        //theTowerManager.AddAngler(this);
    }
}
