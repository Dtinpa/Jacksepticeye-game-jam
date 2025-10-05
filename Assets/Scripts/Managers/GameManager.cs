using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    [SerializeField] private GameObject[] doors;

    private int doorIndex = 0;
    private int maxNumPassPerCart = 3;
    private int currentPassHelped = 0;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        EventManager.current.GameIncrementScore += GameIncrementScore;
        EventManager.current.GameIncrementFloristryScore += GameIncrementFloristryScore;
    }
    private void OnDestroy()
    {
        EventManager.current.GameIncrementScore -= GameIncrementScore;
        EventManager.current.GameIncrementFloristryScore -= GameIncrementFloristryScore;
    }

    // keep track of the passangers the player has helped throughout the current cart they're in
    // if the number of passangers helped is equal t0 the amount in the cart, then move on to the next cart
    private void GameIncrementScore()
    {
        currentPassHelped++;

        if(currentPassHelped >= maxNumPassPerCart)
        {
            maxNumPassPerCart -= 1;
            currentPassHelped = 0;

            if(doorIndex < doors.Length)
            {
                doors[doorIndex].SetActive(false);
                doorIndex++;
            }
        }
    }

    private void GameIncrementFloristryScore()
    {

    }
}
