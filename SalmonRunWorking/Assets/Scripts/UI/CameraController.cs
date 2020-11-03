using System;
using UnityEngine;

/*
 * RTS-style camera controller script.
 * 
 * Based on tutorial by Brackeys: https://www.youtube.com/watch?v=cfjLQrMGEb4
 */
public class CameraController : MonoBehaviour {

    [Header("Pan Properties")]
    // can the mouse be used to pan as well as the arrow keys?
    [SerializeField] private bool panWithMouse;

    // how fast the camera pans
    [SerializeField] private float panSpeed;

    // lowest speed the camera can pan at, expressed in terms of a fraction of the default pan speed
    [Range(0f, 1f)]
    [SerializeField] private float minPanSpeedRatio;

    // size of borders around edge of screen which will start panning when the mouse enters the area
    [SerializeField] private float panBorderThickness;

    [Header("Camera Bounds")]
    // farthest / closest the camera can zoom out
    // min and max values for x and y
    [SerializeField] private MinMax bounds;
    
    [Header("Speeds")]

    // how speed at which you can zoom in or out
    [SerializeField] private float zoomSpeed;

    [SerializeField] private float lerpSpeed;
	
    private Vector3 target;
    private bool moving;
    private float timeFactor;
    
	private void FixedUpdate ()
    {
        UpdateTarget();

        UpdatePosition();
    }

    /**
     * Update camera position and zoom
     */
    private void UpdateTarget()
    {
        if (!moving)
        {
            moving = true;
            target = transform.position;
        }

        timeFactor = Time.deltaTime / Time.timeScale;
        
        // get camera's current pos
        target = transform.position;

        // figure out new zoom
        // TODO: SCROLL MORE WHEN YOU ARE CLOSE TO THE EDGE
        float scroll = 12f * Input.GetAxis("Mouse ScrollWheel");

        // look for keyboard inputs as well
        if (Input.GetButton("Zoom")|| Input.GetButton("Zoom"))
        {
            scroll = 0.1f * Input.GetAxisRaw("Zoom");
        }

        target.z += scroll * zoomSpeed * 100f * timeFactor;

        // modulate the pan speed based on the current zoom level (smaller pan when more zoomed in)
        float zoomMultiplier = 1.1f - (target.z - bounds.Min.z) / (bounds.Max.z - bounds.Min.z);

        // figure out how much distance the pan should cover
        // dividing by timeScale so we always appear to pan at the same speed regardless of gameplay speed
        float panDistance = Mathf.Min(panSpeed * zoomMultiplier * timeFactor, panSpeed * timeFactor);
        
        // pan depending on which keys have been pressed (or which borders the mouse is currently in)
        if (Input.GetButton("Vertical") || panWithMouse && 
            (Input.mousePosition.y >= Screen.height - panBorderThickness || Input.mousePosition.y <= panBorderThickness))
        {
            target.y += panDistance * Input.GetAxisRaw("Vertical");
        }
        if (Input.GetButton("Horizontal") || panWithMouse && 
            (Input.mousePosition.x <= panBorderThickness || Input.mousePosition.x >= Screen.width - panBorderThickness))
        {
            target.x += panDistance * Input.GetAxisRaw("Horizontal");
        }

        // Clamp Pan (XY) / Zoom (Z) within boundaries
        target = bounds.Clamp(target);
    }

    private void UpdatePosition()
    {
        if (moving)
        {
            if (transform.ComparePosition(target, 0.01f))
            {
                moving = false;
                return;
            }
            transform.SmoothMoveTowards(target, lerpSpeed);
        }
    }
}

[Serializable]
public class MinMax
{
    [SerializeField] private Vector3 min;
    [SerializeField] private Vector3 max;

    public Vector3 Min => min;

    public Vector3 Max => max;

    public Vector3 Clamp(Vector3 target)
    {
        float x = Mathf.Clamp(target.x, min.x, max.x);
        float y = Mathf.Clamp(target.y, min.y, max.y);
        float z = Mathf.Clamp(target.z, min.z, max.z);
        return new Vector3(x, y, z);
    }
}
