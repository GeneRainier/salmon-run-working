using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Tower Buttons")] 
    [SerializeField] private Button anglerButton;
    [SerializeField] private Button rangerButton;
    [SerializeField] private Button damButton;
    [SerializeField] private Button salmonLadderButton;
    [SerializeField] private Button seaLionButton;
    [SerializeField] private Button truckButton;
    [SerializeField] private Button tunnelButton;

    public void Enable(TowerType tower)
    {
        Toggle(tower, true);
    }

    public void Disable(TowerType tower)
    {
        Toggle(tower, false);
    }

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
