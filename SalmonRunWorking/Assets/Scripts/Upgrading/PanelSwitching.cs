using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSwitching : MonoBehaviour
{
    public RectTransform theRectTransform;

    public void OnButtonPress ()
    {
        Debug.Log("buttonPressed");

        //theRectTransform = transform as RectTransform; // Cast it to RectTransform
        theRectTransform.SetAsLastSibling(); // Make the panel show on top.
    }
}
