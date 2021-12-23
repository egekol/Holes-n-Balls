using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColliderController : MonoBehaviour
{
    [SerializeField] private LevelService levelService;
    public bool EatenByHole { get; set; }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collision on : " + other);
        Debug.Log(" on : ");
        if (other.CompareTag("Hole") && gameObject.CompareTag("Ball"))
        {
            DisableComponents(gameObject);
            GameManager.Instance.EatenBall = gameObject;
        }

        if (other.CompareTag("Spike") && gameObject.CompareTag("Ball"))
        {
            DisableComponents(gameObject);
            Debug.Log("game over, spike!");
            GameManager.Instance.gameState = GameState.Lose;
            //gameover
        }

        if (other.CompareTag("Collectable"))
        {
            DisableComponents(other.gameObject);
            Debug.Log("Coins! ++");
            GameManager.Instance.CoinsLeft--;
        }
    }

    private void CheckIfWhiteExist()
    {
        /*var movableObjList = levelService.movableDict.Values.ToList();
        if (movableObjList.Count != 2
            && !movableObjList.Exists(i => i.name == "Ball_White(Clone)"))
        {
            GameManager.Instance.gameState = GameState.Lose;
            Debug.Log("game over, wrong ball");
        }*/
    }

    private void DisableComponents(GameObject o)
    {
        o.GetComponent<MeshRenderer>().enabled = false;
        o.GetComponent<Collider>().enabled = false;
        if (o.TryGetComponent<TrailRenderer>(out var component))
        {
            component.enabled = false;
        }
    }
}