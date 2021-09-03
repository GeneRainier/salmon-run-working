using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * Class that describes an angler
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
[RequireComponent(typeof(LineRenderer))]
public class AnglerTower : TowerBase
{
    public MeshRenderer flashRenderer;      //< Mesh renderer that will flash when the angler tower is affected by a ranger or another tower

    // Materials for line renderer that indicates whether a fish has been hit or missed
    public Material hitLineMaterial;
    public Material missLineMaterial;

    public Material flashMaterial;      //< Material that will 

    // Default rates of success of fish catch attempt for small, medium and large fish
    public float smallCatchRate = 0.7F;   
    public float mediumCatchRate = 0.4F;
    public float largeCatchRate = 0.1F;
    public int fishCaught = 0;          //< The number of fish this angler has caught

    private List<Fish> caughtFish;      //< Fish that have been caught by this angler

    private Fish catchAttemptFish;      //< Fish that the catch attempt line is pointing at

    private LineRenderer catchAttemptLine;      //< LineRenderer component used to display a catch or catch attempt

    // GameObject and Animator to start the fishing animation
    Animator fishing;
    public GameObject fisherman;
    private static readonly int Fishing = Animator.StringToHash("Fishing");

    public bool anglerCounted = false;      // Boolean for checking whether regulated angler has been counted yet or not

    /**
     * Start is called on object initialization
     */
    protected override void Awake()
    {
        // Base class awake
        base.Awake();

        // Set catch rates and radius based on Initialization values
        effectRadius = initializationValues.anglerRadius;
        smallCatchRate = initializationValues.anglerSmallCatchRate;
        mediumCatchRate = initializationValues.anglerMediumCatchRate;
        largeCatchRate = initializationValues.anglerLargeCatchRate;

        // This tells the upgrade manager what the small, medium, and large rates are that can be upgraded.
        ManagerIndex.MI.UpgradeManager.SmallRate = smallCatchRate;
        ManagerIndex.MI.UpgradeManager.MediumRate = mediumCatchRate;
        ManagerIndex.MI.UpgradeManager.LargeRate = largeCatchRate;

        Debug.Log("Awake cScr=" + smallCatchRate + "; cMcr=" + mediumCatchRate + "; cLcr=" + largeCatchRate);
    }

    /**
     * Called before the first frame update
     */
    protected override void Start()
    {
        base.Start();

        caughtFish = new List<Fish>();
        catchAttemptLine = GetComponent<LineRenderer>();
        catchAttemptLine.enabled = false;

        fishing = fisherman.GetComponent<Animator>();
    }

    /**
     * Update is called once per frame
     */
    private void Update()
    {
        // Update fish line position
        if (catchAttemptLine.enabled)
        {
            SetLinePos();
        }
    }

    /**
     * Affect the catch rate of a fish for a certain amount of time
     * 
     * @param smallEffect float The value that the current catch rate will be modified by for small fish
     * @param mediumEffect float The value that the current catch rate will be modified by for medium fish
     * @param largeEffect float The value that the current catch rate will be modified by for large fish
     * @param length float The amount of time (in seconds) that the effect will last
     */
    public void AffectCatchRate(float smallEffect, float mediumEffect, float largeEffect, float length)
    {
        StartCoroutine(AffectCatchRateCoroutine(smallEffect, mediumEffect, largeEffect, length));
    }

    /**
     * Determine whether a fisherman could be placed at the location specified by a raycast
     * 
     * @param primaryHitInfo RaycastHit The results of the raycast that was done
     * @param secondaryHitInfo List RaycastHit The results of any secondary raycasts that were done
     * @return bool Whether or not the placement location of the tower is valid
     */
    protected override bool TowerPlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        int correctLayer = LayerMask.NameToLayer(Layers.TERRAIN_LAYER_NAME);

        // For placement to be valid, primary raycast must have hit a gameobject on the Terrain layer
        if (primaryHitInfo.collider && primaryHitInfo.collider.gameObject.layer == correctLayer)
        {
            // Secondary raycasts must also hit gameobjects on the Terrain layer at approximately the same y-pos as the primary raycast
            return secondaryHitInfo.TrueForAll(hitInfo => hitInfo.collider &&
                                                            hitInfo.collider.gameObject.layer == correctLayer &&
                                                            Mathf.Abs(hitInfo.point.y - primaryHitInfo.point.y) < 1f);
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
        transform.parent.position = primaryHitInfo.point;
        towerManager.AddTower(this);
        turnPlaced = GameManager.Instance.Turn;
    }

