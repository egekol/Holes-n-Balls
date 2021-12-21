using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private LevelService _levelService;
    public List<GameObject> balls;
    public List<GameObject> hole;
    public int[,] holeCoordinate;

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log(balls[0] +":test: ");
        var lineGrid = _levelService.lineGrid;
        //Debug.Log(_levelService.ballsCoordinate);
        //Debug.Log(lineGrid[(int)balls[0].transform.localPosition.x,(int)balls[0].transform.localPosition.y]);
        Debug.Log(balls[0].transform.localPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveObjectsTo(SwipeDirection swipeDirection)
    {
        var getPosition = _levelService.GetBallPathLength(swipeDirection);

        for (int i = 0; i < getPosition.Count; i++)
        {
            _levelService.objList[i].transform.DOLocalMove(getPosition[i], .5f).SetEase(Ease.InCubic);
        }

        //balls[0].transform.localPosition = getPosition[0];//new Vector3(getPosition[0].x,getPosition[0].y,balls[0].transform.position.z);

        /*var lineGrid = _levelService.lineGrid;
        
        Debug.Log(lineGrid[(int)balls[0].transform.position.x,(int)balls[0].transform.position.y]);
        Debug.Log(balls[0].transform.position);*/
    }
}
