using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class CreateAnimation : MonoBehaviour
{
    [SerializeField] private float animationSpeed = 5f;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private Vector3 animationDistance = new Vector3(0, 0, -3);
    [SerializeField] private bool animateMove;
    //[SerializeField] private bool animateScale;
    
    private float coordinateSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        //transform.position = transform.position - animationDistance;
        var parentPositionY = transform.parent.localPosition.y;
        var _yCoordinateSpeed = SetCoordinateSpeed();
        
        if (animateMove)
        {
            transform.position -= animationDistance;
            transform.DOMove(transform.position+ animationDistance, animationDuration);
        }
        
        //Used shader instead of DOtween, check ScaleAnim instead of URP:Lit
        /*if (animateScale)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one,_yCoordinateSpeed);
        }*/
    }

    public float SetCoordinateSpeed( )
    {
        var absPositionY = transform.localPosition.y;
        var absPositionX = transform.localPosition.x;
        var PositionV2 = new Vector2(absPositionX,absPositionY).magnitude;
        // _yCoordinateSpeed = 1/((-positionY+.5f) * animationSpeed); 
        Debug.Log("PositionV2 " + PositionV2);
        coordinateSpeed = ((PositionV2+2)*animationSpeed*1.5f);
       // Debug.Log(_yCoordinateSpeed + "-" + positionY);
        return coordinateSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
