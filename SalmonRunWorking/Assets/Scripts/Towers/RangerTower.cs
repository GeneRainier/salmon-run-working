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
     * Kill results in the Ranger destroying each Angler tower in range
     * Slowdown results in the Ranger adjusting the Angler catch rates based on the Ranger's personal policies
     */
    public enum Mode
    {
        Kill,
        Slowdown
    }

    public Mode mode;           //< Current mode that the ranger is in

    public Material hitLineMaterial;        //< Material for lines that show what angler the ranger is affecting

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
        // Creates a list of Angler colliders based on if the Anglers are in range (the Ranger's effect range dictates the size of the overlap sphere here)
        Collider[] anglerColliders = Physics.OverlapSphere(transform.position, GetEffectRadius(), LayerMask.GetMask(Layers.PLACED_OBJECTS))
            .Where((collider) => {
                return collider.GetComponentInChildren<AnglerTower>() != null && collider.GetComponentInChildren<AnglerTower>().TowerActive;
            }).ToArray();

        // Loop throught the Anglers we have, grab each script, and regulate them
        foreach (Collider anglerCollider in anglerColliders)
        {
            AnglerTower fishermanTower = anglerCollider.GetComponent<AnglerTower>();
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
        towerManager.AddTower(this);
        turnPlaced = GameManager.Instance.Turn;
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
        GameObject linePrefab = Instantiate(lineRendererPrefab, transform);
        LineRenderer lineRenderer = linePrefab.GetComponent<LineRenderer>();

        lineRenderer.material = hitLineMaterial;
        lineRenderer.enabled = true;

        towerEffectLineRenderers.Add(lineRenderer);

        Vector3 towerEffectPosition = anglerTower.transform.position;
        towerEffectPositions.Add(towerEffectPosition);

        // How we handle this depends on what mode the ranger is in
        switch (mode)
        {
            case Mode.Kill:
                // Make the fisherman inactive
                anglerTower.TowerActive = false;

                // Remove the fisherman tower
                if (anglerTower != null)
                {
                    Destroy(anglerTower.transform.root.gameObject);
                }
                break;
            case Mode.Slowdown:
                // Apply the affect to the angler
                anglerTower.AffectCatchRate(slowdownEffectSmall, slowdownEffectMedium, slowdownEffectLarge, timePerApplyEffect);
                yield return new WaitForSeconds((float) timePerApplyEffect / 2f);
                break;
        }

        // End the catch attempt line
        // NOTE: The yield return above suspends this coroutine for a few moments to allow the lines to render before this removal makes them disappear
        towerEffectPositions.Remove(towerEffectPosition);
        towerEffectLineRenderers.Remove(lineRenderer);
        Destroy(linePrefab);

        yield break;
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
            //endPos.y = startPos.y + 10.0f;      //< The "+ 10.0f" is a dirty fix to prevent the lines from running into the terrain when the Ranger is on a low elevation

            towerEffectLineRenderers[i].SetPositions(new Vector3[] { startPos, endPos });
        }
    }
}
