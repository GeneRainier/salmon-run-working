using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/**
 * Class that controls a three-part bar graph
 */
public class HorizontalBarGraph : MonoBehaviour
{
    [SerializeField] private List<GraphEntry> dataEntries;

    // how wide the entirety of the graph is
    private float graphWidth;

    /**
     * Initialization function
     */
    private void Awake()
    {
        // make sure we have minimum number of bars
        if (dataEntries.Count < 2)
        {
            Debug.LogError("Less than two bars or numeric displays or bar titles assigned to a horiz bar graph!");
        }

        // the graph width is taken from how wide the rightmost bar/image is in the scene
        // the bars overlap each other, so the width of the right bar is the width of the whole graph
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
        // if we have more values than bars, issue a warning
        // we will continue onwards while ignoring the extra values since we can't show them
        // but the warning should inform the programmer that something's up
        if (values.Length != dataEntries.Count)
        {
            Debug.LogError("Number of values supplied to graph does not match number of bars");
        }

        // figure out the total sum of all of the values (not counting any extra values we aren't displaying on-screen
        // if there is no corresponding value (because too few values were provided), just assume zero
        float totalSum = values.Sum();
        
        // actually update the graph, looping through each bar
        float previousSum = 0;
        foreach (var property in dataEntries.Zip(values, Tuple.Create))
        {
            // all bars are (should be) left anchored at the same position, and the left parts of the bar draw over the right parts of the bar
            // so, we figure out how much the combined value of this bar and all previous bars
            float currentSum = previousSum + property.Item2;
            
            // bar length is set to the a proportion of the full graph length corresponding to the current sum over the total sum
            property.Item1.Location.sizeDelta = new Vector2(currentSum / totalSum * graphWidth, property.Item1.Location.sizeDelta.y);
            
            // keep track of how big this bar was for use in the next bar
            previousSum = currentSum;
            
            // also update the numeric display text
            property.Item1.SetText(property.Item2);
        }
    }

    [Serializable]
    public class GraphEntry
    {
        // Entry title
        [SerializeField] private string name;
        
        // rect transform array for bars/images representing the category being displayed on the left, center, and right of the graph
        [SerializeField] private RectTransform location;

        // Text to display the numeric value of each bar
        [SerializeField] private TextMeshProUGUI text;

        public string Title => name;

        public RectTransform Location => location;

        public void SetText(int value)
        {
            text.text = $"{Title}\n({value})";
        }
    }
}
