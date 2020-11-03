using UnityEngine;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{
    [SerializeField] private TowerType towerType;
    [SerializeField] private Color enabledColor = new Color32(233, 233, 233, 255);
    [SerializeField] private Color disabledColor = new Color32(108, 108, 108, 255);
    
    private float towerCost;
    private Image icon;
    
    private void Awake()
    {
        icon = GetComponent<Image>();
    }

    private void Start()
    {
        towerCost = ManagerIndex.MI.TowerManager.GetTowerCost(towerType);
    }

    public bool CanAfford => ManagerIndex.MI.MoneyManager.CanAfford(towerCost);

    public void Purchase()
    {
        ManagerIndex.MI.MoneyManager.SpendMoney(towerCost);
    }

    public void UpdateColor()
    {
        icon.color = CanAfford ? enabledColor : disabledColor;
    }
}