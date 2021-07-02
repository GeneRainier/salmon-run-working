using UnityEngine;
using UnityEngine.UI;

/*
 * Class that describes the UI for each UI element
 * 
 * Authors: Benjamin Person (Editor 2020)
 */ 
public class TowerUI : MonoBehaviour
{
    [SerializeField] private TowerType towerType = TowerType.Angler;             //< The type of tower this is UI for
    [SerializeField] private Color enabledColor = new Color32(233, 233, 233, 255);      //< The color of an enabled UI element
    [SerializeField] private Color disabledColor = new Color32(108, 108, 108, 255);     //< The color of a disabled UI element

    // Determines if the tower can be placed by a player based on the TowerType
    public bool isPlaceableTower => towerType == TowerType.Angler || towerType == TowerType.Dam ||
                                    towerType == TowerType.Ladder || towerType == TowerType.Ranger;

    public TowerType TowerType => towerType;

    private float towerCost;        //< The cost of this tower
    private Image icon;             //< The UI icon for this type of tower
    
    /*
     * Awake is called after the initialization of gameobjects prior to the start of the game. 
     */
    private void Awake()
    {
        icon = GetComponent<Image>();
    }

    /*
     * Start is called prior to the first frame update
     */
    private void Start()
    {
        towerCost = ManagerIndex.MI.TowerManager.GetTowerCost(towerType);
    }

    public bool CanAfford => ManagerIndex.MI.MoneyManager.CanAfford(towerCost);     //< Determines if the tower is affordable based on the cost

    /*
     * Spends the necessary money to place a tower
     */
    public void Purchase()
    {
        ManagerIndex.MI.MoneyManager.SpendMoney(towerCost);
    }

    /*
     * Disables the button by switching the icon color
     */
    public void TurnColorOff()
    {
        icon.color = disabledColor;
    }

    /*
     * Enables the color of the UI button based on if the tower is affordable by the player
     */
    public void UpdateColor()
    {
        icon.color = CanAfford ? enabledColor : disabledColor;
    }
}