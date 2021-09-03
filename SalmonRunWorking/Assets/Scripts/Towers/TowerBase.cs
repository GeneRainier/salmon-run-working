using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Base class for towers
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
[RequireComponent(typeof(TowerRangeEffect))]
public abstract class TowerBase: MonoBehaviour, IDragAndDropObject, IPausable
{
    [SerializeField] protected int effectRadius;             //< Effect radius of the tower

    public ManagerIndex initializationValues;                //< The ManagerIndex with initialization values for a given tower
    public TowerManager towerManager;                        //< The Tower Manager with lists of all the towers in the scene

    public bool TowerActive { get; set; } = false;           //< Whether the tower is currently activated or not

    protected bool paused = true;                            //< Whether the tower is paused or not

    [SerializeField] protected int timePerApplyEffect;       //< Time between each application of tower effects

    private TowerRangeEffect rangeEffect;                    //< Tower range effect script

    public int turnPlaced = 0;                               //< The turn this tower was placed in the level

    #region Major Monobehaviour Functions

    /**
     * Awake is called after initialization of gameobjects prior to the start of the game. Handle initialization tasks common to all towers
     */
    protected virtual void Awake()
    {
        // Get initialization values and set this towers basic values
        initializationValues = FindObjectOfType<ManagerIndex>();
        // Get the Tower Manager
        towerManager = FindObjectOfType<TowerManager>();
        // Get component references
        rangeEffect = GetComponent<TowerRangeEffect>();
    }

    /**
     * Handle tasks before the first frame update common to all towers
     */
    protected virtual void Start()
    {
        StartCoroutine(StartTowerEffectLoop());
    }

    /**
     * Handle mouse over
     */
    private void OnMouseOver()
    {
        // Check for RMB
        if (Input.GetMouseButtonDown(1))
        {
            // If RMB down, delete the tower from the root
            Destroy(transform.root.gameObject);
        }
    }

    #endregion

    #region Getters and Setters

    /**
     * Get the radius of the area the tower effects
     * 
     * @return float The radius of the tower effect
     */
    public float GetEffectRadius()
    {
        return effectRadius;
    }

    #endregion

    #region Game Loop

    /**
     * Coroutine that will loop until the script is disabled or the object is deactivated and runs the tower's effect
     */
    protected IEnumerator StartTowerEffectLoop()
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForSeconds(timePerApplyEffect);
            if (TowerActive && !paused)
            {
                ApplyTowerEffect();
            }
        }
    }

    #endregion

    #region Pausable

    /**
     * Pause the tower
     */
    public void Pause()
    {
        paused = true;
    }

    /**
     * Resume the tower
     */
    public void Resume()
    {
        paused = false;
    }

    #endregion

    #region Placement

    /**
     * Apply effects of this tower
     */
    protected abstract void ApplyTowerEffect();

    /**
     * IDragAndDropInterface method implementation for PlacementValid
     * 
     * Determines if a location is valid for placement
     * 
     * This is the outwardly-facing function, which will take care of any
     * generic actions common to all towers and then rely on each class's
     * TowerPlacementValid implementation for actually determining if the
     * location is valid.
     * 
     * @param primaryHitInfo The raycast info from the main camera raycast
     * @param secondaryHitInfo The raycast info from the bounds of the tower
     * @return bool Whether or not the placement of the tower is valid
     */
    public bool PlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        bool placementValid = TowerPlacementValid(primaryHitInfo, secondaryHitInfo);

        if (placementValid)
        {
            rangeEffect.UpdateEffect(TowerRangeEffect.EffectState.Valid);
        }
        else
        {
            rangeEffect.UpdateEffect(TowerRangeEffect.EffectState.Invalid);
        }

        return placementValid;
    }

    /**
     * IDragAndDropInterface method implementation for Place
     * 
     * Places a drag and drop object into the environment
     * 
     * This is the outwardly-facing function, which will take care of any
     * generic actions common to all towers and then rely on each class's
     * PlaceTower implementation for actually placing the tower.
     * 
     * @param primaryHitInfo The raycast info from the main camera raycast
     * @param secondaryHitInfo The raycast info from the bounds of the tower
     */
    public void Place(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        ManagerIndex.MI.TimeManager.RegisterPausable(this);

        PlaceTower(primaryHitInfo, secondaryHitInfo);

        rangeEffect.UpdateEffect(TowerRangeEffect.EffectState.Off);

        TowerActive = true;
    }

    /**
     * Abstract function that each tower will implement
     * 
     * Determines whether a tower placement is valid
     * 
     * @param primaryHitInfo The raycast info from the main camera raycast
     * @param secondaryHitInfo The raycast info from the bounds of the tower
     */
    protected abstract bool TowerPlacementValid(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo);

    /**
     * Abstract function that each tower will implement
     *
     * Places a tower into the environment   
     * 
     * @param primaryHitInfo The raycast info from the main camera raycast
     * @param secondaryHitInfo The raycast info from the bounds of the tower
     */
    protected abstract void PlaceTower(RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo);

    #endregion
}
