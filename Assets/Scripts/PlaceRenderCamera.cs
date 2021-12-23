using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceRenderCamera : MonoBehaviour
{
    private GameObject _hole;
    private Vector3 holePos;
    private Vector3 distance;
    public Vector3 pivotDiff;

    // Start is called before the first frame update
    void Start()
    {
        _hole = GameObject.FindGameObjectWithTag("Hole");
        holePos = _hole.transform.position;
        Debug.Log("FindGameObjectWithTag" + pivotDiff);
        distance = holePos - transform.position-pivotDiff;
    }

    // Update is called once per frame
    void Update()
    {
        holePos = _hole.transform.position;
        var transform1 = transform.position;
        transform1 = new Vector3(-holePos.x-distance.x,holePos.y-distance.y,transform1.z);
        transform.position = transform1;
    }
}
