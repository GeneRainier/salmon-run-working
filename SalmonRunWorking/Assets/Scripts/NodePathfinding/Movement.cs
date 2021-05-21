using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Destination destination;     //< The destination this object is moving towards
    public GenericSpawner spawner;      //< The spawner that created this object
    [SerializeField] private float speed;   //< The speed this object is moving

    // Update is called once per frame
    void Update()
    {
        if (destination != null)
        {
            // Move towards the destination at a constant speed
            transform.position = Vector3.MoveTowards(transform.position, destination.destinationPosition, speed * Time.deltaTime);
        }
        else if (spawner != null)
        {
            destination = spawner.initialDestinations[Random.Range(0, 5)];
        }
        else
        {
            spawner = FindObjectOfType<GenericSpawner>();
        }
    }

    // When we hit a trigger, check that it is our destination. If not, keep going to the destination
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Detected");
        if (other.gameObject.GetComponent<Destination>() == destination)
        {
            Debug.Log("Made it in the check");
            if (destination.finalDestination == true)
            {
                Destroy(this.gameObject);
            }
            else
            {
                destination = destination.destinations[Random.Range(0, 5)];
            }
        }
        else
        {
            return;
        }
    }
}
