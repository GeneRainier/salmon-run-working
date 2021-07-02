using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script for testing movement methods for Fish Pathfinding. This can be attached to moveing objects (fish) for testing prior to moving the 
 * body of the script over to the Fish script.
 * 
 * Authors: Benjamin Person
 */
public class Movement : MonoBehaviour
{
    public Destination destination = null;     //< The destination this object is moving towards
    public GenericSpawner spawner = null;      //< The spawner that created this object
    private Quaternion destinationDirection = Quaternion.identity;    //< The rotation the fish will begin to face over time
    [SerializeField] private float moveSpeed = 10.0f;   //< The speed this object is moving
    [SerializeField] private float turnSpeed = 45.0f;   //< The speed this object turns while moving

    [SerializeField] private List<Destination> path = null;    //< A list of destinations that the fish will follow
    private bool craftingPath = true;                   //< Whether or not this fish is still making a path to the end
    private int currentIndex = 0;                       //< The index of the current node this fish is moving to

    /*
     * Start is called prior to the first frame update
     */
    private void Start()
    {
        spawner = FindObjectOfType<GenericSpawner>();

        destination = spawner.initialDestinations[Random.Range(0, 5)];
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
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == destination.destinationPosition)
        {
            if (destination.finalDestination == true)
            {
                Destroy(this.gameObject);
            }
            else
            {
                currentIndex++;
                destination = path[currentIndex];

                Vector3 lookPosition = destination.destinationPosition - transform.position;
                lookPosition.y = 0.0f;
                destinationDirection = Quaternion.LookRotation(lookPosition);
            }
        }

        // Move towards the destination at a constant speed
        transform.position = Vector3.MoveTowards(transform.position, destination.destinationPosition, moveSpeed * Time.deltaTime);

        // Determine if we are already facing towards the destination
        float deltaAngle = Quaternion.Angle(transform.rotation, destinationDirection);

        // Exit early if no update required
        if (deltaAngle == 0.00F)
        { 
            return;
        }

        // Turn the fish to face the target at a constant speed
        transform.rotation = Quaternion.Slerp(transform.rotation, destinationDirection, turnSpeed * Time.deltaTime / deltaAngle);
    }
}
