using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogShift : MonoBehaviour
{
    //[SerializeField] private Vector3 originalPosition;       //< The position the fog is first located
    //[SerializeField] private Vector3 middlePosition;         //< The position the fog is located during Phase 2 of the level
    //[SerializeField] private Vector3 rightPosition;          //< The position the fog is located during Phase 3 of the level
    //[SerializeField] private Vector3 currentTarget;          //< The current target position of the fog

    private float timeElapsed = 0.0f;       //< The amount of time that has passed during the current lerp
    private float totalMoveTime = 3.0f;     //< The amount of time we want the fog lerp to last

    [SerializeField] private ParticleSystem fog;    //< The particle system running our fog effect

    private void Start()
    {
        fog.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            fog.Stop();
        }

        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    StopCoroutine("MoveOverSeconds");
        //    currentTarget = rightPosition;
        //    StartCoroutine("MoveOverSeconds");
        //}
        //else if (Input.GetKeyDown(KeyCode.K))
        //{
        //    StopCoroutine("MoveOverSeconds");
        //    currentTarget = middlePosition;
        //    StartCoroutine("MoveOverSeconds");
        //}
        //else if (Input.GetKeyDown(KeyCode.J))
        //{
        //    StopCoroutine("MoveOverSeconds");
        //    currentTarget = originalPosition;
        //    StartCoroutine("MoveOverSeconds");
        //}
    }

    //IEnumerator MoveOverSeconds()
    //{
    //    Vector3 currentPos = transform.position;
    //    timeElapsed = 0;
    //    float t = 0.0f;
    //    while (t < 1)
    //    {
    //        timeElapsed += Time.deltaTime;
    //        t = timeElapsed / totalMoveTime;
    //        transform.position = Vector3.Lerp(currentPos, currentTarget, t);
    //        yield return null;
    //    }
    //}
}
