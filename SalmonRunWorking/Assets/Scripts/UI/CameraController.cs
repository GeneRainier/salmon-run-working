using System;
using UnityEngine;

/*
 * RTS-style camera controller script.
 * 
 * Based on tutorial by Brackeys: https://www.youtube.com/watch?v=cfjLQrMGEb4
 * NOTE: As of 2021, this has been adjusted substantially to better fit our new 2021 design documentation and direction
 * 
 * Authors: Benjamin Person (Rewrite in 2021)
 */
public class CameraController : MonoBehaviour {

    [Header("Pan Properties")]
    [SerializeField] private bool panWithMouse = false;     //< Can the mouse be used to pan as well as the arrow keys?

    [SerializeField] private float panSpeed = 0.0f;        //< How fast the camera pans

    [SerializeField] private float panBorderThickness = 1.0f;  //< Size of borders around edge of screen which will start panning when the mouse enters the area

    [Header("Camera Bounds")]
    [SerializeField] private MinMax bounds = null;     //< Farthest / closest the camera can zoom out. Min and Max values for x and y
    public float height;                        //< The difference between the max and min y values of bounds for calculating rotational interpolation

    [Header("Speeds")]
    [SerializeField] private float zoomSpeed = 2.0f;   //< The speed at which you can zoom in or out

    [SerializeField] private float lerpSpeed = 0.0f;   //< The speed at which the camera pans

    private Vector3 target;     //< The position the camera is aimed at
    private Vector3 targetRotation;   //< The rotation the camera is currently at
    private bool moving;        //< Is the camera moving?
    private float timeFactor;   //< What time scale is the game in?

    [SerializeField] private Vector3 initialPosition;       //< The location the camera is at when the level begins
    [SerializeField] private Vector3 initialRotation;       //< The rotation the camera is at when the level begins
    [SerializeField] private float initialXRotation;       //< The x rotation when the camera is fully zoomed out
    [SerializeField] private float zoomedXRotation;        //< The x rotation the camera has when it is fully zoomed in

    /*
     * Awake is called after the initialization of gameobjects prior to the start of the game. This is used as an Initialization Function
     */
    private void Awake()
    {
        initialPosition = this.gameObject.transform.position;
        initialRotation = this.gameObject.transform.rotation.eulerAngles;
        initialXRotation = this.gameObject.transform.rotation.eulerAngles.x;
        zoomedXRotation = 30.0f;
        height = bounds.Max.y - bounds.Min.y;
}

    /**
     * Update is called once every frame update
     */
    private void Update ()
    {
        // We UpdateTarget here to make the movement of the camera consistent across all TimeScales
        // Movement should generally be handled in FixedUpdate for consistency across all scripts
        UpdateTarget();
    }

    /*
     * LateUpdate is called once per frame after Update (and FixedUpdate in this case) is complete
     */
    private void LateUpdate()
    {
        // We physically move the camera after the UI has been adjusted so that the tooltips do not flash when the camera moves
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

        // Get camera's current pos
        target = transform.position;

        // Figure out new zoom
        // TODO: SCROLL MORE WHEN YOU ARE CLOSE TO THE EDGE
        float scroll = 12f * Input.GetAxis("Mouse ScrollWheel");

        // Look for keyboard inputs as well
        if (Input.GetButton("Zoom") || Input.GetButton("Zoom"))
        {
            scroll = 0.1f * 100.0f * Input.GetAxisRaw("Zoom");
        }

        target.y += scroll * zoomSpeed * Time.unscaledDeltaTime;

        // Figure out how much distance the pan should cover
        // Multiplied by unscaledDeltaTime so pausing and fast forwarding does not affect camera movement
        float panDistance = panSpeed * Time.unscaledDeltaTime;

        // Pan depending on which keys have been pressed (or which borders the mouse is currently in)
        if (Input.GetButton("Vertical") || (panWithMouse &&
            (Input.mousePosition.y >= Screen.height - panBorderThickness || Input.mousePosition.y <= panBorderThickness)))
        {
            target.z += panDistance * Input.GetAxisRaw("Vertical");
        }
        if (Input.GetButton("Horizontal") || (panWithMouse &&
            (Input.mousePosition.x <= panBorderThickness || Input.mousePosition.x >= Screen.width - panBorderThickness)))
        {
            target.x += panDistance * Input.GetAxisRaw("Horizontal");
        }

        // Clamp Pan (XZ) / Zoom (Y) within boundaries
        target = bounds.Clamp(target);
    }

    /*
     * Updates the position of the camera
     */
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

        // Calculate the interpolation factor for the camera rotation
        // This is handle in UpdatePosition rather than UpdateTarget to keep the Slerp effect smooth
        float interpolation = (transform.position.y - bounds.Min.y) / (height);
        float xRotation = Mathf.Lerp(zoomedXRotation, initialXRotation, interpolation);
        transform.rotation = Quaternion.Euler(xRotation, transform.rotation.y, transform.rotation.z);
    }

    /*
     * Set the camera position and rotation back to the start of the level when the end of round button is clicked
     */
    public void ResetCameraPosition()
    {
        this.gameObject.transform.position = initialPosition;
        this.gameObject.transform.rotation = Quaternion.Euler(initialRotation);
    }
}

/*
 * The minimum and maximum values the camera may utilize in zooming and panning
 */
[Serializable]
public class MinMax
{
    [SerializeField] private Vector3 min = Vector3.zero;       //< The minimum location
    [SerializeField] private Vector3 max = Vector3.zero;       //< The maximum location

    public Vector3 Min => min;

    public Vector3 Max => max;

    /*
     * Clamps the given value between the given minimum float and maximum float values.
     * 
     * @param target The location value to clamp values from
     */
    public Vector3 Clamp(Vector3 target)
    {
        float x = Mathf.Clamp(target.x, min.x, max.x);
        float y = Mathf.Clamp(target.y, min.y, max.y);
        float z = Mathf.Clamp(target.z, min.z, max.z);
        return new Vector3(x, y, z);
    }
}
