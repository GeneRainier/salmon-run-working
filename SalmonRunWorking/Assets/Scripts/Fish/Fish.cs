using System.Collections;
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
    public float swimSpeed;

    // speed at which fish will rotate towards direction of motion
    public float rotateSpeed;

    public Destination destination;     //< The destination this object is moving towards

    private Quaternion destinationDirection;    //< The rotation the fish will begin to face over time

    [SerializeField] private List<Destination> path;    //< A list of destinations that the fish will follow

    private bool craftingPath = true;                   //< Whether or not this fish is still making a path to the end
    private int currentIndex = 0;                       //< The index of the current node this fish is moving to

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

    // Counter for attempts to pass the dam
    public int damPassCounter = 0;

    #region Major Monobehaviour Functions

    /**
     * Called when the fish is enabled
     */
    private void OnEnable()
    {
        // get refs
        rigid = GetComponent<Rigidbody>();
        fishAppearance = GetComponentInChildren<FishAppearance>();
        school = FindObjectOfType<FishSchool>();

        // Craft the path this fish will follow
        // While the fish is waiting to have its school assigned by the Fish School script, we wait

        destination = school.initialDestinations[Random.Range(0, 5)];
        Vector3 lookPosition = destination.destinationPosition - transform.position;
        destinationDirection = Quaternion.LookRotation(lookPosition);
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

        // set up initial energy
        currentEnergy = startingEnergy;
    }

    #endregion

    #region Movement

    /**
     * Make the fish swim by applying a force in a direction
     */
    public void Swim()
    {
        if (transform.position == destination.destinationPosition)
        {
            if (destination.finalDestination != true)
            {
                currentIndex++;
                destination = path[currentIndex];

                Vector3 lookPosition = destination.destinationPosition - transform.position;
                lookPosition.y = 0.0f;
                destinationDirection = Quaternion.LookRotation(lookPosition);
            }
        }

        // Move towards the destination at a constant speed
        transform.position = Vector3.MoveTowards(transform.position, destination.destinationPosition, swimSpeed * Time.deltaTime);

        // Determine if we are already facing towards the destination
        float deltaAngle = Quaternion.Angle(transform.rotation, destinationDirection);

        // Exit early if no update required
        if (deltaAngle == 0.00F)
        {
            return;
        }

        // Turn the fish to face the target at a constant speed
        transform.rotation = Quaternion.Slerp(transform.rotation, destinationDirection, rotateSpeed * Time.deltaTime / deltaAngle);
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
