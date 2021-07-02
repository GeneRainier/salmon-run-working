using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The class which describes the abilities of a tower regarding range and appearance
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
[RequireComponent(typeof(TowerBase))]
public class TowerRangeEffect : MonoBehaviour
{
    public static TowerRangeEffect currentlySelectedRangeEffect { get; private set; }       //< Currently selected tower effect

    /**
     * Enum representing states the range effect can be in
     */
    public enum EffectState
    {
        Off,
        Neutral,
        Invalid,
        Valid
    }

    // Materials to be applied for each non-off state
    public Material neutralMaterial = null;
    public Material invalidMaterial = null;
    public Material validMaterial = null;

    [SerializeField]
    private GameObject rangeVisualizationObj = null;       //< Object that actually holds the effect object/sprite

    public EffectState State { get; private set; } = EffectState.Off;       //< The current state of this range effect

    private TowerBase tower;        //< The actual tower component this effect is for

    /**
     * Start is called before the first frame update
     */
    private void Start()
    {
        // Get component reference
        tower = GetComponent<TowerBase>();

        UpdateRadius();
        UpdateEffect(EffectState.Off);
    }

    /**
     * Handle left click on the tower range effect
     */
    private void OnMouseDown()
    {
        // Make sure the tower is on
        if (tower.isActiveAndEnabled)
        {
            // If a tower effect is already on and it's not this one, turn it off
            if (currentlySelectedRangeEffect != null && currentlySelectedRangeEffect != this)
            {
                currentlySelectedRangeEffect.UpdateEffect(EffectState.Off);
            }

            // Update currently selected range effect
            currentlySelectedRangeEffect = this;

            // Toggle the selected range effect
            UpdateEffect(State == EffectState.Off ? EffectState.Neutral : EffectState.Off);
        }
    }

    /**
     * Update the tower range effect's state
     * 
     * @param effectState The state a tower is currently in
     */
    public void UpdateEffect(EffectState effectState)
    {
        // Update the state
        State = effectState;

        // Special case for turning it off
        if (State == EffectState.Off)
        {
            // Turn off the range visualizer object
            rangeVisualizationObj.SetActive(false);
        }
        else
        {
            // Make sure the range visualizer object is on
            rangeVisualizationObj.SetActive(true);

            // Get the correct material to apply using switch statement
            Material m = neutralMaterial;
            switch (State)
            {

                case EffectState.Neutral:
                    m = neutralMaterial;
                    break;
                case EffectState.Invalid:
                    m = invalidMaterial;
                    break;
                case EffectState.Valid:
                    m = validMaterial;
                    break;
            }

            // Get the visualizer's mesh renderer
            MeshRenderer mr = rangeVisualizationObj.GetComponent<MeshRenderer>();

            // Destroy the old material instance to prevent memory leak
            Destroy(mr.material);

            // Set the material to the correct one
            mr.material = m;       
         }

    }

    /**
     * Update the radius of the effect
     */
    public void UpdateRadius()
    {
        float radius = tower.GetEffectRadius();
        rangeVisualizationObj.transform.parent = null;
        rangeVisualizationObj.transform.localScale = new Vector3(radius * 2, rangeVisualizationObj.transform.localScale.y, radius * 2);
        rangeVisualizationObj.transform.parent = transform;
    }
}
