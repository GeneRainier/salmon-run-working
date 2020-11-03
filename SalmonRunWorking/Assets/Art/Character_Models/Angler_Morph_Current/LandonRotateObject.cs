using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandonRotateObject : MonoBehaviour {
	public Vector3 angle = new Vector3(0, 0, 0);
	
	void Update () {
		transform.Rotate(angle * Time.deltaTime);
	}
}