using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script to handle selecting objects in the level such as towers
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class ObjectSelectManager : MonoBehaviour
{
    private TowerBase selectedTower;                //< Tower that is currently selected
    private TowerRangeEffect selectedRangeEffect;   //< The selected towers range

    /*
     * Start is called before the first frame update
     */
    void Start()
    {
        
    }

    /*
     * Update is called once per frame
     */
    void Update()
    {
        CheckInput();
    }

    /*
     * Checks every frame for if a mouse input has been provided
     */
    private void CheckInput()
    {
        // Only check when mouse is clicked
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            // Raycast into scene from mouse pos to determine what we clicked on
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if we actually hit an object that we care about
            if (Physics.Raycast(ray, out hit) && hit.collider)
            {
                CheckForTower(hit.collider.gameObject, Input.GetMouseButtonDown(0));
                CheckForFish(hit.collider.gameObject);
            }
        }
    }

    /**
     * Check for a tower and apply the effects that selecting a tower should trigger
     * 
     * @param hitObject GameObject The object that the raycast hit
     * @param isLeftMouse Bool dictating if the left mouse button was clicked or not
     */
    private void CheckForTower(GameObject hitObject, bool isLeftMouse)
    {
        // Flag to tell us later if we actually hit a tower
        bool hitTower = false;

        // Attempt to get the TowerBase component from the object if it has one
        TowerBase towerTemp = hitObject.GetComponent<TowerBase>();

        // Make sure that we found a TowerBase component that is active
        if (towerTemp != null && towerTemp.isActiveAndEnabled)
        {
            // Set flag so we know later that we hit a tower
            hitTower = true;

            // Turn off previous effect if there was one
            if (selectedRangeEffect != null && selectedRangeEffect != hitObject.GetComponent<TowerRangeEffect>())
            {
                // Turn the neutral range effect off
                selectedRangeEffect.UpdateEffect(TowerRangeEffect.EffectState.Off);
            }

            // Set as selected tower
            selectedTower = towerTemp;

            // Get the range effect component and update it to show the neutral tower range effect
            selectedRangeEffect = hitObject.GetComponent<TowerRangeEffect>();
            selectedRangeEffect.UpdateEffect(selectedRangeEffect.State == TowerRangeEffect.EffectState.Off ? TowerRangeEffect.EffectState.Neutral : TowerRangeEffect.EffectState.Off);
        }

        // If we did not hit a tower, remove selected tower
        if (!hitTower)
        {
            // Remove reference
            if (selectedTower != null)
            {
                selectedTower = null;
            }

            // Remove references to other components
            if (selectedRangeEffect != null)
            {
                // Turn the neutral range effect off
                selectedRangeEffect.UpdateEffect(TowerRangeEffect.EffectState.Off);
                selectedRangeEffect = null;
            }
        }
    }

    /**
     * Check for a fish and apply the effects that selecting a fish should trigger
     * 
     * @param hitObject GameObject The object that the raycast hit
     */
    private void CheckForFish(GameObject hitObject)
    {
        Fish fish = hitObject.GetComponentInChildren<Fish>();

        if (fish != null && fish.isActiveAndEnabled)
        {
            Debug.Log(fish.GetGenome()[FishGenome.GeneType.Sex].momGene);
            Debug.Log(fish.GetGenome()[FishGenome.GeneType.Sex].dadGene);
            Debug.Log(fish.GetGenome()[FishGenome.GeneType.Size].momGene);
            Debug.Log(fish.GetGenome()[FishGenome.GeneType.Size].dadGene);
        }
    }
}
