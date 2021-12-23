using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class ScaleAnimationController : MonoBehaviour
{
    private Material _material;

    [SerializeField] private CreateAnimation _createAnimation;

    private float _animationSpeed;
     // Start is called before the first frame update
    void Start()
    {
        
        _material = GetComponent<MeshRenderer>().material;
        _animationSpeed = _createAnimation.SetCoordinateSpeed();
        _material.SetFloat("Vector1_f9afb7bb363c4f6497bca4f0cadf8d96", _animationSpeed);
        _material.SetFloat("Vector1_802ae354d683446db656c6577abd3a81", Time.time);
        // MaterialPropertyBlock
        // DOVirtual.Float(0, 1, _animationSpeed, value =>
        //     {
        //         _material.SetFloat("AnimationSpeed", value);
        //     });
        //.SetDelay(transform.localPosition.y / 10);
        //Debug.Log(_animationSpeed + "Shader");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            print("space");
            
        }
        
    }
}
