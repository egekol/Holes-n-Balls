using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

enum Tile
{
    Floor = 0,
    Wall = 1,
    Ball = 2,
    Hole = 3,
    Spike
}

public class LevelService : MonoBehaviour
{
    [SerializeField] private GameController _gameController;
    public int[,] objectGrid;
    public int[,] lineGrid;
    public List<GameObject> tileList;
    public List<GameObject> objList;
    public List<Vector3> objCoordinateList;
   
    public List<Vector2> ballsCoordinate;
    public Vector2 holeCoordinate;


    // Start is called before the first frame update
    void Start()
    {
        /*tileList.Add(floor);
        tileList.Add(rock);
        tileList.Add(ball);*/
        Debug.Log(tileList[(int) Tile.Floor]);
        lineGrid = ExportLevel();
        objectGrid = ExportObject();

        RenderBlock(lineGrid);
        RenderObject(objectGrid);

        float xVal = -objectGrid.GetLength(1) / 2f + .5f;
        float yVal = objectGrid.GetLength(0) / 2f + .5f;
        transform.position = new Vector3(xVal, yVal, 0);
    }

    private int[,] ExportObject()
    {
        //recources load all
        var levelText = Resources.Load<TextAsset>("Levels/level02").text;
        var lines = levelText.Split("\n"[0]);
        var lineDataWidth = (lines[0].Trim()).Split(";"[0]).Length;

        objectGrid = new int[lines.Length - 1, lineDataWidth];
        //ballsCoordinate= new int[lines.Length-1,lineDataWidth];
        //_gameController.holeCoordinate= new int[lines.Length-1,lineDataWidth];
        for (int i = 0; i < lines.Length - 1; i++)
        {
            var lineData = (lines[i].Trim()).Split(";"[0]);
            for (int j = 0; j < lineData.Length; j++)
            {
                int.TryParse(lineData[j], out objectGrid[i, j]);
            }
        }


        return objectGrid;
    }

    private int[,] ExportLevel()
    {
        var levelText = Resources.Load<TextAsset>("Levels/level02").text;
        var lines = levelText.Split("\n"[0]);
        var lineDataWidth = (lines[0].Trim()).Split(";"[0]).Length;
        lineGrid = new int[lines.Length - 1, lineDataWidth];
        //ballsCoordinate= new int[lines.Length-1,lineDataWidth];
        //_gameController.holeCoordinate= new int[lines.Length-1,lineDataWidth];
        for (int i = 0; i < lines.Length - 1; i++)
        {
            var lineData = (lines[i].Trim()).Split(";"[0]);
            for (int j = 0; j < lineData.Length; j++)
            {
                if (lineData[j] != "0" && lineData[j] != "1")
                {
                    int.TryParse("0", out lineGrid[i, j]);
                }
                else
                {
                    int.TryParse(lineData[j], out lineGrid[i, j]);
                }
            }
        }

        return lineGrid;
    }


    public void RenderBlock(int[,] levelGrid)
    {
        for (int i = 0; i < levelGrid.GetLength(0); i++)
        {
            for (int j = 0; j < levelGrid.GetLength(1); j++)
            {
                GameObject prefab = tileList[levelGrid[i, j]];
                Vector3 position = new Vector3(j, -i, transform.position.z);
                Quaternion rotatiton = transform.rotation;
                Transform parent = transform;
                if (levelGrid[i,j] == (int)Tile.Floor )
                {
                    Instantiate(prefab, position, rotatiton, parent);
                }
                else if(levelGrid[i,j] == (int)Tile.Wall || levelGrid[i,j] == (int)Tile.Spike)
                {
                    Instantiate(prefab, position, rotatiton, parent);
                    Instantiate(tileList[(int)Tile.Floor], position, rotatiton, parent);
                }
            }
        }

        /*
        transform.position = new Vector3(-levelGrid.GetLength(1) / 2f+.5f, levelGrid.GetLength(0) / 2f+.5f, 0);
    */
    }

