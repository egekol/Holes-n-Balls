using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum Tile
{
    Floor = 0,
    Wall = 1,
    Ball = 2,
    Hole = 3,
    Spike = 4,
    Ice = 5,
    Collectable = 6,
    WhiteBall = 7
}

public class LevelService : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private PlaceRenderCamera secondRenderCamera;
    public int[,] objectGrid;
    public int[,] lineGrid;
    public List<GameObject> tileList;
    public List<GameObject> objList;
    public List<Vector3> objCoordinateList;
    public Dictionary<Vector2, GameObject> movableDict = null;
    public Vector3 centerPosition;

    public List<Vector2> ballsCoordinate;
    public Vector2 holeCoordinate;
    private string levelText;
    public int whiteBallID;


    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance._coinNumber = 0;
        GameManager.Instance.CoinsLeft = 0;
        GameManager.Instance.ballList.Clear();
        levelText = GameManager.Instance.CurrentLevel.ToString();
        
        //        We need 2 grid system; one for the blocks and one for the movable objects.
        lineGrid = ExportLevel();
        objectGrid = ExportObject();
        GameManager.Instance.gameState = GameState.Start;

        float xVal = -objectGrid.GetLength(1) / 2f + .5f;
        float yVal = objectGrid.GetLength(0) / 2f + .5f;
        centerPosition = new Vector3(xVal, yVal, 0);

        RenderBlock(lineGrid);
        RenderObject(objectGrid);
        GameManager.Instance._coinNumber = GameManager.Instance.CoinsLeft;
        //        Set Your pivot to center of the grid
        transform.position = centerPosition;
    }

    private int[,] ExportObject()
    {
        var lines = levelText.Split("\n"[0]);
        var lineDataWidth = (lines[0].Trim()).Split(";"[0]).Length;

        objectGrid = new int[lines.Length - 1, lineDataWidth];
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
        var lines = levelText.Split("\n"[0]);
        var lineDataWidth = (lines[0].Trim()).Split(";"[0]).Length;
        lineGrid = new int[lines.Length - 1, lineDataWidth];
        for (int i = 0; i < lines.Length - 1; i++)
        {
            var lineData = (lines[i].Trim()).Split(";"[0]);
            for (int j = 0; j < lineData.Length; j++)
            {
                if (lineData[j] == "2" || lineData[j] == "3" || lineData[j] == "7")
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
                Quaternion rotatiton = transform.localRotation;
                Transform parent = transform;
                if (levelGrid[i, j] == (int) Tile.Floor)
                {
                    Instantiate(prefab, position, rotatiton, parent);
                }
                else if (levelGrid[i, j] == (int) Tile.Wall || levelGrid[i, j] == (int) Tile.Spike ||
                         levelGrid[i, j] == (int) Tile.Collectable)
                {
                    Instantiate(prefab, position, rotatiton, parent);
                    Instantiate(tileList[(int) Tile.Floor], position, rotatiton, parent);
                    if (levelGrid[i, j] == (int) Tile.Collectable)
                    {
                        GameManager.Instance.CoinsLeft++;
                    }
                }
            }
        }
    }

    private void RenderObject(int[,] objectGrid)
    {
        movableDict = new Dictionary<Vector2, GameObject>();
        for (int i = 0; i < objectGrid.GetLength(0); i++)
        {
            for (int j = 0; j < objectGrid.GetLength(1); j++)
            {
                if (objectGrid[i, j] == (int) Tile.Ball || objectGrid[i, j] == (int) Tile.Hole ||
                    objectGrid[i, j] == (int) Tile.WhiteBall)
                {
                    var trnsfrm = transform;
                    Vector3 pos = new Vector3(j, -i, trnsfrm.position.z);
                    var tile = Instantiate(tileList[objectGrid[i, j]], pos, trnsfrm.localRotation, trnsfrm);
                    movableDict.Add(new Vector2(i, j), tile);
                    SaveObjects(tile, pos);

                    //        Send ball objects to GM for win-condition
                    if (objectGrid[i, j] == (int) Tile.WhiteBall)
                    {
                        whiteBallID = tile.GetInstanceID();
                    }

                    objectGrid[i, j] = (int) Tile.Floor;
                }
            }
        }
    }

    private void SaveObjects(GameObject tile, Vector3 pos)
    {
        //        this if block fixes the hole pipe render to pivot difference
        if (tile.CompareTag("Hole"))
        {
            secondRenderCamera.pivotDiff = centerPosition - new Vector3(-pos.x, -pos.y, pos.z);
        }
        else
        {
            GameManager.Instance.ballList.Add(tile);
        }

        gameController.objectList.Add(tile);
    }

    // Update is called once per frame
    void Update()
    {
        var movableObjList = movableDict.Values.ToList();
        object gameHasEnded = movableObjList.Exists(i => i.CompareTag("Ball")) ? 
            "Game continues." : "Game has ended.";
        Debug.Log(gameHasEnded);
    }

    public (Dictionary<GameObject, Vector3>, List<Vector2>) GetBallPathLength(SwipeDirection swipeDirection)
    {
        //        This sends moving objects and their coord to game controller
        Dictionary<GameObject, Vector3> objectDictionary = new Dictionary<GameObject, Vector3>();
        List<Vector2> pathLength = new List<Vector2>();

        //        Clear object and coord list for next move
        objCoordinateList.Clear();
        objList.Clear(); //= new List<GameObject>();
        objList.AddRange(gameController.objectList);

        //        objectGrid.GetLength(0) => y-axis / height
        //        objectGrid.GetLength(1) => x-axis / width
        switch (swipeDirection)
        {
            case SwipeDirection.Right:
                SwipeRight(objectDictionary, pathLength);
                break;
            case SwipeDirection.Left:
                SwipeLeft(objectDictionary, pathLength);
                break;
            case SwipeDirection.Up:
                SwipeUp(objectDictionary, pathLength);
                break;
            case SwipeDirection.Down:
                SwipeDown(objectDictionary, pathLength);
                break;
        }

        return (objectDictionary, pathLength);
    }

    private void SwipeRight(Dictionary<GameObject, Vector3> objectDictionary, List<Vector2> pathLength)
    {
        //    objectGrid origin => Top left [16,9]
        //    ballPosition origin => Down left (-y)
        //        Set your moving objects referencing it's x position from left to right
        IOrderedEnumerable<GameObject> orderBy = objList.OrderByDescending(o => o.transform.localPosition.x);
        objList = orderBy.ToList();
        foreach (var o in objList)
        {
            var pos = o.transform.localPosition;
            Vector2 oldCoord = new Vector2((int) -pos.y, (int) pos.x);
            //        Check your right every block.
            for (int i = (int) pos.x; i < objectGrid.GetLength(1); i++)
            {
                //        When there is hit, this Vector will be new coordinates. 
                Vector2 newCoord = new Vector2(-pos.y, i);    
                //        To check next tile in the grid.
                int nextTile = objectGrid[(int) -pos.y, i + 1];
                //        To check next position to find if there is object.
                Vector2 nextPos = new Vector2((int) -pos.y, i + 1);
                //        Is there any movable object next block?
                GameObject nextMovable = movableDict.ContainsKey(nextPos) ? movableDict[nextPos] : null;
                //        If this block is ball:
                #region Ball
                
                if (o.CompareTag("Ball"))
                {
                    bool hasHit = nextTile == (int) Tile.Wall || nextTile == (int) Tile.Ice;
                    int isFall = 0;
                    if (nextMovable != null)
                    {
                        if (nextMovable.CompareTag("Ball"))
                        {
                            hasHit = true;
                        }

                        if (nextMovable.CompareTag("Hole"))
                        {
                            hasHit = true;
                            isFall = 1;
                        }
                    }

                    if (hasHit)
                    {
                        //        Target coordinate list and object list.
                        objectDictionary.Add(o, new Vector3(i + isFall, pos.y, pos.z));
                        //        Calculate the length of the road for animation duration
                        pathLength.Add(new Vector2(-pos.y, i + isFall) - oldCoord);
                        movableDict.Remove(oldCoord);
                        //        İf ball has fallen in the hole, it doesn't have coordinate in movables dictionary.
                        if (isFall == 0)
                        {
                            movableDict[newCoord] = o;
                        }

                        break;
                    }
                }

                #endregion

                #region Hole

                if (o.CompareTag("Hole"))
                {
                    bool hasHit = nextTile == (int) Tile.Wall || nextTile == (int) Tile.Ice ||
                                  nextTile == (int) Tile.Spike;
                    if (nextMovable != null)
                    {
                        if (nextMovable.CompareTag("Ball"))
                        {
                            movableDict.Remove(nextPos);
                            //gameController.EatTheBall(nextMovable);
                        }

                        if (nextMovable.CompareTag("Hole"))
                        {
                            hasHit = true;
                        }
                    }

                    if (hasHit)
                    {
                        objectDictionary.Add(o, new Vector3(i, pos.y, pos.z));
                        pathLength.Add(newCoord - oldCoord);
                        movableDict.Remove(oldCoord);
                        movableDict[newCoord] = o;
                        break;
                    }
                }

                #endregion
            }
        }
    }


    private void SwipeLeft(Dictionary<GameObject, Vector3> objectDictionary, List<Vector2> pathLength)
    {
        IOrderedEnumerable<GameObject> orderBy = objList.OrderBy(o => o.transform.localPosition.x);
        objList = orderBy.ToList();
        foreach (var o in objList)
        {
            var pos = o.transform.localPosition;
            Vector2 oldCoord = new Vector2((int) -pos.y, (int) pos.x);
            for (int i = (int) pos.x; i > 0; i--)
            {
                Vector2 newCoord = new Vector2((int) -pos.y, i);
                int nextTile = objectGrid[(int) -pos.y, i - 1];
                Vector2 nextPos = new Vector2((int) -pos.y, i - 1);
                GameObject nextMovable = movableDict.ContainsKey(nextPos) ? movableDict[nextPos] : null;

                if (o.CompareTag("Ball"))
                {
                    bool hasHit = nextTile == (int) Tile.Wall || nextTile == (int) Tile.Ice;
                    int isFall = 0;
                    if (nextMovable != null)
                    {
                        if (nextMovable.CompareTag("Ball"))
                        {
                            hasHit = true;
                        }

                        if (nextMovable.CompareTag("Hole"))
                        {
                            hasHit = true;
                            isFall = 1;
                        }
                    }

                    if (hasHit)
                    {
                        objectDictionary.Add(o, new Vector3(i - isFall, pos.y, pos.z));
                        pathLength.Add(new Vector2((int) -pos.y, i - isFall) - oldCoord);
                        movableDict.Remove(oldCoord);
                        if (isFall == 0)
                        {
                            movableDict[newCoord] = o;
                        }

                        break;
                    }
                }

                if (o.CompareTag("Hole"))
                {
                    bool hasHit = nextTile == (int) Tile.Wall || nextTile == (int) Tile.Ice ||
                                  nextTile == (int) Tile.Spike;
                    if (nextMovable != null)
                    {
                        if (nextMovable.CompareTag("Ball"))
                        {
                            movableDict.Remove(nextPos);
                            //gameController.EatTheBall(nextMovable);
                        }

                        if (nextMovable.CompareTag("Hole"))
                        {
                            hasHit = true;
                        }
                    }

                    if (hasHit)
                    {
                        objectDictionary.Add(o, new Vector3(i, pos.y, pos.z));
                        pathLength.Add(newCoord - oldCoord);
                        movableDict.Remove(oldCoord);
                        movableDict[newCoord] = o;
                        break;
                    }
                }
            }
        }
    }

    private void SwipeUp(Dictionary<GameObject, Vector3> objectDictionary, List<Vector2> pathLength)
    {
        IOrderedEnumerable<GameObject> orderBy = objList.OrderByDescending(o => o.transform.localPosition.y);
        objList = orderBy.ToList();
        foreach (GameObject o in objList)
        {
            var pos = o.transform.localPosition;
            Vector2 oldCoord = new Vector2((int) -pos.y, (int) pos.x);

            for (int i = (int) -pos.y; i > 0; i--)
            {
                Vector2 newCoord = new Vector2(i, (int) pos.x);
                int nextTile = objectGrid[i - 1, (int) pos.x];
                Vector2 nextPos = new Vector2(i - 1, (int) pos.x);
                GameObject nextMovable = movableDict.ContainsKey(nextPos) ? movableDict[nextPos] : null;
                if (o.CompareTag("Ball"))
                {
                    bool hasHit = nextTile == (int) Tile.Wall || nextTile == (int) Tile.Ice;
                    int isFall = 0;
                    if (nextMovable != null)
                    {
                        if (nextMovable.CompareTag("Ball"))
                        {
                            hasHit = true;
                        }

                        if (nextMovable.CompareTag("Hole"))
                        {
                            hasHit = true;
                            isFall = 1;
                        }
                    }

                    if (hasHit)
                    {
                        objectDictionary.Add(o, new Vector3((int) pos.x, -i - isFall, (int) pos.z));
                        pathLength.Add(new Vector2(i + isFall, (int) pos.x) - oldCoord);
                        movableDict.Remove(oldCoord);
                        if (isFall == 0)
                        {
                            movableDict[newCoord] = o;
                        }

                        break;
                    }
                }

                if (o.CompareTag("Hole"))
                {
                    bool hasHit = nextTile == (int) Tile.Wall || nextTile == (int) Tile.Ice ||
                                  nextTile == (int) Tile.Spike;
                    if (nextMovable != null)
                    {
                        if (nextMovable.CompareTag("Ball"))
                        {
                            movableDict.Remove(nextPos);
                            //gameController.EatTheBall(nextMovable);
                        }

                        if (nextMovable.CompareTag("Hole"))
                        {
                            hasHit = true;
                        }
                    }

                    if (hasHit)
                    {
                        objectDictionary.Add(o, new Vector3(pos.x, -i, pos.z));
                        pathLength.Add(newCoord - oldCoord);
                        movableDict.Remove(oldCoord);
                        movableDict[newCoord] = o;
                        break;
                    }
                }
            }
        }
    }

    private void SwipeDown(Dictionary<GameObject, Vector3> objectDictionary, List<Vector2> pathLength)
    {
        IOrderedEnumerable<GameObject> orderBy = objList.OrderBy(o => o.transform.localPosition.y);
        objList = orderBy.ToList();
        foreach (GameObject o in objList)
        {
            var pos = o.transform.localPosition;
            Vector2 oldCoord = new Vector2((int) -pos.y, (int) pos.x);
            for (int i = (int) -pos.y; i < objectGrid.GetLength(0); i++)
            {
                Vector2 newCoord = new Vector2(i, (int) pos.x);
                int nextTile = objectGrid[i + 1, (int) pos.x];
                Vector2 nextPos = new Vector2(i + 1, (int) pos.x);
                GameObject nextMovable = movableDict.ContainsKey(nextPos) ? movableDict[nextPos] : null;
                if (o.CompareTag("Ball"))
                {
                    bool hasHit = nextTile == (int) Tile.Wall || nextTile == (int) Tile.Ice;
                    int isFall = 0;
                    if (nextMovable != null)
                    {
                        if (nextMovable.CompareTag("Ball"))
                        {
                            hasHit = true;
                        }

                        if (nextMovable.CompareTag("Hole"))
                        {
                            hasHit = true;
                            isFall = 1;
                        }
                    }

                    if (hasHit)
                    {
                        objectDictionary.Add(o, new Vector3((int) pos.x, -i - isFall, (int) pos.z));
                        pathLength.Add(new Vector2(i + isFall, (int) pos.x) - oldCoord);
                        movableDict.Remove(oldCoord);
                        if (isFall == 0)
                        {
                            movableDict[newCoord] = o;
                        }

                        break;
                    }
                }

                if (o.CompareTag("Hole"))
                {
                    bool hasHit = nextTile == (int) Tile.Wall || nextTile == (int) Tile.Ice ||
                                  nextTile == (int) Tile.Spike;
                    if (nextMovable != null)
                    {
                        if (nextMovable.CompareTag("Ball"))
                        {
                            movableDict.Remove(nextPos);
                            //gameController.EatTheBall(nextMovable);
                        }

                        if (nextMovable.CompareTag("Hole"))
                        {
                            hasHit = true;
                        }
                    }

                    if (hasHit)
                    {
                        objectDictionary.Add(o, new Vector3(pos.x, -i, pos.z));
                        pathLength.Add(newCoord - oldCoord);
                        movableDict.Remove(oldCoord);
                        movableDict[newCoord] = o;
                        break;
                    }
                }
            }
        }
    }
    
}