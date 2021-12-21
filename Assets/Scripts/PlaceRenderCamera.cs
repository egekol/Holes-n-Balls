using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceRenderCamera : MonoBehaviour
{
    [SerializeField] private GameObject _hole;

    private Vector3 distance;

    // Start is called before the first frame update
    void Start()
    {
        distance = _hole.transform.position - transform.position;
        //var cameraPosition = GetComponent<Camera>().transform;
        //_camera.transform.position = transform.position + new Vector3(0,0,-15);
    }

    // Update is called once per frame
    void Update()
    {
        var newDistance = _hole.transform.position - transform.position;
        transform.position = transform.position + (distance - newDistance);
    }
}
