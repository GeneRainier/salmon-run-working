using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Manager script to handle the various UI elements such as buttons and panels
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class MenuManager : MonoBehaviour
{
    // The buttons along the tower panel in the UI
    [Header("Tower Buttons")] 
    [SerializeField] private Button anglerButton = null;
    [SerializeField] private Button rangerButton = null;
    [SerializeField] private Button damButton = null;
    [SerializeField] private Button salmonLadderButton = null;
    [SerializeField] private Button seaLionButton = null;
    [SerializeField] private Button truckButton = null;
    [SerializeField] private Button tunnelButton = null;

    /*
     * Enables the towers of a particular type
     * 
     * @param tower The type of tower we are enabling
     */
    public void Enable(TowerType tower)
    {
        Toggle(tower, true);
    }

    /*
     * Disables the towers of a particular type
     * 
     * @param tower The type of tower we are enabling
     */
    public void Disable(TowerType tower)
    {
        Toggle(tower, false);
    }

    /*
     * Toggles the button for a type of tower based on whether it is enabled currently
     * 
     * @param tower The type of tower we are enabling the buttons for
     * @param toggle Whether we are enabling or disabling
     */
    public void Toggle(TowerType tower, bool toggle)
    {
        switch (tower)
        {
            case TowerType.Angler:
                anglerButton.interactable = toggle;
                break;
            case TowerType.Ranger:
                rangerButton.interactable = toggle;
                break;
            case TowerType.Dam:
                damButton.interactable = toggle;
                break;
            case TowerType.Ladder:
                salmonLadderButton.interactable = toggle;
                break;
            case TowerType.SeaLion:
                seaLionButton.interactable = toggle;
                break;
            case TowerType.Truck:
                truckButton.interactable = toggle;
                break;
            case TowerType.Tunnel:
                tunnelButton.interactable = toggle;
                break;
        }
    }
}
