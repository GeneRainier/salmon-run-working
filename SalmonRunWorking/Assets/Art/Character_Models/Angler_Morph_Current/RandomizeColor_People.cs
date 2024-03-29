using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomizeColor_People : MonoBehaviour
{
	[Header("Color Properties")]
	[SerializeField] private bool useColorPreset = false;
	[SerializeField] private ColorPreset colorPreset;

	[Header("Model Properties")]
	[SerializeField] private Transform[] bones;
	[SerializeField] private SkinnedMeshRenderer skinRenderer = null;

	[SerializeField] private bool[] randomizeValue = 
		new[] {false, false, false, false};

	private delegate Color ColorFunction();
	
	private Dictionary<string, ColorFunction> colorFunctions =
		new Dictionary<string, ColorFunction>()
	{
		{ "_PaintR", GenerateClothColor },
		{ "_PaintG", GenerateHairColor },
		{ "_PaintB", GenerateSkinColor },
		{ "_PaintA", GenerateClothColor },
	};

	private void Awake()
	{
        //InvokeRepeating("Randomize", 1f,0.5f);
        Randomize();

        if (useColorPreset && !colorPreset)
        {
	        colorPreset = ManagerIndex.MI.ColorManager.GetRandomPreset();
        }
	}
	
	private void Randomize()
	{
        SetColor();
		//ResizeBones();
		SetBlendWeights();
	}
	
	private void SetColor()
	{
		if (useColorPreset && colorPreset)
		{
			foreach (Tuple<string, Color> property in colorFunctions.Keys.Zip(colorPreset.GetValues(), Tuple.Create))
			{
				skinRenderer.material.SetColor(property.Item1, property.Item2);
			}
			return;
		}
		foreach (KeyValuePair<string, ColorFunction> property in colorFunctions)
		{
			skinRenderer.material.SetColor(property.Key, property.Value());
		}
	}

	private static Color GenerateSkinColor() 
	{
		float h = Random.Range(0.04f,0.09f);
        float v = 1f - (Mathf.Pow(Random.value, 2) * 0.6f + 0.1f);
        float s = 1f - v + 0.2f;
		return Color.HSVToRGB(h,s,v);
	}
	
    private static Color GenerateHairColor()
    {
        float h = Random.Range(0.04f, 0.09f);
        float v = 1f - Mathf.Pow(Random.value, 2);
        float s = Random.value;
        return Color.HSVToRGB(h, s, v);
    }
    
    private static Color GenerateClothColor()
    {
        float h = Random.value;
        float s = Random.Range(0f, 0.7f);
        float v = Random.Range(0.5f, 1f);
        return Color.HSVToRGB(h, s, v);
    }

    /*private void ResizeBones()
     {
		for (int i=0; i<bones.Length-1; i+=1)
		{
			var a = Random.Range(0.5f,1.5f);
			bones [i].localScale = new Vector3 (a, a, a);
		}
	}
    */

    // Set the Body Weight/Size
    private void SetBlendWeights()
    {
        float malePercent = Mathf.Pow(Random.value,1.5f);
        float weightPercent = Mathf.Pow(Random.value,2f);
        float maxWeight = 1 - malePercent;
        float weight = maxWeight * weightPercent;
        skinRenderer.SetBlendShapeWeight(0, malePercent * 100f);
        skinRenderer.SetBlendShapeWeight(1, weight * 100f);
    }
}