    /**
     * Apply the effects of the fisherman tower
     */
    protected override void ApplyTowerEffect()
    {
        // Get all fish that aren't already being caught
        Collider[] fishColliders = Physics.OverlapSphere(transform.position, GetEffectRadius(), LayerMask.GetMask(Layers.FISH_LAYER_NAME))
            .Where(fishCollider => !fishCollider.GetComponent<Fish>()?.beingCaught??true).ToArray();

        // Select one of the fish
        if (fishColliders.Length <= 0) return;
        
        Fish fish = fishColliders[Random.Range(0, fishColliders.Length)].GetComponent<Fish>();

        if (fish)
        {
            transform.parent.LookAt(fish.transform, Vector3.up);
            TryCatchFish(fish);
            fishing.SetBool(Fishing, true);
        }
        else
        {
            Debug.LogError("Error with selecting random fish to catch -- should not happen!");
        }
    }

    /**
     * Attempt to catch a fish
     * 
     * @param f The fish being caught
     */
    private void TryCatchFish(Fish f)
    {
        StartCoroutine(TryCatchFishCoroutine(f));
    }

    /**
     * Display attempt to catch fish
     * 
     * @param fish The fish the fisherman is trying to catch
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
            case FishGenome.b when sizeGenePair.dadGene == FishGenome.b:
                catchRate = smallCatchRate;
                weight = 2f;  //4
                 //Debug.Log("bbCatchR=" + catchRate + "; weight=" + weight);
                break;
            case FishGenome.B when sizeGenePair.dadGene == FishGenome.B:
                catchRate = largeCatchRate;
                weight = 9f;  //15
                 //Debug.Log("BBCatchR=" + catchRate + "; weight=" + weight);
                break;
            default:
                catchRate = mediumCatchRate;
                weight = 6f;  //9
                 //Debug.Log("BbCatchR=" + catchRate + "; weight=" + weight);
                break;
        }
        Debug.Log("TryCatchFishCoroutine: cScr=" + smallCatchRate + "; cMcr=" + mediumCatchRate + "; cLcr=" + largeCatchRate);


        // Figure out whether the fish will be caught or not
        bool caught = Random.Range(0f, 1f) <= catchRate;

        // Do setup for catch attempt line visualizer
        catchAttemptFish = fish;

        Destroy(catchAttemptLine.material);
        catchAttemptLine.material = caught ? hitLineMaterial : missLineMaterial;

        catchAttemptLine.enabled = true;

        // Handle fish being caught
        if (caught)
        {
            // Tell the fish that it is being caught
            fish.StartCatch();

            // Trigger Water Splash effect on fish
            fish.waterSplash.Play();
            fish.swimSpeed = 0;
            yield return new WaitForSeconds(1);
            fish.fishRenderer.enabled = false;
            yield return new WaitForSeconds(1);

            // Actually catch the fish
            fish.Catch();
            caughtFish.Add(fish);
            
            // Add Appropriate Funds to Bank
            ManagerIndex.MI.MoneyManager.AddCatch(weight);

            // Increase this angler's fish caught total
            fishCaught++;
        }
        // Fish escaped -- just wait for end of action
        else
        {
            yield return new WaitForSeconds(timePerApplyEffect);
        }

        // End the catch attempt line
        catchAttemptLine.enabled = false;
    }

    /**
     * Coroutine for affecting the catch rate of the angler
     * 
     * @param smallEffect float The value that the catch rate will be modified by for small fish
     * @param mediumEffect float The value that the catch rate will be modified by for medium fish
     * @param largeEffect float The value that the catch rate will be modified by for large fish
     * @param length float The amount of time (seconds) that the effect will last
     */
    private IEnumerator AffectCatchRateCoroutine(float smallEffect, float mediumEffect, float largeEffect, float length)
    {
        // NOTE: With the Upgrade Manager Refactor, this coroutine will be reexamined to try and make the effect occur once (like a constant policy)

        smallCatchRate += smallEffect;
        mediumCatchRate += mediumEffect;
        largeCatchRate += largeEffect;

        //Debug.Log("before yield return cScr=" + currentSmallCatchRate + "; cMcr=" + currentMediumCatchRate + "; cLcr=" + currentLargeCatchRate);

        yield return new WaitForSeconds(length);

        smallCatchRate -= smallEffect;
        mediumCatchRate -= mediumEffect;
        largeCatchRate -= largeEffect;
    }

    /**
     * Set line position for a fish
     */
    private void SetLinePos()
    {
        var position = transform.position;
        /*
         * 2020-05-11 @WRE Error in game says object Fish has already been destroyed,
         * check it against null before attempting to access it. Trying that out.
         */
        if (null == catchAttemptFish)
        {
            return;
        }
        // Fish is not null, play on.
        Vector3 startPos = new Vector3(position.x, position.y, position.z);
        Vector3 fishPos = catchAttemptFish.transform.position;
        fishPos.y = startPos.y;

        catchAttemptLine.SetPositions(new []{ startPos, fishPos});
    }
}