using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    private string dialogueRootDirectory = "Assets/Dialogue/";
    private GameObject currentNpc;
    private PlayerController playerObj;

    [SerializeField] private TextMeshProUGUI nameTxtField;

    // Start is called before the first frame update
    void Awake()
    {
        EventManager.current.InitializeDialogue += InitializeDialogue;
    }

    private void OnDestroy()
    {
        EventManager.current.InitializeDialogue -= InitializeDialogue;
    }

    // Update is called once per frame

    void Update()
    {
        //TODO: advance to next dialogue in text file
        if (playerObj.playerInput.actions["Interact"].WasPressedThisFrame())
        {

        }
    }

    private void InitializeDialogue(GameObject npc, PlayerController player)
    {
        playerObj = player;
        currentNpc = npc;
        nameTxtField.text = npc.name;
    }
}
