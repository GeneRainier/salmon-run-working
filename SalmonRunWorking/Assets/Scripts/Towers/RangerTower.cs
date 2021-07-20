using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Class that describes a ranger
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class RangerTower : TowerBase
{
    /**
     * Modes that the ranger can be in
     */
    public enum Mode
    {
        Kill,
        Slowdown
    }

    public Mode mode;           //< Current mode that the ranger is in

    // Materials for lines that show what angler the ranger is affecting
    public Material hitLineMaterial;
    public Material missLineMaterial;

    public Material flashMaterial;      //< Material for making an angler flash to show it is being affected

    public GameObject lineRendererPrefab;   //< Prefab for line renderer

    [Range(-1f, 1f)]
    public float slowdownEffectSmall;   //< Float representing how much of an effect the ranger should have if it regulates an angler in slowdown mode for small fish

    [Range(-1f, 1f)]
    public float slowdownEffectMedium;  //< Float representing how much of an effect the ranger should have if it regulates an angler in slowdown mode for medium fish

    [Range(-1f, 1f)]
    public float slowdownEffectLarge;   //< Float representing how much of an effect the ranger should have if it regulates an angler in slowdown mode for large fish

    [Range(0f, 1f)]
    public float regulationSuccessRate; //< Float representing how likely the ranger is to successfully regulate an angler
    // Initialized in Unity
    // Project -> Assets -> Prefabs -> Towers -> RangerTower
    // then look at Hierarchy
    // Hierarchy -> RangerTower -> TokenBase

    public int numFlashesPerCatch;      //< How many times the fish will flash in and out to show it is being caught

    private List<LineRenderer> towerEffectLineRenderers = new List<LineRenderer>();     //< List of linerenderers used to show effects

    private List<Vector3> towerEffectPositions = new List<Vector3>();       //< List of linerenderer pos

    /**
     * Start is called before the first frame update
     */
    protected override void Start()
    {
        effectRadius = initializationValues.rangerRadius;
        slowdownEffectSmall = initializationValues.rangerSmallModifier;
        slowdownEffectMedium = initializationValues.rangerMediumModifier;
        slowdownEffectLarge = initializationValues.rangerLargeModifier;
        regulationSuccessRate = initializationValues.rangerSuccessRate;
        base.Start();
    }

    /**
     * Update is called once per frame
     */
    void Update()
    {
        // Update fishermen line positions
        SetLinePositions();
    }

    /**
     * Apply the effects of the ranger tower
     */
    protected override void ApplyTowerEffect()
    {
        Collider[] anglerColliders = Physics.OverlapSphere(transform.position, GetEffectRadius(), LayerMask.GetMask(Layers.PLACED_OBJECTS))
            .Where((collider) => {
                return collider.GetComponentInChildren<AnglerTower>() != null && collider.GetComponentInChildren<AnglerTower>().TowerActive;
            }).ToArray();

        
        foreach (Collider anglerCollider in anglerColliders)
        {
            AnglerTower fishermanTower = anglerCollider.GetComponent<AnglerTower>();

            if (fishermanTower.anglerCounted == false)
            {
                if (fishermanTower.transform.position.x < 1115)
                {
                    initializationValues.lowerManagedAnglerCount += 1;
                    fishermanTower.anglerCounted = true;
                }
                else
                {
                    initializationValues.upperManagedAnglerCount += 1;
                    fishermanTower.anglerCounted = true;
                }
            }

            RegulateFisherman(fishermanTower);
        }
    }

    /**
     * Position the ranger at the correct location using the information from a raycast
     * 
     * @param primaryHitInfo RaycastHit The results of the primary raycast that was done
     * @param secondaryHitInfo List RaycastHit The results of any secondary raycasts that were done
     */
    protected override void PlaceTower(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        transform.parent.position = primaryHitInfo.point;
        initializationValues.rangerCount += 1;
    }

    /**
     * Determine whether a ranger could be placed at the location specified by a raycast
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
            // Secondary raycasts must also hit gameobjects on the Terrain layer at approximately the same z-pos as the primary raycast
            return secondaryHitInfo.TrueForAll((hitInfo) =>
            {
                return hitInfo.collider &&
                       hitInfo.collider.gameObject.layer == correctLayer &&
                       Mathf.Abs(hitInfo.point.y - primaryHitInfo.point.y) < 0.1f;
            });
        }

        // If one of these conditions was not met, return false
        return false;
    }

    /**
     * Attempt to regulate an angler
     * 
     * @param fishermanTower The tower the ranger is regulating
     */
    private void RegulateFisherman(AnglerTower anglerTower)
    {
        StartCoroutine(RegulateFishermanCoroutine(anglerTower));
    }

    /**
     * Display attempt to catch fish
     * 
     * @param fishermanTower The tower the ranger is regulating
     */
    private IEnumerator RegulateFishermanCoroutine(AnglerTower anglerTower)
    {
        // Figure out whether the fisherman will be stopped or not
        bool caught = Random.Range(0f, 1f) <= regulationSuccessRate;

        GameObject g = Instantiate(lineRendererPrefab, transform);
        LineRenderer lr = g.GetComponent<LineRenderer>();

        lr.material = caught ? hitLineMaterial : missLineMaterial;

        lr.enabled = true;

        towerEffectLineRenderers.Add(lr);

        Vector3 towerEffectPosition = anglerTower.transform.position;
        towerEffectPositions.Add(towerEffectPosition);

        // Handle fish being caught
        if (caught)
        {
            // Want this variable so we can make the fisherman flash, regardless of what mode we're in
            MeshRenderer fishermanTowerRenderer = anglerTower.flashRenderer;

            // How we handle this depends on what mode the ranger is in
            switch (mode)
            {
                case Mode.Kill:
                    // Make the fisherman inactive
                    anglerTower.TowerActive = false;

                    // Make the fisherman flash for a bit
                    for (int i = 0; i < numFlashesPerCatch; i++)
                    {
                        Material oldMaterial = null;
                        if (fishermanTowerRenderer != null)
                        {
                             oldMaterial = fishermanTowerRenderer.material;
                            fishermanTowerRenderer.material = flashMaterial;
                        }
                        yield return new WaitForSeconds((float)timePerApplyEffect / numFlashesPerCatch / 2f);

                        if (fishermanTowerRenderer != null)
                        {
                            Destroy(fishermanTowerRenderer.material);
                            fishermanTowerRenderer.material = oldMaterial;
                        }
                        yield return new WaitForSeconds((float)timePerApplyEffect / numFlashesPerCatch / 2f);
                    }

                    // Remove the fisherman tower
                    if (anglerTower != null)
                    {
                        Destroy(anglerTower.transform.root.gameObject);
                    }
                    break;
                case Mode.Slowdown:
                    // Apply the affect to the angler
                    anglerTower.AffectCatchRate(slowdownEffectSmall, slowdownEffectMedium, slowdownEffectLarge, timePerApplyEffect);

                    // Make the fisherman flash  for a bit
                    for (int i = 0; i < numFlashesPerCatch; i++)
                    {
                        Material oldMaterial = null;
                        if (fishermanTowerRenderer != null)
                        {
                            oldMaterial = fishermanTowerRenderer.material;
                            fishermanTowerRenderer.material = flashMaterial;
                        }
                        yield return new WaitForSeconds((float)timePerApplyEffect / numFlashesPerCatch / 2f);
                        if (fishermanTowerRenderer != null)
                        {
                            Destroy(fishermanTowerRenderer.material);
                            fishermanTowerRenderer.material = oldMaterial;
                        }
                        yield return new WaitForSeconds((float)timePerApplyEffect / numFlashesPerCatch / 2f);
                    }
                    break;
            }
        }
        // Fish escaped -- just wait for end of action
        else
        {
            yield return new WaitForSeconds(timePerApplyEffect);
        }

        // End the catch attempt line
        towerEffectPositions.Remove(towerEffectPosition);
        towerEffectLineRenderers.Remove(lr);
        Destroy(g);
        
    }

    /**
     * Set line position for a fisherman we are targeting
     */
    private void SetLinePositions()
    {
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        for (int i = 0; i < towerEffectLineRenderers.Count; i++)
        {
            Vector3 endPos = towerEffectPositions[i];
            endPos.y = startPos.y;

            towerEffectLineRenderers[i].SetPositions(new Vector3[] { startPos, endPos });
        }
    }
}
