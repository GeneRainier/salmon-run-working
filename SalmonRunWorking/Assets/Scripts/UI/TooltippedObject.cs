using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Script for objects that will generate tooltips when the mouse is over them
 */
public class TooltippedObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // content that the tooltip will have
    public string content;
    public string cantBuy;

    private DragAndDropIcon towerIcon;
    
    private TowerUI towerUI;

    private void Start()
    {
        towerIcon = GetComponent<DragAndDropIcon>();
        towerUI = GetComponent<TowerUI>();
    }

    /**
     * Handle mouse entering an object
     */
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (!GameManager.Instance.Started || !towerIcon) return;
        bool canAfford = towerUI.CanAfford;
        Tooltip.Instance.ShowTooltip(canAfford ? content : cantBuy);
    }

    /**
     * Handle mouse exiting the object
     */
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.HideTooltip();
    }
}
