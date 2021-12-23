using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        //    TOUCH / MOUSE INPUT
        //    (Input.touchCount != 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        if (Input.GetMouseButtonDown(0))
        {
            CheckIfTouched();
        }

        //if (Input.touchCount!= 0 && Input.GetTouch(0).phase==TouchPhase.Moved)
        if (isTouching)
        {
            var swipeDirection = GetSwipeDirection();
            if (swipeDirection != SwipeDirection.Null && GameManager.Instance.allowToMove)
            {
                if (GameManager.Instance.gameState== GameState.Start)
                {
                    GameManager.Instance.gameState = GameState.Playing;
                }
                gameController.MoveObjectsTo(swipeDirection);
            }
        }

        //    TOUCH / MOUSE INPUT
        //if (Input.touchCount!= 0 && Input.GetTouch(0).phase==TouchPhase.Ended)
        if (Input.GetMouseButtonUp(0))
        {
            CheckIfTapped();
        }
    }

    private void CheckIfTapped()
    {
        //    TOUCH / MOUSE INPUT
        //_touchEndedPosition = Input.GetTouch(0).position;
        _touchEndedPosition = Input.mousePosition;
        var range = _touchBeganPosition - _touchEndedPosition;
        if (range.magnitude < tapTolerance)
        {
            Debug.Log("tapped");
            if (GameManager.Instance.gameState == GameState.Win)
            {
                GameManager.Instance.LevelNumber++;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            if (GameManager.Instance.gameState == GameState.Lose)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        isTouching = false;
    }

    public SwipeDirection GetSwipeDirection()
    {
        //_touchMovingPosition = Input.GetTouch(0).position;
        _touchMovingPosition = Input.mousePosition;

        var range = _touchMovingPosition -_touchBeganPosition;
        if (range.x > swipeRangeTolerance)
        {
            // Debug.Log("right");
            isTouching = false;
            return SwipeDirection.Right;
        }
        else if (range.x < -swipeRangeTolerance)
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
        }
        else if (range.y < -swipeRangeTolerance)
        {
            // Debug.Log("down");
            isTouching = false;
            return SwipeDirection.Down;
        }


        return SwipeDirection.Null;
    }

    private void CheckIfTouched()
    {
        //    TOUCH / MOUSE INPUT
        //_touchBeganPosition = Input.GetTouch(0).position;
        _touchBeganPosition = Input.mousePosition;
        Debug.Log("touched");
        //Debug.Log(_touchBeganPosition.ToString());
        isTouching = true;
    }
}