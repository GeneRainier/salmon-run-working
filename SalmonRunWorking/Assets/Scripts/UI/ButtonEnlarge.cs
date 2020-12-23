using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEnlarge : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 cachedScale;
    Vector3 biggerScale;

    void Start()
    {

        cachedScale = transform.localScale;
        biggerScale = new Vector3(0.3f, 0.5f, 0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        //transform.localScale = new Vector3(1.5f, 1.5f);
        transform.localScale = cachedScale + biggerScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        transform.localScale = cachedScale;
    }
}
