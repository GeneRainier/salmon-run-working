using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class that controls a single fish
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
[RequireComponent(typeof(Rigidbody))]
public class Fish : MonoBehaviour
{
    public float startingEnergy;        //< Amount of energy the fish has when they spawn

    public float baselineEnergyUsageRate;   //< Baseline amount of energy that is expended by the fish if they are not moving

    public float motionEnergyUsageRate;     //< Rate at which the fish expends energy

    public float swimSpeed;                 //< Maximum speed for fish movement

    public float rotateSpeed;               //< Speed at which fish will rotate towards direction of motion

    public Destination destination;     //< The destination this object is moving towards

    private Quaternion destinationDirection;    //< The rotation the fish will begin to face over time

    //private float destinationDistance;          //< The distance between the initial location of the fish and its current destination
    //private float lerpTime;                     //< The current percentage through the lerp this fish is
    //private float timer = 0.0f;                        //< The current time this fish has been travelling to a destination
    //private float travelTime = 0.0f;                   //< The amount of time it will take to lerp to the current destination

    [SerializeField] private List<Destination> path = null;    //< A list of destinations that the fish will follow

    private bool craftingPath = true;                   //< Whether or not this fish is still making a path to the end
    private int currentIndex = 0;                       //< The index of the current node this fish is moving to

    public bool beingCaught { get; private set; } = false;  //< Is this fish being caught?

    private FishSchool school;          //< Fish school the fish belongs to

    private FishGenome genome;          //< The fish's genome

    private FishAppearance fishAppearance;      //< Fish appearance controller

    private Rigidbody rigid;            //< Fish's rigidbody

    private float currentEnergy;        //< Amount of energy the fish currently has

    private bool stuck = false;         //< Is this fish currently stuck from moving on (because it failed to get across a dam or something like that)

    // Cached velocity & angular velocity for resume from pause
    private Vector3 cachedVelocity;
    private Vector3 cachedAngularVelocity;

    public int damPassCounter = 0;      //< Counter for attempts to pass the dam

    public ParticleSystem waterSplash = null;     //< The water splash effect that will play when this fish is caught

    #region Major Monobehaviour Functions

    /**
     * Called when the fish is enabled
     */
    private void OnEnable()
    {
        // Get References
        rigid = GetComponent<Rigidbody>();
        fishAppearance = GetComponentInChildren<FishAppearance>();
        school = FindObjectOfType<FishSchool>();

        // Craft the path this fish will follow
        // While the fish is waiting to have its school assigned by the Fish School script, we wait

        destination = school.initialDestinations[Random.Range(0, 5)];
        Vector3 lookPosition = destination.destinationPosition - transform.position;
        destinationDirection = Quaternion.LookRotation(lookPosition);

        //destinationDistance = Vector3.Distance(transform.position, destination.destinationPosition);
        //travelTime = destinationDistance / swimSpeed;

        path.Add(destination);
        Destination currentNode = destination;

        while (craftingPath)
        {
            Destination newNode = currentNode.destinations[Random.Range(0, 5)];
            if (newNode.finalDestination == true)
            {
                path.Add(newNode);
                craftingPath = false;
            }
            else
            {
                path.Add(newNode);
                currentNode = newNode;
            }
        }

        // Set up initial energy
        currentEnergy = startingEnergy;
    }

    /* 
     * IDEA NOTE: For Path smoothing: The fish simply move FORWARD. When they change destination nodes to the next one in the path
     * the turning is all that will adjust. This makes it so the movement is kept simple and low cost, but the turning imitates a
     * smooth path. No need for a complication Bezier calculation or A* path smoothing which would be longer code
     */

    /*
     * Update is called every frame update. This is mainly used for fish movement and energy degradation.
     */
    private void Update()
    {
        // If the game is paused, we do not want to handle any of this movement
        // NOTE: If more elements are added to this Update function, they may need to be placed prior to this check to 
        //       ensure that pausing does not break vital behaviors
        if (Time.timeScale == 0)
        {
            return;
        }

        // If we are close to the current destination, then begin moving towards the next one in the path
        if (Vector3.Distance(transform.position, destination.destinationPosition) <= 10.0f)
        {
            if (destination.finalDestination != true)
            {
                currentIndex++;
                destination = path[currentIndex];
            }
        }

        Vector3 lookPosition = destination.destinationPosition - transform.position;
        lookPosition.y = 0.0f;
        destinationDirection = Quaternion.LookRotation(lookPosition);

        // Move towards the destination at a constant speed
        transform.position += transform.forward * swimSpeed * Time.deltaTime;
        // Determine if we are already facing towards the destination
        float deltaAngle = Quaternion.Angle(transform.rotation, destinationDirection);

        // Exit early if no update required
        if (deltaAngle == 0.00F)
        {
            return;
        }

        // Turn the fish to face the target at a constant speed
        transform.rotation = Quaternion.RotateTowards(transform.rotation, destinationDirection, rotateSpeed * Time.deltaTime);
    }

    #endregion

    #region Movement

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
     * 
     * @param school The FishSchool this fish is from
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
     * 
     * @param genome The genome belonging to this fish
     */
    public void SetGenome(FishGenome genome)
    {
        this.genome = genome;

        // If the appearance script can be found, set the appearance based on the genome
        if (fishAppearance)
        {
            fishAppearance.SetAppearance(genome);
        }
    }

    /**
     * Set whether this fish is stuck from moving on past obstacles in the level
     * 
     * @param stuck Is the fish currently stuck or not
     */
    public void SetStuck(bool stuck)
    {
        this.stuck = stuck;
        school.FishKilled(this);

        DeactivateFish();
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
        // Remove the fish from the school
        school.FishKilled(this);

        DeactivateFish();
    }

    public void Killed()
    {
        school.FishKilled(this);

        DeactivateFish();
    }

    /**
     * Handle the fish sucessfully reaching a spawning grounds
     */
    public void ReachSpawningGrounds()
    {
        // Tell the school that this fish succeeded
        school.FishSucceeded(this);

        DeactivateFish();
    }

    /**
     * Deactivate the fish
     */
    private void DeactivateFish()
    {
        // Disable the whole fish
        // Have to go to the root because the fish component may not be on the outermost gameobject
        // and therefore disabling it might not disable the renderer or other fish parts
        transform.root.gameObject.SetActive(false);
    }

    #endregion
}
