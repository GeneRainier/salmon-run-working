using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Script that controls the bottom UI tower selection bar
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
[RequireComponent(typeof(RectTransform))]
public class BottomBarController : MonoBehaviour
{
    [Header("Expand/Contract")]
    public float expandContractDistance;        //< The distance up and down the bar will move when the user presses the expand/contract button

    [Header("Content Page Buttons")]
    public Button contentPageButtonPrefab;      //< Prefab for buttons that will be used to switch between content pages

    public Vector2 initialButtonPosition;       //< Initial position of the first button

    public Vector2 distanceBetweenButtons;      //< Spacing between buttons

    private RectTransform rectTransform;        //< Bottom bar's rect transform

    private bool expanded = true;               //< Is the bottom bar currently expanded?

    private BottomBarContentPage[] contentPages;    //< List of all content pages that can be shown in the bottom bar

    private int activeContentPageIndex;         //< Index of currently active content page

    /**
     * Awake is called after the initialization of gameobjects prior to the start of the game. This is used as an Initialization Function
     */
    private void Awake()
    {
        // Get component refs
        rectTransform = GetComponent<RectTransform>();
    }

    /**
     * Start is called before the first frame update
     */
    private void Start()
    {
        // Get all content pages
        contentPages = GetComponentsInChildren<BottomBarContentPage>();

        // Set up the content pages
        SetupContentPageButtons();

        // Activate the first one
        SetActivePage(0);
    }

    /**
     * Handle expand/contract button being pressed
     */
    public void ExpandContractButtonPressed()
    {
        // Move the panel up or down, depending on whether we want to expand or "contract"
        float moveDistance = expanded ? -1 * expandContractDistance: expandContractDistance;
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + moveDistance);

        // Toggle our flag in code for whether we're expaned or not
        expanded = !expanded;
    }

    /**
     * Setup buttons for our content pages
     */
    private void SetupContentPageButtons()
    {
        // Make a button for each content page
        for (int i = 0; i < contentPages.Length; i++)
        {
            // Instantiate the button
            Button button = Instantiate(contentPageButtonPrefab, transform);

            // Place the button in the correct position
            RectTransform buttonRectTransform = button.GetComponent<RectTransform>();
            buttonRectTransform.anchoredPosition = initialButtonPosition + (i * distanceBetweenButtons);

            // Set the button text
            button.GetComponentInChildren<Text>().text = contentPages[i].pageTitle;

            // Tell the button what to do when it is clicked on
            int temp = i; // Need a temp var because i will always end up evaluating to the last value in the for loop
            button.onClick.AddListener(() => SetActivePage(temp));

        }
    }

    /**
     * Handle one of the buttons being clicked
     * 
     * @param index Page index of the page we want to set active
     */
    private void SetActivePage(int index)
    {
        // Set our index so we can keep track of which page is active
        activeContentPageIndex = index;

        // Loop through all content page
        // Turn on the newly activated page and deactivate all others
        for(int i = 0; i < contentPages.Length; i++)
        {
            contentPages[i].Active = (activeContentPageIndex == i);
        }

        // Make sure the bar is expanded
        if (!expanded)
        {
            ExpandContractButtonPressed();
        }
    }
    
}
