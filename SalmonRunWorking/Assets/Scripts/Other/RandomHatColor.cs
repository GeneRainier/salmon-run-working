using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomHatColor : MonoBehaviour
{

    public List<Color> ColorList = new List<Color>() {
        new Color(0.6037736f, 0.2274741f, 0.1281595f, 1f),
        new Color(0.509434f, 0.1417764f, 0.2814927f, 1f),
        new Color(0.5660378f, 0.1895693f, 0.4840343f, 1f),
        new Color(0.3825291f, 0.1882353f, 0.5647059f, 1f),
        new Color(0.08365966f, 0.3773585f, 0.1815592f, 1f),
        new Color(0.08365966f, 0.3773585f, 0.1815592f, 1f),
        new Color(0.08365966f, 0.3773585f, 0.1815592f, 1f),
        new Color(0.08365966f, 0.3773585f, 0.1815592f, 1f),
        new Color(0.08365966f, 0.3773585f, 0.1815592f, 1f),
        new Color(0.08365966f, 0.3773585f, 0.1815592f, 1f)};

    void Start()
    {
        int x = Random.Range(0, 10);
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", ColorList[x]);

    }

    void ApplyMaterial(Color color, int targetMaterialIndex)
    {
    }
}
