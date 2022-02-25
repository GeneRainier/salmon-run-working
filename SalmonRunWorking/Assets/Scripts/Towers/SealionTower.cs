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
    // Default rates of success of fish catch attempt for small, medium and large fish
    public float femaleCatchRate = 0.1F;
    public float maleCatchRate = 0.1F;

    private List<Fish> caughtFish;          //< Fish that have been caught by this sealion

    /*
     * Awake is called after the initialization of gameobjects prior to the start of the game
     */
    protected override void Awake()
    {
        // Base class awake
        base.Awake();

        femaleCatchRate = initValues.initSets[initValues.setToUse].sealionFemaleCatchRate;
        maleCatchRate = initValues.initSets[initValues.setToUse].sealionMaleCatchRate;

        initValues.sealionPresent = 1;
    }

    /*
     * Start is called before the first frame update
     */
    protected override void Start()
    {
        TowerActive = true;
        paused = false;

        turnPlaced = GameManager.Instance.Turn;
        towerManager.AddTower(this);

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
        // Get all fish that aren't already being caught (the Sealion's effect range dictates the size of the overlap sphere here)
        Collider[] fishColliders = Physics.OverlapSphere(transform.position, GetEffectRadius(), LayerMask.GetMask(Layers.FISH_LAYER_NAME))
            .Where(fishCollider => !fishCollider.GetComponent<Fish>()?.beingCaught ?? true).ToArray();

        // Select one of the fish
        if (fishColliders.Length <= 0) return;

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
                catchRate = femaleCatchRate;
                weight = 2f;  //4
                Debug.Log("bbCatchR=" + catchRate + "; weight=" + weight);
                break;
            case FishGenome.X when sizeGenePair.dadGene == FishGenome.Y:
                catchRate = maleCatchRate;
                weight = 9f;  //15
                Debug.Log("BBCatchR=" + catchRate + "; weight=" + weight);
                break;
            default:
                catchRate = maleCatchRate;
                weight = 6f;  //9
                break;
        }
        Debug.Log("SeaLionCatch: MaleCR=" + maleCatchRate + "FemCR=" + femaleCatchRate);

        // Figure out whether the fish will be caught or not
        bool caught = Random.Range(0f, 1f) <= catchRate;

        // Handle fish being caught
        if (caught)
        {
            // Tell the fish that it is being caught
            fish.StartCatch();

            // Trigger Water Splash effect on fish
            fish.waterSplash.Play();
            fish.swimSpeed = 0;
            fish.fishRenderer.enabled = false;
            yield return new WaitForSeconds(2);

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
        femaleCatchRate += femaleEffect;
        maleCatchRate += maleEffect;

        yield return new WaitForSeconds(length);

        femaleCatchRate -= femaleEffect;
        maleCatchRate -= maleEffect;
    }

    /*
     * TowerBase abstract function implementation
     * THIS IS ALL JUST HERE BECAUSE TOWERBASE REQUIRES IT, IT (currently) DOESN'T ACTUALLY DO ANYTHING SINCE SEALIONS ARE SPAWNED NOT PLACED
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
        towerManager.AddTower(this);
        turnPlaced = GameManager.Instance.Turn;
    }
}
