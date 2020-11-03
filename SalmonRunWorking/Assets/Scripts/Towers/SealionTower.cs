﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class SealionTower : TowerBase
{
    // mesh renderer that will flash when the fisherman tower is affected by a ranger or another tower
    public MeshRenderer flashRenderer;

    // materials for line renderer that indicates whether a fish has been hit or missed
    public Material hitLineMaterial;
    public Material missLineMaterial;

    // material that will 
    public Material flashMaterial;

    // default rate of success of a fish catch attempt
    [Range(0f, 1f)]
    public float defaultCatchRate;
    // initalized in Unity interface:
    //Project -> Assets -> Prefabs -> Towers -> FishermanTower
    // then look at Hierarchy
    //Hierarchy -> FishermanTower -> TokenBase  set to 1 on 2020_510

    // how many times the fish will flash in and out to show it is being caught
    public int numFlashesPerCatch;

    // default rate of success of fish catch attempt for small, medium and large fish
    public float defaultSmallCatchRate = 0.0F;
    public float defaultMediumCatchRate = 0.9F;
    public float defaultLargeCatchRate = 0.0F;

    // current rate of success of fish catch attempt for small, medium, and large fish
    private float currentSmallCatchRate;
    private float currentMediumCatchRate;
    private float currentLargeCatchRate;

    // fish that have been caught by this fisherman
    private List<Fish> caughtFish = new List<Fish>();

    // fish that the catch attempt line is pointing at
    private Fish catchAttemptFish;

    // LineRenderer component used to display a catch or catch attempt
    private LineRenderer catchAttemptLine;

    /**
     * Start is called on object initialization
     */
    protected override void Awake()
    {
        // bas class awake
        base.Awake();

        // set current catch rates
        currentSmallCatchRate = defaultCatchRate * defaultSmallCatchRate;
        currentMediumCatchRate = defaultCatchRate * defaultMediumCatchRate;
        currentLargeCatchRate = defaultCatchRate * defaultLargeCatchRate;
    }

    /**
     * Called before the first frame update
     */
    protected override void Start()
    {
        base.Start();

        catchAttemptLine = GetComponent<LineRenderer>();
        catchAttemptLine.enabled = false;
    }

    /**
     * Update is called once per frame
     */
    private void Update()
    {
        // update fish line position
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
     */
    protected override bool TowerPlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        int correctLayer = LayerMask.NameToLayer(Layers.TERRAIN_LAYER_NAME);

        // for placement to be valid, primary raycast must have hit a gameobject on the Terrain layer
        if (primaryHitInfo.collider && primaryHitInfo.collider.gameObject.layer == correctLayer)
        {
            // secondary raycasts must also hit gameobjects on the Terrain layer at approximately the same z-pos as the primary raycast
            return secondaryHitInfo.TrueForAll((hitInfo) => hitInfo.collider &&
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
    }

    /**
     * Apply the effects of the fisherman tower
     */
    protected override void ApplyTowerEffect()
    {
        // get all fish that aren't already being caught
        Collider[] fishColliders = Physics.OverlapSphere(transform.position, GetEffectRadius(), LayerMask.GetMask(Layers.FISH_LAYER_NAME))
            .Where(fishCollider => {
                Fish f = fishCollider.GetComponent<Fish>();

                // throw a warning if something on the fish layer doesn't have a Fish component
                if (!f)
                {
                    Debug.LogWarning("Something on the fish layer does not have a Fish component!");
                }

                return f && !f.beingCaught;
            }).ToArray();

        // select one of the fish
        if (fishColliders.Length <= 0) return;
        {
            Fish f = fishColliders[Random.Range(0, fishColliders.Length)].GetComponent<Fish>();

            if (f)
            {
                transform.parent.LookAt(f.transform, Vector3.back);

                TryCatchFish(f);

            }
            else
            {
                Debug.LogError("Error with selecting random fish to catch -- should not happen!");
            }
        }

    }

    /**
     * Attempt to catch a fish
     */
    private void TryCatchFish(Fish f)
    {
        StartCoroutine(TryCatchFishCoroutine(f));
    }

    /**
     * Display attempt to catch fish
     */
    private IEnumerator TryCatchFishCoroutine(Fish fish)
    {
        // how likely we are to catch fish is dependent on what size the fish is
        // determine that now
        float catchRate;
        float weight;
        FishGenePair sizeGenePair = fish.GetGenome()[FishGenome.GeneType.Size];
        switch (sizeGenePair.momGene)
        {
            case FishGenome.b when sizeGenePair.dadGene == FishGenome.b:
                catchRate = currentSmallCatchRate;
                weight = 4f;
                break;
            case FishGenome.B when sizeGenePair.dadGene == FishGenome.B:
                catchRate = currentMediumCatchRate;
                weight = 15f;
                break;
            default:
                catchRate = currentLargeCatchRate;
                weight = 9f;
                break;
        }

        // figure out whether the fish will be caught or not
        bool caught = Random.Range(0f, 1f) <= catchRate;

        // do setup for catch attempt line visualizer
        catchAttemptFish = fish;

        Destroy(catchAttemptLine.material);
        catchAttemptLine.material = caught ? hitLineMaterial : missLineMaterial;

        catchAttemptLine.enabled = true;

        // handle fish being caught
        if (caught)
        {
            // tell the fish that it is being caught
            fish.StartCatch();

            // make the fish flash  for a bit
            SkinnedMeshRenderer fishRenderer = fish.GetComponentInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < numFlashesPerCatch; i++)
            {
                Material oldMaterial = fishRenderer.material;
                fishRenderer.material = flashMaterial;
                yield return new WaitForSeconds((float)timePerApplyEffect / numFlashesPerCatch / 2f);
                Destroy(fishRenderer.material);
                fishRenderer.material = oldMaterial;
                yield return new WaitForSeconds((float)timePerApplyEffect / numFlashesPerCatch / 2f);
            }

            // actually catch the fish
            fish.Catch();
            caughtFish.Add(fish);
            
            // Add Appropriate Funds to Bank
            //ManagerIndex.MI.MoneyManager.AddCatch(weight);
        }
        // fish escaped -- just wait for end of action
        else
        {
            yield return new WaitForSeconds(timePerApplyEffect);
        }

        // end the catch attempt line
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
        currentSmallCatchRate += smallEffect;
        currentMediumCatchRate += mediumEffect;
        currentLargeCatchRate += largeEffect;

        yield return new WaitForSeconds(length);

        currentSmallCatchRate -= smallEffect;
        currentMediumCatchRate -= mediumEffect;
        currentLargeCatchRate -= largeEffect;
    }

    /**
     * Set line position for a fish
     */
    private void SetLinePos()
    {
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 40);
        Vector3 fishPos = catchAttemptFish.transform.position;
        fishPos.z = startPos.z;

        catchAttemptLine.SetPositions(new Vector3[]{ startPos, fishPos});
    }
}
