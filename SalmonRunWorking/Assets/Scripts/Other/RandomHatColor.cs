using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomHatColor : MonoBehaviour
{

    public List<Color> ColorList = new List<Color>() {
        Color.red,
        Color.black,
        Color.cyan,
        Color.gray,
        Color.green,
        Color.magenta,
        Color.white,
        Color.yellow,
        Color.blue,
        new Color(0.3f, 0.4f, 0.6f, 0.3f)};

    void Start()
    {
        int x = Random.Range(0, 10);
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", ColorList[x]);

    }

    void ApplyMaterial(Color color, int targetMaterialIndex)
    {
    }
}
