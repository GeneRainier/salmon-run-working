using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    // singleton instance for the tooltip
    public static Tooltip Instance { get; private set; }

    // amount of padding around the text
    public Vector2 paddingSize;

    // offset of tooltip from mouse pos
    public Vector2 offset;

    // canvas that the tooltip is on
    [SerializeField] private Canvas canvas;

    // background image for tooltip
    [SerializeField] private Image backgroundImage;

    // text for the tooltip
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private RectTransform rectTransform;
    
    [SerializeField] private RectTransform childRectTransform;

    #region Major Monobehaviour Functions

    /**
     * Initialization function
     */
    private void Awake()
    {
        // setup for singleton
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
        // make the tooltip appear and make sure it appears over everything else
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        // set the text to match the text we've gotten
        text.text = message;

        // size the tooltip's background to the text
        //backgroundImage.rectTransform.sizeDelta = new Vector2(text.preferredWidth + (2 * paddingSize), text.preferredHeight + (2 * paddingSize));

        // offset the text for the padding size
        //text.rectTransform.anchoredPosition = new Vector2(paddingSize, paddingSize);
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

        // turn the screen-space position of the mouse into a point local to the UI canvas
        // for reference, see https://stackoverflow.com/questions/43802207/position-ui-to-mouse-position-make-tooltip-panel-follow-cursor
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition, canvas.worldCamera,
            out localMousePos);

        // use TransformPoint to get the actual correct position for the tooltip
        transform.position = canvas.transform.TransformPoint(localMousePos) + (Vector3)offset;
        
        UpdateSize();

    }

    public void UpdateSize()
    {
        Vector2 size = childRectTransform.sizeDelta;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x + paddingSize.x * 2);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y + paddingSize.y * 2);
    }

    #endregion
}
