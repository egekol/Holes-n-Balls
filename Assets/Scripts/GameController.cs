using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private LevelService levelService;

    [SerializeField] private GameObject ball;
    //public List<GameObject> balls;
    public List<GameObject> objectList;
    public GameObject currentObject;
    // public bool allowToMove = true;
    private int animationCount;
    public int[,] holeCoordinate;

    

    public void MoveObjectsTo(SwipeDirection swipeDirection)
    {
        
        var items = levelService.GetBallPathLength(swipeDirection);
        var positionDictionary = items.Item1;
        var pathLength = items.Item2;

        int index = 0;
        var lastPair = positionDictionary.Count;//positionDictionary.Keys.Last();
        foreach (var pair in positionDictionary)
        {
            float duration = (pathLength[index].magnitude/20)+ .1f;
            pair.Key.transform.DOLocalMove(pair.Value,duration).SetEase(Ease.InQuad).OnComplete(() =>
            {
                pair.Key.transform.DOScale(.8f, .1f).SetEase(Ease.InOutSine).SetLoops(2,LoopType.Yoyo).OnComplete(() =>
                {
                    animationCount++;
                    if (animationCount == lastPair)
                        AnimationOnComplete();
                });
            });
            
            GameManager.Instance.allowToMove = false;
            void AnimationOnComplete()
            {
                if (GameManager.Instance.gameState == GameState.Playing)
                {
                    GameManager.Instance.allowToMove = true;
                }
                animationCount = 0;
            }
            index++;
        }

    }




    public void BallsInHoles(GameObject o , Vector2 oldCoord)
    {
        Debug.Log("Moving Ball is in the hole: " + o + " " + oldCoord);
        o.tag = "Ball In Hole";
        Destroy(o);
    }
}
