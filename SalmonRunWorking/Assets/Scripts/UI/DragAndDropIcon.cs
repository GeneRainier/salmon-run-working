using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/**
 * Script that controls a drag and drop icon for dragging towers onto the play area.
 * 
 * Original Author: Jacob Cousineau
 * Subsequent Authors: Benjamin Person
 */
[RequireComponent(typeof(RectTransform))]
public class DragAndDropIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public GameObject objectToSpawn;  //< The object we should attempt to spawn
                                      // If public it will show up in the inspector; this includes varibles that might be have a start or default value
                                      // public color mycolor with show up as a color picker to get a start color
                                      // can put a hide inspector. [SerializeField] will also show up in inspector, but not be public
                                      // Serialize Field will also keep its value between runs. 

    public Image icon;                //< The image for this type of tower that shows in the UI

    private RectTransform rectTransform;    //< The rect transform for the icon

    private GameObject spawnedObject;       //< The instance of the object we will (attempt to) spawn

    private LayerMask spawnedObjectLayer;   //< Layer the spawned object started on and will be restored to once placed

    private IDragAndDropObject spawnedObjectDragAndDrop;    //< The Tower interface attached to the spawned object

    // Hold offsets for corners of spawned object renderer's bounding box
    // Used for raycasting from all four corners of the spawned object in addition to the center
    private Vector3 spawnedObjectOffsetTopLeft;
    private Vector3 spawnedObjectOffsetTopRight;
    private Vector3 spawnedObjectOffsetBottomLeft;
    private Vector3 spawnedObjectOffsetBottomRight;

    private int raycastLayerMask;       //< Holds layer mask for raycasting

    // Main camera and necessary manager script references
    private Camera mainCamera;
    private GameManager gameManager;
    private MoneyManager moneyManager;
    
    [SerializeField] private Image iconImage;   //< We find the icon image to set its color

    private TowerUI towerUI;            //< Reference to the TowerUI script for necessary functions

    // Boolean determinants for purchasing single purchase items like dams and ladders
    public bool isOneTimePurcahse;
    private bool hasBeenPurchased;

    /**
     * Initialization
     */
    void Start ()
    {
        // Get rect transform component
        rectTransform = GetComponent<RectTransform>();

        towerUI = GetComponent<TowerUI>();

        // Get layer mask for necessary layers
        raycastLayerMask = LayerMask.GetMask(Layers.FLOOR_LAYER_NAME, Layers.TERRAIN_LAYER_NAME, Layers.PLACED_OBJECTS);

        mainCamera = Camera.main;
        gameManager = ManagerIndex.MI.GameManager;

        hasBeenPurchased = false;
    }

    private void Update()
    {
        // Check each frame update for whether the player can afford the specific item
        CanBuy();
    }

    /*
     * This function changes the color depending on if you have enough money or not
     */
    private void CanBuy()
    {
        if (!gameManager.Started) return;

        if (isOneTimePurcahse)
        {
            if (hasBeenPurchased)
            {
                iconImage.color = new Color32(166, 255, 151, 255);
            }
            else
            {
                if (gameManager.PlaceState && towerUI.CanAfford)
                {
                    iconImage.color = new Color32(233, 233, 233, 255);
                }
                else
                {
                    iconImage.color = new Color32(24, 24, 24, 255);
                }
            }
        }
        else
        {
            if (gameManager.PlaceState && towerUI.CanAfford)
            {
                iconImage.color = new Color32(233, 233, 233, 255);
            }
            else
            {
                iconImage.color = new Color32(24, 24, 24, 255);
            }
        }
    }

    /**
     * Handle beginning of a drag action on this object
     * @param eventData The PointerEventData for the drag which allows us to reference the pointer position for UpdateDrag
     */
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!gameManager.PlaceState) return;
        if (towerUI.CanAfford)
        {
            // Spawn the object
            // Keep it invisible and don't set position yet because we haven't actually figured out where it's going to go yet,
            // but we need to instantiate it so we can figure out whether 
            spawnedObject = Instantiate(objectToSpawn);

            // Set the spawned object to be on the unplaced object layer, keeping its old layer for future use
            spawnedObjectLayer = spawnedObject.layer;
            Layers.MoveAllToLayer(spawnedObject, LayerMask.NameToLayer(Layers.IGNORE_RAYCAST_LAYER_NAME));

            // Get the IDragAndDrop interface for the spawned object
            spawnedObjectDragAndDrop = spawnedObject.GetComponentInChildren<IDragAndDropObject>();
            if (spawnedObjectDragAndDrop == null)
            {
                Debug.LogError("Trying to drag and drop an object with no IDragAndDrop interface implemented!");
                return;
            }
            
            // Show any visualizations that may exist for the object
            PlacementVisualizationManager.Instance.DisplayVisualization(spawnedObjectDragAndDrop.GetType(), true);

            // Calculate the offsets needed to get the points at the corners of the spawned object's renderer's bounding box
            // NOTE: These are * 0.25f to allow for Anglers and Rangers to be placed closer to the shore of the river 
            if (spawnedObjectDragAndDrop is MonoBehaviour mb && mb.GetComponent<MeshRenderer>().bounds is Bounds bounds)
            {
                spawnedObjectOffsetTopLeft = new Vector3(-1 * bounds.extents.x * 0.25f, 0, bounds.extents.z * 0.25f);
                spawnedObjectOffsetTopRight = new Vector3(bounds.extents.x * 0.25f, 0, bounds.extents.z * 0.25f);
                spawnedObjectOffsetBottomLeft = new Vector3(-1 * bounds.extents.x * 0.25f, 0, -1 * bounds.extents.z * 0.25f);
                spawnedObjectOffsetBottomRight = new Vector3(bounds.extents.x * 0.25f, 0, -1 * bounds.extents.z * 0.25f);
            }
            else
            {
                Debug.LogError("Attempting to spawn object from drag and drop that has no MeshRenderer!");
            }
            
            // Start moving the icon
            UpdateDrag(eventData);
        }
        else
        {
            Debug.Log("Not Enough Money");
        }
    }

    /**
     * Handle middle of drag action on this object
     * @param eventData The PointerEventData for the drag which allows us to reference the pointer position for UpdateDrag
     */
    public void OnDrag(PointerEventData eventData)
    {
        if (!gameManager.PlaceState) return;
        UpdateDrag(eventData);
    }

    /**
     * Handle end of drag action on this object
     * @param eventData The PointerEventData for the drag which allows us to reference the pointer position for UpdateDrag
     */
    public void OnEndDrag(PointerEventData eventData)
    {
        UpdateDrag(eventData, true);
    }

    /**
     * Make the object follow the pointer as a drag action is occuring
     * 
     * Originally taken from Unity documentation (https://docs.unity3d.com/ScriptReference/EventSystems.IDragHandler.html) but has been heavily modified
     * 
     * @param data PointerEventData The pointer data passed in from OnDragBegin or OnDrag
     * @param finalUpdate bool True if this is the last update at the end of a drag
     */
    private void UpdateDrag(PointerEventData data, bool finalUpdate = false)
    {
        if (!spawnedObject) return;
        
        Vector3 cameraPosition = mainCamera.transform.position;
        
        // Create a ray from the camera towards the terrain
        Ray primaryRay = mainCamera.ScreenPointToRay(data.position);

        // Create a list to hold all of the results from raycasts
        List<RaycastHit> secondaryHitInfo = new List<RaycastHit>();

        // Create variable to store initial raycast result data
        RaycastHit primaryHitInfo;

        // Attempt a raycast on the terrain and floor (water tilemap) layers
        bool hit = Physics.Raycast(primaryRay, out primaryHitInfo, Mathf.Infinity, raycastLayerMask);
        if (hit)
        {
            // We hit something!

            // If we're in the editor, draw the ray
            #if UNITY_EDITOR
            Debug.DrawRay(primaryRay.origin, primaryRay.direction * primaryHitInfo.distance, Color.green);
            #endif

            // Update the location of the spawned object to be at the point of the raycast hit
            spawnedObject.transform.position = primaryHitInfo.point;

            // Do some secondary raycasts to get more information
            // This gets a bit complicated...

            // Figure out the origin for a ray that originates on the camera's y plane
            // and goes through the center of the drag and drop object at a 90 degree angle
            float raycastOriginDepth = cameraPosition.y - primaryHitInfo.point.y;
            
            Vector3 raycastOrigin = mainCamera.ScreenToWorldPoint(new Vector3(data.position.x, data.position.y, raycastOriginDepth));

            // Want to raycast from the camera y, so reset the y value
            raycastOrigin.y = cameraPosition.y;

            // Make modifications to this origin so that the new origins are at each corner of the drag and drop object's bounding box
            Vector3 raycastOriginTopLeft = raycastOrigin + spawnedObjectOffsetTopLeft;
            Vector3 raycastOriginTopRight = raycastOrigin + spawnedObjectOffsetTopRight;
            Vector3 raycastOriginBottomLeft = raycastOrigin + spawnedObjectOffsetBottomLeft;
            Vector3 raycastOriginBottomRight = raycastOrigin + spawnedObjectOffsetBottomRight;

            // Do raycasts from each of these origins forward into the scene, storing their results
            RaycastHit topLeftHitInfo;
            RaycastHit topRightHitInfo;
            RaycastHit bottomLeftHitInfo;
            RaycastHit bottomRightHitInfo;
            Physics.Raycast(raycastOriginTopLeft, Vector3.down, out topLeftHitInfo, Mathf.Infinity, raycastLayerMask);
            Physics.Raycast(raycastOriginTopRight, Vector3.down, out topRightHitInfo, Mathf.Infinity, raycastLayerMask);
            Physics.Raycast(raycastOriginBottomLeft, Vector3.down, out bottomLeftHitInfo, Mathf.Infinity, raycastLayerMask);
            Physics.Raycast(raycastOriginBottomRight, Vector3.down, out bottomRightHitInfo, Mathf.Infinity, raycastLayerMask);

            // Draw these raycasts for debugging purposes
            // NOTE: These will be visible immediately around the dragged tower in the Unity Scene whenever the pointer MOVES
            Debug.DrawRay(raycastOriginTopLeft, Vector3.down * (cameraPosition.y - topLeftHitInfo.point.y), Color.blue);
            Debug.DrawRay(raycastOriginTopRight, Vector3.down * (cameraPosition.y - topRightHitInfo.point.y), Color.blue);
            Debug.DrawRay(raycastOriginBottomLeft, Vector3.down * (cameraPosition.y - bottomLeftHitInfo.point.y), Color.blue);
            Debug.DrawRay(raycastOriginBottomRight, Vector3.down * (cameraPosition.y - bottomRightHitInfo.point.y), Color.blue);

            // Put the secondary raycasts in a list to send along to the drag and drop object
            secondaryHitInfo.Add(topLeftHitInfo);
            secondaryHitInfo.Add(topRightHitInfo);
            secondaryHitInfo.Add(bottomLeftHitInfo);
            secondaryHitInfo.Add(bottomRightHitInfo);
        }

        // Check if the current location of the spawn is valid
        bool locationValid = spawnedObjectDragAndDrop.PlacementValid(primaryHitInfo, secondaryHitInfo);

        // If this is the final drag update, call a method to handle it
        if (finalUpdate)
        {
            FinalizeDrag(locationValid, primaryHitInfo, secondaryHitInfo);
        }
    }


    /**
     * Finalize spawning of objects at the end of a drag and drop.
     * 
     * @param locationValid bool True if the cursor is over a valid location to place the object
     * @param hitInfo RaycastHit Result of the raycast done to test location validity
     */
    private void FinalizeDrag(bool locationValid, RaycastHit primaryHitInfo, List<RaycastHit> secondaryHitInfo)
    {
        // Hide any visualizations that may have existed for the object
        PlacementVisualizationManager.Instance.DisplayVisualization(spawnedObjectDragAndDrop.GetType(), false);

        // If we are in a valid place, spawn the object
        if (locationValid)
        {
            // Place the spawned object
            spawnedObjectDragAndDrop.Place(primaryHitInfo, secondaryHitInfo);

            // Make it visible and turn it on
            spawnedObject.SetActive(true);

            // Set the layer back to whatever it was before
            Layers.MoveAllToLayer(spawnedObject, spawnedObjectLayer);

            // If the spawnedObject is the salmon ladder, mark this as the first ladder for UpgradeManager
            if (towerUI.TowerType == TowerType.Ladder)
            {
                ManagerIndex.MI.UpgradeManager.OriginalLadder = spawnedObject;
            }

            if (towerUI.TowerType == TowerType.Angler)
            {
                ManagerIndex.MI.UpgradeManager.isAFisherman = true;
            }

            // Subtract the cost of the tower from your bank
            towerUI.Purchase();

            if (isOneTimePurcahse)
            {
                hasBeenPurchased = true;
            }

            spawnedObject = null;
        }
        // If not in a valid place, destroy the spawned object
        else
        {
            // Destroy object
            Destroy(spawnedObject);

            // Clear old references, reset values to default
            spawnedObject = null;
            spawnedObjectDragAndDrop = null;
            spawnedObjectOffsetTopLeft = Vector3.zero;
            spawnedObjectOffsetTopRight = Vector3.zero;
            spawnedObjectOffsetBottomLeft = Vector3.zero;
            spawnedObjectOffsetBottomRight = Vector3.zero;
        }
    }
}
