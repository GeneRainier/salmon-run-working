using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/**
 * Class that controls a three-part bar graph
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class HorizontalBarGraph : MonoBehaviour
{
    [SerializeField] private List<GraphEntry> dataEntries = null;      //< The entries for each set of data (sex, size, state)

    private float graphWidth;               //< How wide the entirety of the graph is

    /**
     * Awake is called after the initialization of gameobjects prior to the start of the game. This is used as an Initialization Function
     */
    private void Awake()
    {
        // Make sure we have minimum number of bars
        if (dataEntries.Count < 2)
        {
            Debug.LogError("Less than two bars or numeric displays or bar titles assigned to a horiz bar graph!");
        }

        // The graph width is taken from how wide the rightmost bar/image is in the scene
        // The bars overlap each other, so the width of the right bar is the width of the whole graph
        graphWidth = dataEntries.Last().Location.sizeDelta.x;
    }

    /**
     * Update the graph on-screen
     * 
     * Params for this function are a little tricky because we want ensure that 2 OR MORE values are provided
     * 
     * @param firstValue int The value corresponding to the first bar
     * @param secondValue in The value corresponding to the second bar
     * @param otherValues params int[] Values corresponding to the rest of the bars
     */
    public void UpdateGraph(params int[] values)
    {
        // If we have more values than bars, issue a warning
        // We will continue onwards while ignoring the extra values since we can't show them
        // but the warning should inform the programmer that something's up
        if (values.Length != dataEntries.Count)
        {
            Debug.LogError("Number of values supplied to graph does not match number of bars");
        }

        // Figure out the total sum of all of the values (not counting any extra values we aren't displaying on-screen
        // If there is no corresponding value (because too few values were provided), just assume zero
        float totalSum = values.Sum();
        
        // Actually update the graph, looping through each bar
        float previousSum = 0;
        foreach (var property in dataEntries.Zip(values, Tuple.Create))
        {
            // All bars are (should be) left anchored at the same position, and the left parts of the bar draw over the right parts of the bar
            // So, we figure out how much the combined value of this bar and all previous bars
            float currentSum = previousSum + property.Item2;
            
            // Bar length is set to the a proportion of the full graph length corresponding to the current sum over the total sum
            property.Item1.Location.sizeDelta = new Vector2(currentSum / totalSum * graphWidth, property.Item1.Location.sizeDelta.y);
            
            // Keep track of how big this bar was for use in the next bar
            previousSum = currentSum;
            
            // Also update the numeric display text
            property.Item1.SetText(property.Item2);
        }
    }

    /*
     * Nested class describing one of the types of data entry (sex, size, state)
     */
    [Serializable]
    public class GraphEntry
    {
        [SerializeField] private string name = null;       //< Entry title

        [SerializeField] private RectTransform location = null;    //< Rect transform array for bars/images representing the category being displayed on the left, center, and right of the graph

        [SerializeField] private TextMeshProUGUI text = null;      //< Text to display the numeric value of each bar

        public string Title => name;

        public RectTransform Location => location;

        /*
         * Setter for the text in the data entry
         * 
         * @param value The data entry value
         */
        public void SetText(int value)
        {
            text.text = $"{Title}\n({value})";
        }
    }
}
