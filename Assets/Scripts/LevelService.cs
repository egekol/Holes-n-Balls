using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelService : MonoBehaviour
{
    public GameObject rock;
    public GameObject floor;
    private int[,] lineGrid;
    public GameObject ball;

    public GameObject hole;
    public enum eTile
    {
        Rock =0 , Floor, Ball, Hole
    }
    // Start is called before the first frame update
    void Start()
    {
        var levelText = Resources.Load<TextAsset>("Levels/level").text;
        Debug.Log(levelText);
        var lines = levelText.Split("\n"[0]);
        var lineDataWidth   = (lines[0].Trim()).Split(";"[0]).Length;
        lineGrid = new int[lines.Length-1, lineDataWidth];
        for (int i = 0; i < lines.Length-1; i++)
        {
            var lineData   = (lines[i].Trim()).Split(";"[0]);
            for (int j = 0; j < lineData.Length; j++)
            {
                int.TryParse(lineData[j],out lineGrid[i,j]);
            }
        }
        RenderBlock(lineGrid);
        
    }
    public void RenderBlock(int [,]  levelGrid)
    {
        for (int i = 0; i < levelGrid.GetLength(0); i++)
        {
            for (int j = 0; j < levelGrid.GetLength(1); j++)
            {
                Debug.Log(levelGrid[i,j] + " "+i + j );
                if (levelGrid[i,j]==(int) eTile.Rock)
                {
                    Instantiate(rock,new Vector3(j,-i,transform.position.z),transform.rotation,transform);
                }
                else if (levelGrid[i,j] == (int) eTile.Floor)
                {
                    Instantiate(floor,new Vector3(j,-i,transform.position.z),transform.rotation,transform);
                    
                }
                {

                }
            }
        }

        transform.position = new Vector3(-levelGrid.GetLength(1) / 2f+.5f, levelGrid.GetLength(0) / 2f+.5f, 0);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
