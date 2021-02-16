using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSliderButton : MonoBehaviour
{
    public GameObject LadderSlider;

    public void OnButtonPress()
    {
        Debug.Log("buttonPressed");

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
