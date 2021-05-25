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

    // Update is called once per frame
    void Update()
    {
        if (destination != null)
        {
            // Move towards the destination at a constant speed
            transform.position = Vector3.MoveTowards(transform.position, destination.destinationPosition, moveSpeed * Time.deltaTime);
            // Turn the fish to face the target at a constant speed
            transform.rotation = Quaternion.RotateTowards(transform.rotation, destinationDirection, turnSpeed * Time.deltaTime);
        }
        else if (spawner != null)
        {
            destination = spawner.initialDestinations[Random.Range(0, 5)];
            Vector3 lookPosition = destination.destinationPosition - transform.position;
            destinationDirection = Quaternion.LookRotation(lookPosition);
            float deltaAngle = Quaternion.Angle(transform.rotation, destinationDirection);

            if (deltaAngle == 0.00F)
            { // Exit early if no update required
                return;
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, destinationDirection, turnSpeed * Time.deltaTime / deltaAngle);
        }
        else
        {
            spawner = FindObjectOfType<GenericSpawner>();
        }
    }

    // When we hit a trigger, check that it is our destination. If not, keep going to the destination
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Destination>() == destination)
        {
            if (destination.finalDestination == true)
            {
                Destroy(this.gameObject);
            }
            else
            {
                destination = destination.destinations[Random.Range(0, 5)];
                Vector3 lookPosition = destination.destinationPosition - transform.position;
                lookPosition.y = 0.0f;

                destinationDirection = Quaternion.LookRotation(lookPosition);
            }
        }
        else
        {
            return;
        }
    }
}
