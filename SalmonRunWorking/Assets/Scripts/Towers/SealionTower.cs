using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class SealionTower : TowerBase
{
    // default rate of success of a fish catch attempt
    [Range(0f, 1f)]
    public float defaultCatchRate;

    // default rate of success of fish catch attempt for small, medium and large fish
    public float defaultFemaleCatchRate = 0.5F;
    public float defaultMaleCatchRate = 0.1F;

    // current rate of success of fish catch attempt for small, medium, and large fish
    [SerializeField] private float currentFemaleCatchRate;
    [SerializeField] private float currentMaleCatchRate;

    // fish that have been caught by this sealion
    private List<Fish> caughtFish;


    // material that will 
    public Material flashMaterial;

    // how many times the fish will flash in and out to show it is being caught
    public int numFlashesPerCatch;

    // Reference to the Tower Manager
    [SerializeField] private TowerManager theTowerManager;


    protected override void Awake()
    {
        // bas class awake
        base.Awake();

        defaultFemaleCatchRate = initializationValues.sealionFemaleCatchRate;
        defaultMaleCatchRate = initializationValues.sealionMaleCatchRate;

        // set current catch rates
        currentFemaleCatchRate = defaultCatchRate * defaultFemaleCatchRate;
        currentMaleCatchRate = defaultCatchRate * defaultMaleCatchRate;

        initializationValues.sealionPresent = 1;

        //Debug.Log("cScr=" + currentSmallCatchRate + "; cMcr=" + currentMediumCatchRate + "; cLcr=" + currentLargeCatchRate);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        TowerActive = true;
        paused = false;

        base.Start();

        caughtFish = new List<Fish>();

    }


    //Catch rate effect stuff
    public void AffectCatchRate(float smallEffect, float mediumEffect, float largeEffect, float length)
    {
        StartCoroutine(AffectCatchRateCoroutine(smallEffect, largeEffect, length));
    }

    protected override void ApplyTowerEffect()
    {

        Debug.Log("do Sealion Stuff");

        // get all fish that aren't already being caught
        Collider[] fishColliders = Physics.OverlapSphere(transform.position, GetEffectRadius(), LayerMask.GetMask(Layers.FISH_LAYER_NAME))
            .Where(fishCollider => !fishCollider.GetComponent<Fish>()?.beingCaught ?? true).ToArray();

        // select one of the fish
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

    private void TryCatchFish(Fish f)
    {
        StartCoroutine(TryCatchFishCoroutine(f));
    }

    private IEnumerator TryCatchFishCoroutine(Fish fish)
    {
        // how likely we are to catch fish is dependent on what size the fish is
        // determine that now
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

        // figure out whether the fish will be caught or not
        bool caught = Random.Range(0f, 1f) <= catchRate;

        // handle fish being caught
        if (caught)
        {
            // tell the fish that it is being caught
            fish.StartCatch();

            float timeToWait = (float)timePerApplyEffect / numFlashesPerCatch / 2f;

            // make the fish flash  for a bit
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

            // actually catch the fish
            fish.Catch();
            caughtFish.Add(fish);

        }
        // fish escaped -- just wait for end of action
        else
        {
            yield return new WaitForSeconds(timePerApplyEffect);
        }
    }

    private IEnumerator AffectCatchRateCoroutine(float femaleEffect, float maleEffect, float length)
    {
        currentFemaleCatchRate += femaleEffect;
        currentMaleCatchRate += maleEffect;

        yield return new WaitForSeconds(length);

        currentFemaleCatchRate -= femaleEffect;
        currentMaleCatchRate -= maleEffect;
    }



    //THIS IS ALL JUST HERE BECAUSE TOWERBASE REQUIRES IT, IT DOESN'T ACTUALLY DO ANYTHING
    protected override bool TowerPlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        
        int correctLayer = LayerMask.NameToLayer(Layers.TERRAIN_LAYER_NAME);

        // for placement to be valid, primary raycast must have hit a gameobject on the Terrain layer
        if (primaryHitInfo.collider && primaryHitInfo.collider.gameObject.layer == correctLayer)
        {
            // secondary raycasts must also hit gameobjects on the Terrain layer at approximately the same z-pos as the primary raycast
            return secondaryHitInfo.TrueForAll(hitInfo => hitInfo.collider &&
                                                            hitInfo.collider.gameObject.layer == correctLayer &&
                                                            Mathf.Abs(hitInfo.point.z - primaryHitInfo.point.z) < 1f);
        }
        

        // if one of these conditions was not met, return false
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
