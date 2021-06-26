using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * Script that enlarges and shrinks buttons based on cursor actions
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class ButtonEnlarge : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 cachedScale;        //< The original scale of the element
    private Vector3 biggerScale;        //< The enlarged scale of the element

    /*
     * Start is called prior to the first frame update
     */
    void Start()
    {
        cachedScale = transform.localScale;
        biggerScale = new Vector3(0.3f, 0.5f, 0f);
    }

    /*
     * Called when the mouse pointer enters the bounds of an element
     * 
     * @param eventData The information associated with the mouse pointer
     */
    public void OnPointerEnter(PointerEventData eventData)
    {
        //transform.localScale = new Vector3(1.5f, 1.5f);
        transform.localScale = cachedScale + biggerScale;
    }

    /*
     * Called when the mouse pointer leaves the bounds of an element
     * 
     * @param eventData The information associated with the mouse pointer
     */
    public void OnPointerExit(PointerEventData eventData)
    {

        transform.localScale = cachedScale;
    }
}
