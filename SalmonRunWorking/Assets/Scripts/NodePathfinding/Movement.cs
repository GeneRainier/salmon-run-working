using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Destination destination;     //< The destination this object is moving towards
    public GenericSpawner spawner;      //< The spawner that created this object
    Quaternion destinationDirection;    //< The rotation the fish will begin to face over time
    [SerializeField] private float moveSpeed;   //< The speed this object is moving
    [SerializeField] private float turnSpeed;   //< The speed this object turns while moving

    [SerializeField] private List<Destination> path;    //< A list of destinations that the fish will follow
    private bool craftingPath = true;                   //< Whether or not this fish is still making a path to the end
    private int currentIndex = 0;                        //< The index of the current node this fish is moving to

    private int nodeLayerMask = 1 << 12;     //< The layer that the nodes are located on

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
        if (destination != null)
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

    // Only necessary for the Raycast Method
    //private void FixedUpdate()
    //{
    //    // A recurring raycast check for this destination
    //    RaycastHit hit;

    //    // Does this raycast hit any of the fish?
    //    if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, nodeLayerMask))
    //    {
    //        // Change the fishes destination
    //        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down), Color.yellow);
    //        if (hit.collider.gameObject.GetComponent<Destination>().finalDestination == true)
    //        {
    //            Destroy(this.gameObject);
    //        }
    //        else
    //        {
    //            destination = destination.destinations[Random.Range(0, 5)];
    //            Vector3 lookPosition = destination.destinationPosition - transform.position;
    //            lookPosition.y = 0.0f;

    //            destinationDirection = Quaternion.LookRotation(lookPosition);
    //        }
    //    }
    //    else
    //    {
    //        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down), Color.yellow);
    //    }
    //}

    // When we hit a trigger, check that it is our destination. If not, keep going to the destination
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.GetComponent<Destination>() == destination)
    //    {
    //        if (destination.finalDestination == true)
    //        {
    //            Destroy(this.gameObject);
    //        }
    //        else
    //        {
    //            destination = destination.destinations[Random.Range(0, 5)];
    //            Vector3 lookPosition = destination.destinationPosition - transform.position;
    //            lookPosition.y = 0.0f;

    //            destinationDirection = Quaternion.LookRotation(lookPosition);
    //        }
    //    }
    //    else
    //    {
    //        return;
    //    }
    //}
}
