using System;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;
using System.Collections;

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
    [SerializeField] private float xLimitMax;
    [SerializeField] private float xLimitMin;
    [SerializeField] private float zLimitMax;
    [SerializeField] private float zLimitMin;

    [Header("Camera Bounds")]
    public float height;                               //< The difference between the max and min y values of bounds for calculating rotational interpolation

    [Header("Speeds")]
    [SerializeField] private float zoomSpeed = 2.0f;   //< The speed at which you can zoom in or out
    [SerializeField] private float xRotateSpeed = 3f;
    [SerializeField] private float towerOrbitSpeed = 3f;

    private Vector3 target;           //< The position the camera is aimed at
    private Vector3 targetRotation;   //< The rotation the camera is currently at
    private bool moving;              //< Is the camera moving?
    private float timeFactor;         //< What time scale is the game in?

    [Header("Rotations")]
    [SerializeField] private Vector3 initialPosition;       //< The location the camera is at when the level begins
    [SerializeField] private Vector3 initialRotation;       //< The rotation the camera is at when the level begins
    [SerializeField] private float initialXRotation;       //< The x rotation when the camera is fully zoomed out
    [SerializeField] private float zoomedXRotation;        //< The x rotation the camera has when it is fully zoomed in
    private float interpolation = 1.0f;                     //< The percentage value we are at in the xAxis rotation Lerp

    [Header("Cameras")]
    [SerializeField] private Camera cameraMain;
    [SerializeField] private Camera cameraTower;
    [SerializeField] private Camera cameraFish;

    [SerializeField] private CinemachineVirtualCamera virtualCameraTower;
    [SerializeField] private CinemachineVirtualCamera virtualCameraFish;

    [Header("Focused Objects")]
    private GameObject selectedTower;
    public GameObject firstFish;
    public GameObject lastFish;

    [Header("Transitions")]
    [SerializeField] private PlayableDirector mainTowerTransition;
    [SerializeField] private PlayableDirector towerMainTransition;
    [SerializeField] private PlayableDirector mainFishTransition;
    [SerializeField] private PlayableDirector fishMainTransition;

    public enum CamState
    {
        camMain,
        camTower,
        camFish
    }

    public CamState camState = CamState.camMain;

    /*
     * Awake is called after the initialization of gameobjects prior to the start of the game. This is used as an Initialization Function
     */
    private void Awake()
    {
        initialPosition = this.gameObject.transform.position;
        initialRotation = this.gameObject.transform.rotation.eulerAngles;
        initialXRotation = this.gameObject.transform.rotation.eulerAngles.x;
        zoomedXRotation = 30.0f;
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

    private void Start()
    {
        cameraMain.tag = "MainCamera";
    }

    /*
     * Updates the position of the camera
     */
    private void UpdatePosition()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && camState == CamState.camMain)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            if (hit.collider)
            {
                if (hit.collider.gameObject.tag == "Tower")
                {
                    selectedTower = hit.collider.gameObject;
                    camState = CamState.camTower;
                    StartCoroutine("MainToTowerRoutine");
                }
            }
        }

        else if (Input.GetKeyDown(KeyCode.P) && camState == CamState.camTower)
        {
            camState = CamState.camMain;
            StartCoroutine("TowerToMainRoutine");
        }

        /*
        else if (Input.GetKeyDown(KeyCode.O) && camState == CamState.camMain)
        {
            camState = CamState.camFish;
            StartCoroutine("MainToFishRoutine");
        }
        
        else if (Input.GetKeyDown(KeyCode.O) && camState == CamState.camFish)
        {
            camState = CamState.camMain;
            StartCoroutine("FishToMainRoutine");
        }
        */

        if (camState == CamState.camMain)
        {
            CamMainUpdate();
        }
        else if (camState == CamState.camTower)
        {
            CamTowerUpdate();
        }
        
        else if (camState == CamState.camFish)
        {
            CamFishUpdate();
        }
        
    }

    public void CamMainUpdate()
    {
        // Search for rotation input
        // We only want to turn the camera if we are zoomed in
        if (Input.GetButton("Rotate"))
        {
            cameraMain.transform.RotateAround(cameraMain.transform.position, Vector3.up, xRotateSpeed * Input.GetAxisRaw("Rotate"));
        }

        // Store the current rotation for when we apply the XRotation Lerp
        Vector3 currentRotation = cameraMain.transform.rotation.eulerAngles;

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
                cameraMain.transform.Translate(forward * Input.GetAxisRaw("Vertical") * panDistance, Space.World);
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
                cameraMain.transform.Translate(right * panDistance * Input.GetAxisRaw("Horizontal"), Space.World);
            }
            else
            {
                cameraMain.transform.Translate(Vector3.right * panDistance * Input.GetAxisRaw("Horizontal"), Space.World);
            }
        }

        if(cameraMain.transform.position.x > xLimitMax)
        {
            cameraMain.transform.position = new Vector3(xLimitMax, cameraMain.transform.position.y, cameraMain.transform.position.z);
        }
        if (cameraMain.transform.position.x < xLimitMin)
        {
            cameraMain.transform.position = new Vector3(xLimitMin, cameraMain.transform.position.y, cameraMain.transform.position.z);
        }
        if (cameraMain.transform.position.z > zLimitMax)
        {
            cameraMain.transform.position = new Vector3(cameraMain.transform.position.x, cameraMain.transform.position.y, zLimitMax);
        }
        if (cameraMain.transform.position.z < zLimitMin)
        {
            cameraMain.transform.position = new Vector3(cameraMain.transform.position.x, cameraMain.transform.position.y, zLimitMin);
        }
    }

    public void CamTowerUpdate()
    {
        virtualCameraTower.transform.RotateAround(selectedTower.transform.position, Vector3.up, Input.GetAxisRaw("Rotate") * -towerOrbitSpeed);
    }

    public void CamFishUpdate()
    {

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

    public IEnumerator MainToTowerRoutine()
    {
        virtualCameraTower.transform.position = selectedTower.transform.position + new Vector3(35, 20, 0);
        virtualCameraTower.transform.LookAt(selectedTower.transform.position + new Vector3(0, 10, 0));
        mainTowerTransition.Play();
        yield return new WaitForSeconds(1);
        yield break;
    }

    public IEnumerator TowerToMainRoutine()
    {
        mainTowerTransition.Stop();
        towerMainTransition.Play();
        yield return new WaitForSeconds(1);
        yield break;
    }

    public IEnumerator MainToFishRoutine()
    {
        virtualCameraFish.transform.position = new Vector3(-285, 147, 0);
        //virtualCameraFish.transform.LookAt(selectedFish.transform.position + new Vector3(0, 10, 0));
        mainFishTransition.Play();
        yield return new WaitForSeconds(1);
        yield break;
    }

    public IEnumerator FishToMainRoutine()
    {
        mainFishTransition.Stop();
        fishMainTransition.Play();
        yield return new WaitForSeconds(1);
        yield break;
    }
}
