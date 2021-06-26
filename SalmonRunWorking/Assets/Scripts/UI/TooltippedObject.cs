using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Script for objects that will generate tooltips when the mouse is over them
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class TooltippedObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string content;          //< Content that the tooltip will have
    public string cantBuy;          //< Content that says the tower cannot be purchased

    private DragAndDropIcon towerIcon;      //< The icon that represents the tower in the UI
    
    private TowerUI towerUI;                //< The UI for this particular object

    /*
     * Start is called prior to the first frame update
     */
    private void Start()
    {
        towerIcon = GetComponent<DragAndDropIcon>();
        towerUI = GetComponent<TowerUI>();
    }

    /**
     * Handle mouse entering an object
     * 
     * @param eventData The information coming from the mouse pointer
     */
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (!GameManager.Instance.Started || !towerIcon) return;
        bool canAfford = towerUI.CanAfford;
        Tooltip.Instance.ShowTooltip(canAfford ? content : cantBuy);
    }

    /**
     * Handle mouse exiting the object
     * 
     * @param eventData The information coming from the mouse pointer
     */
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.HideTooltip();
    }
}
