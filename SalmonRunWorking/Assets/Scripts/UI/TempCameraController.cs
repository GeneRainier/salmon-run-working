using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TempCameraController : MonoBehaviour
{
    public GameObject panCamera;
    public GameObject towerCamera;
    public GameObject fishCamera;
    public CinemachineVirtualCamera panCamCinemachine;
    public CinemachineVirtualCamera towerCamCinemachine;
    public CinemachineVirtualCamera fishCamCinemachine;

    public Vector3 camFocusPoint;
    public float zoomDistance = 200;
    public float panSpeed = 2;

    public enum CameraState
    {
        pan,
        tower,
        fish
    }

    public CameraState camState;

    // Start is called before the first frame update
    void Start()
    {
        panCamCinemachine = panCamera.GetComponent<CinemachineVirtualCamera>();
        towerCamCinemachine = towerCamera.GetComponent<CinemachineVirtualCamera>();
        fishCamCinemachine = fishCamera.GetComponent<CinemachineVirtualCamera>();
        camState = CameraState.pan;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T) && camState == CameraState.pan)
        {
            camState = CameraState.tower;
        }
        else if(Input.GetKeyDown(KeyCode.T) && camState == CameraState.tower)
        {
            camState = CameraState.pan;
        }

        if(camState == CameraState.pan)
        {
            PanCameraController();
        }
        if(camState == CameraState.tower)
        {
            TowerCameraController();
        }
    }

    void PanCameraController()
    {
        panCamCinemachine.Priority = 10;
        towerCamCinemachine.Priority = 9;
        fishCamCinemachine.Priority = 9;
        if (Input.GetButton("Vertical"))
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                // Calculate the perpendicular axis of travel for the camera based on its current rotation
                Vector3 forward = Vector3.Cross(transform.right, Vector3.up);
                camFocusPoint = camFocusPoint + panSpeed * forward * Input.GetAxisRaw("Vertical");
            }
            else
            {
                camFocusPoint = camFocusPoint + panSpeed * Vector3.forward * Input.GetAxisRaw("Vertical");
            }
        }
        if (Input.GetButton("Horizontal"))
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                Vector3 right = Vector3.Cross(Vector3.up, transform.up);
                camFocusPoint = camFocusPoint + panSpeed * right * Input.GetAxisRaw("Horizontal");
            }
            else
            {
                camFocusPoint = camFocusPoint + panSpeed * Vector3.right * Input.GetAxisRaw("Horizontal");
            }
        }

        if (Input.GetButton("Rotate"))
        {
            panCamera.transform.RotateAround(transform.position, Vector3.up, -1.0f * Input.GetAxisRaw("Rotate"));
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            camFocusPoint = new Vector3(0, 0, 0);
            transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        panCamera.transform.position = new Vector3(camFocusPoint.x, camFocusPoint.y + zoomDistance, camFocusPoint.z);
    }
    void TowerCameraController()
    {
        panCamCinemachine.Priority = 9;
        towerCamCinemachine.Priority = 10;
        fishCamCinemachine.Priority = 9;
    }
}
