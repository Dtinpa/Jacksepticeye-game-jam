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
    public Stack<string> inventory = new Stack<string>();

    private bool gamePaused = false;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        EventManager.current.AddInventory += AddInventory;
        EventManager.current.GiveItemFromInventory += GiveItemFromInventory;

        EventManager.current.GameStarted += StartGame;
        EventManager.current.GameEnded += EndGame;
        EventManager.current.GameExit += GameExit;

        EventManager.current.GameFinished += GameFinished;

        EventManager.current.GamePause += PauseGame;
        EventManager.current.GameUnPause += UnPauseGame;
    }
    private void OnDestroy()
    {
        EventManager.current.AddInventory -= AddInventory;
        EventManager.current.GiveItemFromInventory -= GiveItemFromInventory;

        EventManager.current.GameStarted -= StartGame;
        EventManager.current.GameEnded -= EndGame;
        EventManager.current.GameExit -= GameExit;

        EventManager.current.GameFinished -= GameFinished;

        EventManager.current.GamePause -= PauseGame;
        EventManager.current.GameUnPause -= UnPauseGame;
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

    private void AddInventory(string flowerAttr, string ribbonTagPositive, string ribbonTagNegative, string teaModifier) 
    {
        int score = 0;
        string[] flowerTags = flowerAttr.Split(";");

        // get the score modifier for the ribbon and flower combo
        foreach (string tag in flowerTags)
        {
            if (ribbonTagNegative == tag)
            {
                score -= 1;
            }
            if (ribbonTagPositive.Contains(tag))
            {
                score += 1;
            }
        }

        score += int.Parse(teaModifier);
        inventory.Push(flowerAttr + "," + score);

    }

    private void GiveItemFromInventory(GameObject npc)
    {
        NPCController controller = npc.GetComponent<NPCController>();
        string[] npcTags = controller.GetTagSet().Split(";");

        string flowerAttrAndScore = inventory.Pop();
        string[] attr = flowerAttrAndScore.Split(",");
        int score = int.Parse(attr[1]);
        foreach (string tag in npcTags)
        {
            if(attr[0].Contains(tag))
            {
                score += 1;
            }
        }

        Debug.Log(score);

        GameIncrementScore();
        EventManager.current.OnCompleteDialogue(score);
    }

    private void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    private void EndGame()
    {
        Application.Quit();
    }

    private void GameFinished()
    {

    }

    private void GameExit()
    {
        SceneManager.LoadScene(0);
    }

    private void PauseGame()
    {
        if (!gamePaused)
        {
            Time.timeScale = 0;
            gamePaused = true;
            EventManager.current.OnTogglePauseUI();
        }
    }

    private void UnPauseGame()
    {
        if (gamePaused)
        {
            Time.timeScale = 1;
            gamePaused = false;
            EventManager.current.OnTogglePauseUI();
        }
    }
}
