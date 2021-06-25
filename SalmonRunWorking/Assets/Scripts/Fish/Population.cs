using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class that manages the immediate population of salmon using generation utility functions
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class Population : MonoBehaviour {

    public GameObject MemberPrefab;
    public int PopSize;
    private int Generation;
    private List<GameObject> Members;
    public float GenerationLength;
    
	// Start is called before the first frame update
	void Start () {
        NewGeneration();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /*
     * Takes the old generation of fish and creates a new generation based on the older one
     */
    private void NewGeneration()
    {
        List<GameObject> OldMembers = Members;
        Members = new List<GameObject>();

        for (int i = 0; i < PopSize; i++)
        {
            Members.Add(Instantiate(MemberPrefab));
        }

        if (OldMembers != null)
        {
            foreach (GameObject Member in OldMembers)
            {
                Destroy(Member);
            }
        }

        StartCoroutine(GenerationCoroutine());
    }

    private IEnumerator GenerationCoroutine()
    {
        yield return new WaitForSeconds(GenerationLength);
        NewGeneration();
    }
}
