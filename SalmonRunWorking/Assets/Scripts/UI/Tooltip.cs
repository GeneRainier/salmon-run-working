using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * Class that contains the content and appearance of a tooltip message
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance { get; private set; }        //< Singleton instance for the tooltip

    public Vector2 paddingSize;             //< Amount of padding around the text

    public Vector2 offset;                  //< Offset of tooltip from mouse pos

    [SerializeField] private Canvas canvas = null;     //< Canvas that the tooltip is on

    [SerializeField] private TextMeshProUGUI text = null;      //< Text for the tooltip

    [SerializeField] private RectTransform rectTransform = null;       //< The transform information for the tooltip
    
    [SerializeField] private RectTransform childRectTransform = null;  //< The transform information of any children objects to this tooltip

    #region Major Monobehaviour Functions

    /**
     * Awake is called after the initialization of gameobjects prior to the start of the game. This is used as an Initialization Function
     */
    private void Awake()
    {
        // Setup for singleton
        if (Instance == null)
        {
            Instance = this;
            HideTooltip();
        }
        else
        {
            Debug.LogError("More than one tooltip in the scene!");
        }
    }

    /**
     * Update is called once per frame
     */
    void Update()
    {
        UpdateTooltipPosition();
    }

    #endregion

    #region Private Functions

    /**
     * Show this tooltip
     * 
     * @param message string The message that will appear in the tooltip
     */
    public void ShowTooltip(string message)
    {
        // Make the tooltip appear and make sure it appears over everything else
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        // Set the text to match the text we've gotten
        text.text = message;
    }

    /**
     * Hide this tooltip
     */
    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    /**
     * Update the tooltip's position to the mouse 
     */
    private void UpdateTooltipPosition()
    {
        Vector2 localMousePos;

        // Turn the screen-space position of the mouse into a point local to the UI canvas
        // For reference, see https://stackoverflow.com/questions/43802207/position-ui-to-mouse-position-make-tooltip-panel-follow-cursor
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition, canvas.worldCamera,
            out localMousePos);

        // Use TransformPoint to get the actual correct position for the tooltip
        transform.position = canvas.transform.TransformPoint(localMousePos) + (Vector3)offset;

        UpdateSize();
    }

    /*
     * Updates the size of the tooltip message based on the size of its children
     */
    public void UpdateSize()
    {
        Vector2 size = childRectTransform.sizeDelta;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x + paddingSize.x * 2);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y + paddingSize.y * 2);
    }

    #endregion
}
