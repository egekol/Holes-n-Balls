using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public enum GameState
{
    Start,
    Lose,
    Playing,
    Win,
}

public class GameManager : MonoBehaviour
{
    // [SerializeField] private LevelService _levelService;
    [SerializeField] private List<GameObject> _ballList;
    [SerializeField] private GameState _gameState;
    [SerializeField] private GameObject _eatenBall;
    [field: SerializeField] public int CoinsLeft ;
    public int _coinNumber;
    private static GameManager instance;
    public static GameManager Instance => instance;

    public List<Object> LevelList;
    public int LevelNumber;
    public bool allowToMove = true;

    public GameState gameState
    {
        get => _gameState;
        set
        {
            UpdateState(value);
            _gameState = value;
        }
    }

    public Object CurrentLevel
    {
        get => LevelList[LevelNumber];
    }

    public List<GameObject> ballList
    {
        get => _ballList;

    }

    public GameObject EatenBall
    {
        set
        {
            CheckWinLostCondition(ballList, value);
            _eatenBall = value;
        }
    }


    private void UpdateState(GameState value)
    {
        switch (value)
        {
            case GameState.Start:
                allowToMove = true;
                break;
            case GameState.Lose:
                allowToMove = false;
                break;
            case GameState.Playing:
                allowToMove = true;
                break;
            case GameState.Win:
                allowToMove = false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        LevelNumber = PlayerPrefs.GetInt("levelNumber", 0);
        LevelList = Resources.LoadAll("", typeof(TextAsset)).ToList();
    }
    
    private void CheckWinLostCondition(List<GameObject> ballList, GameObject ball)
    {
        this.ballList.Remove(ball);
        Debug.Log(ball.name);
        if (ballList.Count >= 1 && ball.name == "Ball_White(Clone)"
                                && Instance.gameState == GameState.Playing)
        {
            Instance.gameState = GameState.Lose;
        }
        else if (!ballList.Exists(i => i.CompareTag("Ball"))
                 && Instance.gameState == GameState.Playing)
        {
            Instance.gameState = GameState.Win;
            var stars = GetCoinResult();
            Debug.Log("stars: " + stars);
        }
    }

    private int GetCoinResult()
    {
      var result = Instance._coinNumber-Instance.CoinsLeft;
      if (result / Instance._coinNumber == 1)
      {
          return 3;
      }if (result / Instance._coinNumber >.5f)
      {
          return 2;
      }
      else
      {
          return 1;
      }
    }
}