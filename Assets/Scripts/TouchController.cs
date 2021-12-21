using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SwipeDirection
{
    Right,
    Down,
    Left,
    Up,
    Null
}
public class TouchController : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private float swipeRangeTolerance;
    [SerializeField] private float tapTolerance;
    private Vector2 _touchBeganPosition;
    private Vector2 _touchMovingPosition;
    private Vector2 _touchEndedPosition;
    public bool isTouching;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("test");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount != 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckIfTouched();
        }
        //if (Input.touchCount!= 0 && Input.GetTouch(0).phase==TouchPhase.Moved)
        if(isTouching)
        {
            var swipeDirection = GetSwipeDirection();
            if (swipeDirection != SwipeDirection.Null)
            {
                gameController.MoveObjectsTo(swipeDirection);
            }
        }
        if (Input.touchCount!= 0 && Input.GetTouch(0).phase==TouchPhase.Ended)
        {
            CheckIfTapped();
        }
    }

        private void CheckIfTapped()
        {
            _touchEndedPosition = Input.GetTouch(0).position;
            //Debug.Log("released");
            Debug.Log(_touchEndedPosition.ToString());
            var range = _touchBeganPosition - _touchEndedPosition;
            if (range.magnitude < tapTolerance)
            {
                Debug.Log("tapped");
            }
            isTouching = false;
        }

    public SwipeDirection GetSwipeDirection()
    {
        
        _touchMovingPosition = Input.GetTouch(0).position;
        //Debug.Log("touching");
        //Debug.Log(touchMovingPosition.ToString());
        var range = _touchMovingPosition - _touchBeganPosition;
        if (range.x > swipeRangeTolerance)
        {
            // Debug.Log("right");
            isTouching = false;
            return SwipeDirection.Right;
        }else if (range.x < -swipeRangeTolerance)
        {
            // Debug.Log("left");
            isTouching = false;
            return SwipeDirection.Left;
        }
        else if (range.y > swipeRangeTolerance)
        {
            // Debug.Log("up");
            isTouching = false;
            return SwipeDirection.Up;
        }else if(range.y < -swipeRangeTolerance)
        {
            // Debug.Log("down");
            isTouching = false;
            return SwipeDirection.Down;
        }

        
        
        return SwipeDirection.Null;
    } 

    private void CheckIfTouched()
    {
            _touchBeganPosition = Input.GetTouch(0).position;
            Debug.Log("touched");
            Debug.Log(_touchBeganPosition.ToString());
            isTouching = true;
    }
}

