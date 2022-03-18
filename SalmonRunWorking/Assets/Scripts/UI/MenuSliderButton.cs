using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script controlling the salmon ladder sub menu
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class MenuSliderButton : MonoBehaviour
{
    public GameObject LadderSlider;     //< The submenu slider in the UI

    /*
     * On a button press, the ladder submenu will slide up or down to open or close for the player
     */
    public void OnButtonPress()
    {
        if(LadderSlider != null)
        {
            Animator animator = LadderSlider.GetComponent<Animator>();

            if (animator != null)
            {
                bool isOpen = animator.GetBool("SlideOut");
                animator.SetBool("SlideOut", !isOpen);
            }
        }
    }
}
