﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class that controls a single fish
 */
[RequireComponent(typeof(Rigidbody))]
public class Fish : MonoBehaviour
{
    // amount of energy the fish has when they spawn
    public float startingEnergy;

    // baseline amount of energy that is expended by the fish if they are not moving
    public float baselineEnergyUsageRate;

    // rate at which the fish expends energy
    public float motionEnergyUsageRate;

    // maximum speed for fish movement
    public float maxSwimSpeed;

    // speed at which fish will rotate towards direction of motion
    public float rotateSpeed;

    // is this fish being caught?
    public bool beingCaught { get; private set; } = false;

    // fish school the fish belongs to
    private FishSchool school;

    // the fish's genome
    private FishGenome genome;

    // fish appearance controller
    private FishAppearance fishAppearance;

    // fish's rigidbody
    private Rigidbody rigid;

    // amount of energy the fish currently has
    private float currentEnergy;

    // is this fish currently stuck from moving on (because it failed to get across a dam or something like that)
    private bool stuck = false;

    // cached velocity & angular velocity for resume from pause
    private Vector3 cachedVelocity;
    private Vector3 cachedAngularVelocity;

    #region Major Monobehaviour Functions

    /**
     * Called when the fish is enabled
     */
    private void OnEnable()
    {
        // get refs
        rigid = GetComponent<Rigidbody>();
        fishAppearance = GetComponentInChildren<FishAppearance>();

        // set up initial energy
        currentEnergy = startingEnergy;
    }

    #endregion

    #region Movement

    /**
     * Make the fish swim by applying a force in a direction
     * 
     * @param vectorFieldInput Vector Direction in which vector field says the fish should go
     * @param randomInputMultiplier float Determines how impactful the random component of movement will be 
     * @param vectorFieldGridScale float Scale of the vector field grid, used in determining how much energy will be expended
     */
    public void Swim()
    {
<<<<<<< Updated upstream
        // generate some random movement to add to the force that will be applied
        Vector3 randomMovement = Random.insideUnitCircle;

        // make the random movement's magnitude a fraction of input from the vector field
        randomMovement *= (vectorFieldInput.magnitude * randomInputMultiplier);

        // combine the vector field and random components of movement
        Vector3 totalMovement = vectorFieldInput + randomMovement;

        // apply the force to the rigidbody
        rigid.AddForce(totalMovement);

        // make sure the fish cannot go over a certain top speed
        rigid.velocity = Vector3.ClampMagnitude(rigid.velocity, maxSwimSpeed);

        // attempt to rotate to face the direction of motion
        float angle = Mathf.Atan2(totalMovement.y, totalMovement.x) * Mathf.Rad2Deg;
        Debug.DrawRay(transform.GetChild(1).position, totalMovement);
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, rotateSpeed * Time.fixedDeltaTime);

        // use energy
        ExpendEnergy(totalMovement.magnitude, vectorFieldGridScale);
=======
        
>>>>>>> Stashed changes
    }

    /**
     * Returns true if the fish has used up all of its energy
     */
    public bool OutOfEnergy()
    {
        return currentEnergy <= 0;
    }

    /**
     * Use up energy for the fish
     */
    private void ExpendEnergy(float movementLength, float gridScale)
    {
        currentEnergy -= (baselineEnergyUsageRate + (movementLength / gridScale * motionEnergyUsageRate));
    }

    #endregion

    #region Getters and Setters

    /**
     * Get the school that the fish belongs to
     */
    public FishSchool GetSchool()
    {
        return school;
    }

    /**
     * Set the school that the fish belongs to
     */
    public void SetSchool(FishSchool school)
    {  
        this.school = school;
    }

    /**
     * Get this fish's genome
     */
    public FishGenome GetGenome()
    {
        return genome;
    }

    /**
     * Set the genome of the fish
     */
    public void SetGenome(FishGenome genome)
    {
        this.genome = genome;

        // if the appearance script can be found, set the appearance based on the genome
        if (fishAppearance)
        {
            fishAppearance.SetAppearance(genome);
        }
    }

    /**
     * Set whether this fish is stuck from moving on past obstacles in the level
     */
    public void SetStuck(bool stuck)
    {
        this.stuck = stuck;
    }

    /**
     * Get whether the fish is stuck currently
     */
    public bool IsStuck()
    {
        return stuck;
    }

    #endregion

    #region Fish Management

    /**
     * Handle fish starting to be caught
     */
    public void StartCatch()
    {
        beingCaught = true;
    }

    /**
     * Handle this fish being caught
     */
    public void Catch()
    {
        // remove the fish from the school
        school.FishKilled(this);

        DeactivateFish();
    }

    /**
     * Handle the fish sucessfully reaching a spawning grounds
     */
    public void ReachSpawningGrounds()
    {
        // tell the school that this fish succeeded
        school.FishSucceeded(this);

        DeactivateFish();
    }

    /**
     * Deactivate the fish
     */
    private void DeactivateFish()
    {
        // disable the whole fish
        // have to go to the root because the fish component may not be on the outermost gameobject
        // and therefore disabling it might not disable the renderer or other fish parts
        transform.root.gameObject.SetActive(false);
    }

    #endregion

    #region Pause and Resume

    /**
     * Pause fish motion, saving rigidbody data so it can be restored later
     */
    public void CacheAndPauseMotion()
    {
        cachedVelocity = rigid.velocity;
        cachedAngularVelocity = rigid.angularVelocity;

        rigid.isKinematic = true;
    }

    /**
     * Resume fish motion from paused state, restoring cached rigidbody data
     */
    public void RestoreAndResumeMotion()
    {
        rigid.isKinematic = false;

        rigid.velocity = cachedVelocity;
        rigid.angularVelocity = cachedAngularVelocity;

        rigid.WakeUp();
    }

    #endregion
}