    private void RenderObject(int[,] objectGrid)
    {
        for (int i = 0; i < objectGrid.GetLength(0); i++)
        {
            for (int j = 0; j < objectGrid.GetLength(1); j++)
            {
                if (objectGrid[i, j] != 0 && objectGrid[i, j] != 1)
                {
                    var tile = Instantiate(tileList[objectGrid[i, j]], new Vector3(j, -i, transform.position.z),
                        transform.rotation, transform);

                    if (objectGrid[i, j] == 2)
                    {
                        ballsCoordinate.Add(new Vector2(j, i));
                        SaveBallObject(tile);
                    }

                    if (objectGrid[i, j] == 3)
                    {
                        holeCoordinate = new Vector2(j, i);
                        SaveHoleObject(tile);
                    }
                }
            }
        }
    }

    private void SaveHoleObject(GameObject tile)
    {
        _gameController.hole.Add(tile);
    }

    private void SaveBallObject(GameObject tile)
    {
        _gameController.balls.Add(tile);
        Debug.Assert(ballsCoordinate != null, "ballsCoordinate == null");
        Debug.Log(ballsCoordinate[0] + "balllz");
    }


    // Update is called once per frame
    void Update()
    {
    }

    public List<Vector3> GetBallPathLength(SwipeDirection swipeDirection)
    {
        objCoordinateList.Clear();
        objList = _gameController.balls;
        var hole = _gameController.hole;
        objList.AddRange(hole);
        //objectGrid.GetLength(0) = y-axis / height
        //objectGrid.GetLength(1) = x-axis / width
        switch (swipeDirection)
        {
            case SwipeDirection.Right:
                objCoordinateList = SwipeRight();
                break;
            case SwipeDirection.Left:
                objCoordinateList = SwipeLeft();
                break;
            case SwipeDirection.Up:
                objCoordinateList = SwipeUp();
                break;
            case SwipeDirection.Down:
                objCoordinateList = SwipeDown();
                break;
            default:
                objCoordinateList = new List<Vector3>() {Vector3.zero};
                break;
        }


        return objCoordinateList;
    }


    private List<Vector3> SwipeRight()
    {
        var ballPosition = _gameController.balls[0].transform.localPosition;
        Debug.Log("(int)ballsCoordinate[0]: " + new Vector2((int) ballsCoordinate[0].x, (int) ballsCoordinate[0].y));
        //Debug.Log("(int)ballsCoordinate[0].y: "+(int)ballsCoordinate[0].x);
        Debug.Log("objectGrid.GetLength: " + objectGrid.GetLength(1));
        //objectGrid.GetLength(0) = y-axis / height
        //objectGrid.GetLength(1) = x-axis / width
        //objectGrid origin => Top left [16,9]
        //ballPosition origin => Down left
        Debug.Log("(int)-ballPosition.y: " + (int) -ballPosition.y);

        for (int i = (int) ballPosition.x; i < objectGrid.GetLength(1); i++)
        {
            Debug.Log("objectGrid[ i+1, (int)-ballPosition.y]: " + objectGrid[(int) -ballPosition.y, i + 1]);
            Debug.Log("next block is : " + new Vector3(i, ballPosition.y,
                ballPosition.z));
            if (objectGrid[(int) -ballPosition.y, i + 1] != (int)Tile.Floor)
            {
                Debug.Log("Go To : " + new Vector3(i, ballPosition.y, ballPosition.z)); //new Vector3(i,0,0));

                objectGrid[(int) -ballPosition.y, i] = (int)Tile.Ball;
                objectGrid[(int) -ballPosition.y, (int)ballPosition.x] = (int)Tile.Floor;
                return new List<Vector3>()
                {
                    new Vector3(i, ballPosition.y, ballPosition.z)
                };
            }
        }

        Debug.Assert(ballsCoordinate.Count != 0, "No Ballz!!?");
        return new List<Vector3>()
        {
            new Vector3(0, ballPosition.y, ballPosition.z)
        };
    }

