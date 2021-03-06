using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Class that manages how fish should appear based on their genomes
 * 
 * Authors: Benjamin Person (Editor 2020)
 */

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class FishAppearance : MonoBehaviour{

	public Transform[] bones;           //< Array of components that make up a fishes appearance
    
	Renderer rend;                      //< Reference to the Scene renderer
	SkinnedMeshRenderer blendRenderer;  //< Reference to the mesh renderer for the fish

    /*
     * Awake is called after the initialization of all gameObjects prior to the start of the game
     */
	private void Awake (){
		rend = GetComponent<Renderer> ();
		blendRenderer = GetComponent<SkinnedMeshRenderer> ();
	}

    /**
     * Set the apperanace of the fish
     * 
     * @param genome FishGenome The genome that will determine appearance
     */
	public void SetAppearance(FishGenome genome) {
		SetTints (genome);
		// ResizeBones(genome);
		// setBlendWeights(genome);
	}

    /**
     * Set the tints for all parts of the fish
     * 
     * @param genome FishGenome The genome that will determine what tints are applied
     */
	private void SetTints (FishGenome genome){

        // Temporary colors to indicate sex and size
        Color color1 = genome.IsMale() ? Color.blue : Color.magenta;

        Color color2;

        if(genome[FishGenome.GeneType.Size].dadGene != genome[FishGenome.GeneType.Size].momGene)
        {
            color2 = Color.yellow;
        }
        else if (genome[FishGenome.GeneType.Size].momGene == FishGenome.B)
        {
            color2 = Color.red;
        }
        else
        {
            color2 = Color.green;
        }

		rend.material.SetColor("_Color", color1);
		rend.material.SetColor("_Color2", color2);
		//rend.material.SetColor("_Color3", GetAColor());
		//rend.material.SetColor("_Color4", GetAColor());
		
	}

    /**
     * Get a random color
     * 
     * @return Color A random color
     */
	private Color GetAColor() {
		float h = Mathf.Pow(Random.Range(0f,1.0f),2);
        float s = 1.0f;						
		float v = Mathf.Pow(Random.Range(0.5f,1f),2);	
		return Color.HSVToRGB(h,s,v);
	}

    /**
     * Set the size of the fish's bones
     * 
     * @param genome FishGenome The genome that will determine how the bones are sized
     */
	private void ResizeBones(FishGenome genome)
    {
		for (int i=0; i<bones.Length-1; i+=1){
			var a = Random.Range(0.5f,1.5f);
			bones [i].localScale = new Vector3 (a, a, a);
		}
	}

    /**
     * Set the fish's blend shape weights
     * 
     * @param genome FishGenome The genome that will determine how the blend shape weights are set
     */
	private void setBlendWeights(FishGenome genome)
    {
		blendRenderer.SetBlendShapeWeight(0,Random.Range(0f,100f));
	}
	
    /**
     * Get a random boolean (true or false)
     * 
     * @return bool 50/50 true/false
     */
	private bool randomBool (){
		return (Random.value > 0.5f);
	}
}