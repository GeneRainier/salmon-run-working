using System;
using UnityEngine;

/*
 * RTS-style camera controller script.
 * 
 * Originally based on a tutorial by Brackeys: https://www.youtube.com/watch?v=cfjLQrMGEb4
 * NOTE: As of 2021, this has been adjusted substantially to better fit our new 2021 design documentation and direction.
 * The new controller removes the need for a camera target to follow and moves its operation to LateUpdate to accomodate tooltips
 * not flashing when the camera moves. The input scheme is essentially the same, but directly modifies the camera's transform
 * rather than adjusting the target and then moving the camera to match. The movement is also split between the WASD controls
 * which move locally to the camera and the Arrow Keys which move north, south, east, west in the relative the to game scene.
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
    public float height;                               //< The difference between the max and min y values of bounds for calculating rotational interpolation
    [SerializeField] private float xRotateMin = 20f;                     //< Minimum rotation set for the camera when rotating on the x axis
    [SerializeField] private float xRotateMax = 89f;                   //< Maximum rotation set for the camera when rotating on the x axis

    [Header("Speeds")]
    [SerializeField] private float zoomSpeed = 2.0f;   //< The speed at which you can zoom in or out
    [SerializeField] private float xRotateSpeed = 50f;

    private Vector3 target;           //< The position the camera is aimed at
    private Vector3 targetRotation;   //< The rotation the camera is currently at
    private bool moving;              //< Is the camera moving?
    private float timeFactor;         //< What time scale is the game in?

    [SerializeField] private Vector3 initialPosition;       //< The location the camera is at when the level begins
    [SerializeField] private Vector3 initialRotation;       //< The rotation the camera is at when the level begins
    [SerializeField] private float initialXRotation;       //< The x rotation when the camera is fully zoomed out
    [SerializeField] private float zoomedXRotation;        //< The x rotation the camera has when it is fully zoomed in
    private float interpolation = 1.0f;                     //< The percentage value we are at in the xAxis rotation Lerp

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

    /*
     * LateUpdate is called once per frame after Update (and FixedUpdate in this case) is complete
     */
    private void LateUpdate()
    {
        // We physically move the camera after the UI has been adjusted so that the tooltips do not flash when the camera moves
        UpdatePosition();

        // Pressing the F key will reset the camera to it's original y position and Vector3 rotation
        if (Input.GetKeyDown(KeyCode.F))
        {
            ResetCameraRotation();
        }
    }

    /*
     * Updates the position of the camera
     */
    private void UpdatePosition()
    {
        // Figure out new zoom
        float scroll = 12f * Input.GetAxis("Mouse ScrollWheel");

        // Search for rotation input
        // We only want to turn the camera if we are zoomed in
        if (Input.GetButton("Rotate"))
        {
            transform.RotateAround(transform.position, Vector3.up, -1.0f * Input.GetAxisRaw("Rotate"));
        }

        // Store the current rotation for when we apply the XRotation Lerp
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // Figure out how much distance the pan should cover
        // Multiplied by unscaledDeltaTime so pausing and fast forwarding does not affect camera movement
        float panDistance = panSpeed * Time.unscaledDeltaTime;

        // Pan depending on which keys have been pressed (or which borders the mouse is currently in)
        if (Input.GetButton("Vertical") || (panWithMouse &&
            (Input.mousePosition.y >= Screen.height - panBorderThickness || Input.mousePosition.y <= panBorderThickness)))
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                // Calculate the perpendicular axis of travel for the camera based on its current rotation
                Vector3 forward = Vector3.Cross(transform.right, Vector3.up);
                transform.Translate(forward * Input.GetAxisRaw("Vertical") * panDistance, Space.World);
            }
            else
            {
                transform.Translate(Vector3.forward * panDistance * Input.GetAxisRaw("Vertical"), Space.World);
            }
        }
        if (Input.GetButton("Horizontal") || (panWithMouse &&
            (Input.mousePosition.x <= panBorderThickness || Input.mousePosition.x >= Screen.width - panBorderThickness)))
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                Vector3 right = Vector3.Cross(Vector3.up, transform.up);
                transform.Translate(right * panDistance * Input.GetAxisRaw("Horizontal"), Space.World);
            }
            else
            {
                transform.Translate(Vector3.right * panDistance * Input.GetAxisRaw("Horizontal"), Space.World);
            }
        }

        //Search for the zoom buttons (Q and E)
        if(Input.GetButton("Zoom"))
        {
            //Use the axis of zoom to determine which way to zoom
            scroll = 0.1f * 100.0f * Input.GetAxisRaw("Zoom");
            transform.Translate(Vector3.down * scroll * zoomSpeed, Space.World);
        }

        //Rotate on the x axis based on keyboard input.
        if(Input.GetKey(KeyCode.C) && currentRotation.x > xRotateMin)
        {
            /*
             * Fancier camera movement, not finished yet!
            RaycastHit hit;
            if(Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, 10000))
            {
                transform.RotateAround(hit.point, Vector3.right, -xRotateSpeed * Time.fixedDeltaTime);
            }
            else
            {
                Debug.LogError("Camera X rotation raycast didn't find an object!");
            }
            */
            //Change rotation based on time between frames 

            float rx =currentRotation.x - xRotateSpeed * Time.unscaledDeltaTime;
            if (rx < xRotateMin)
            {
                rx = xRotateMin;
            }
            transform.rotation = Quaternion.Euler(rx, currentRotation.y, currentRotation.z);
            if (currentRotation.x < xRotateMin)
            {
                transform.rotation = Quaternion.Euler(xRotateMin, currentRotation.y, currentRotation.z);
            }
            
        }
        else if (Input.GetKey(KeyCode.V) && currentRotation.x < xRotateMax)
        {
            /* 
             * Fancier camera movement, not finished yet!
            RaycastHit hit;
            if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, 10000))
            {
                transform.RotateAround(hit.point, Vector3.left, -xRotateSpeed * Time.fixedDeltaTime);
            }
            else
            {
                Debug.LogError("Camera X rotation raycast didn't find an object!");
            }
            */
            //Change rotation based on time between frames 
            
            float rx = currentRotation.x + xRotateSpeed * Time.unscaledDeltaTime;
            if (rx > xRotateMax)
            {
                rx = xRotateMax;
            }
            transform.rotation = Quaternion.Euler(rx, currentRotation.y, currentRotation.z);
            if (currentRotation.x > xRotateMax)
            {
                transform.rotation = Quaternion.Euler(xRotateMax, currentRotation.y, currentRotation.z);
            }
            
        }

        // Clamp Pan (XZ) / Zoom (Y) within boundaries
        transform.position = bounds.Clamp(transform.position);

    }

    /*
     * Set the camera position and rotation back to the start of the level when the end of round button is clicked
     */
    public void ResetCamera()
    {
        this.gameObject.transform.position = initialPosition;
        this.gameObject.transform.rotation = Quaternion.Euler(initialRotation);
    }

    /*
     * Set the camera position and rotation back to their initial values during a round.
     * This will set the position back to its maximum y and its rotation back to its initial values
     */
    public void ResetCameraRotation()
    {
        this.gameObject.transform.position = new Vector3(transform.position.x, initialPosition.y, transform.position.z);
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