    //gameobject ile list yap ve tag ile hangi obje oldupğunu kontrol et.
    private List<Vector3> SwipeLeft()
    {
        //Set your moving objects' x position from left to right
        objList.Sort(delegate(GameObject o, GameObject o1)
            {
                return o.transform.localPosition.x.CompareTo(o1.transform.localPosition.x);
            });
        Debug.Log(objList);
        
        // var ballPosition = _gameController.balls[0].transform.localPosition;
        var ballPosition = _gameController.balls;
        for (int i = 0; i < objList.Count; i++)
        {
            var pos = objList[i].transform.localPosition;
            if (objList[i].CompareTag("Ball"))
            {
                //Check your left every block.
                for (int j = (int) pos.x ; j > 0; j--)
                {
                    //If there is wall on your left
                    if (objectGrid[(int) -pos.y, j - 1] == (int)Tile.Wall)
                    {
                        //Change block positiob from object grid
                        objectGrid[(int) -pos.y, j] = (int)Tile.Ball;
                        objectGrid[(int) -pos.y, (int)pos.x] = (int)Tile.Floor;

                        //Change your previous position to current one
                        objCoordinateList.Add(new Vector3(j, pos.y, pos.z));
                    }
                }
            }
        }

        // for (int i = 0; i < objList.Count; i++)
        // {
        //     objCoordinateList[i]
        // }

        return objCoordinateList;
    }
    
    /*
    private List<Vector3> SwipeLeft()
    {
        var objList = new List<Vector3>();
        var ballPosition = _gameController.balls[0].transform.localPosition;
        for (int i = (int) ballPosition.x; i > 0; i--)
        {
            if (objectGrid[(int) -ballPosition.y, i - 1] != 0)
            {
                objectGrid[(int) -ballPosition.y, i] = (int)Tile.Ball;
                objectGrid[(int) -ballPosition.y, (int)ballPosition.x] = (int)Tile.Floor;

                objList.Add(new Vector3(i, ballPosition.y, ballPosition.z));
                return objList;
            }
        }
        return new List<Vector3>()
        {
            ballPosition
        };
    }*/
    private List<Vector3> SwipeUp()
    {
        var ballPosition = _gameController.balls[0].transform.localPosition;
        for (int i = (int) -ballPosition.y; i > 0; i--)
        {
            Debug.Log("ballsCoordinate[0].y: "+ (int) ballsCoordinate[0].y);
            Debug.Log("objectGrid[ballPosition.x, i-1]: " + objectGrid[i-1, (int) ballPosition.x]);
            if (objectGrid[i-1, (int) ballPosition.x] != 0)
            {
                objectGrid[i, (int) ballPosition.x] = (int)Tile.Ball;
                objectGrid[(int) -ballPosition.y, (int)ballPosition.x] = (int)Tile.Floor;
                return new List<Vector3>()
                {
                    new Vector3(ballPosition.x, -i, ballPosition.z)
                };
            }
        }
        return new List<Vector3>()
        {
            new Vector3(0, ballPosition.y, ballPosition.z)
        };
    }
    private List<Vector3> SwipeDown()
    {
        var ballPosition = _gameController.balls[0].transform.localPosition;
        for (int i = (int) -ballPosition.y; i < objectGrid.GetLength(0); i++)
        {
            Debug.Log("ballsCoordinate[0].y: "+ (int) ballsCoordinate[0].y);
            Debug.Log("objectGrid[ballPosition.x, i-1]: " + objectGrid[i+1, (int) ballPosition.x]);
            if (objectGrid[i+1, (int) ballPosition.x] != 0)
            {
                objectGrid[i, (int) ballPosition.x] = (int)Tile.Ball;
                objectGrid[(int) -ballPosition.y, (int)ballPosition.x] = (int)Tile.Floor;
                return new List<Vector3>()
                {
                    new Vector3(ballPosition.x, -i, ballPosition.z)
                };
            }
        }
        return new List<Vector3>()
        {
            new Vector3(0, ballPosition.y, ballPosition.z)
        };
    }
}