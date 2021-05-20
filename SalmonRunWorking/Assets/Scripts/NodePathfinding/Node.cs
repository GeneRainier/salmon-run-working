using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector3 position;    //< The position of this node
    public List<Node> nextNodes;    //< All the nodes that can be travelled to from this node
    public bool endNode = false;    //< Boolean representing if this is the final node in the path

    void Start()
    {
        position = this.gameObject.transform.position;
    }
}
