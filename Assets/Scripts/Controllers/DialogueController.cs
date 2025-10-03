using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogueController : MonoBehaviour
{
    private NPCController currentNpc;
    private PlayerController playerObj;
    private int totalCharacters;
    private int totalVisibleCharacters;
    private bool isTyping = true;
    private string[] lines;
    private int lineIndex = 0;

    [SerializeField] private TextMeshProUGUI nameTxtField;
    [SerializeField] private TextMeshProUGUI dialogueTxtBox;
    [SerializeField] private float timeBetweenChr = 0.2f;

    // Start is called before the first frame update
    void Awake()
    {
        totalCharacters = 0;
        totalVisibleCharacters = 0;

        EventManager.current.InitializeDialogue += InitializeDialogue;
    }

    private void OnDestroy()
    {
        EventManager.current.InitializeDialogue -= InitializeDialogue;
    }

    // Update is called once per frame

    void Update()
    {
        // if the player hits the interact button, we move to the next bit of dialogue and reset variables to track where we are in the script
        if (playerObj.playerInput.actions["Interact"].WasPressedThisFrame())
        {
            totalVisibleCharacters = 0;
            isTyping = true;
            lineIndex++;

            while(lines[lineIndex] == string.Empty)
            {
                lineIndex++;
            }

            dialogueTxtBox.text = lines[lineIndex];
            dialogueTxtBox.maxVisibleCharacters = 0;
            totalCharacters = dialogueTxtBox.text.Length;
        }

        if(isTyping)
        {
            StartCoroutine(TextVisible());
        }
    }

    private void InitializeDialogue(NPCController npc, PlayerController player)
    {
        playerObj = player;
        currentNpc = npc;
        nameTxtField.text = npc.npcName;

        GetDialogue();
    }

    private IEnumerator TextVisible()
    {
        // if the total visible characters doesn't match the total characters in the sentence, continue revealing letters
        // else, we're done typing the sentence for now, and we wait for the player to press the interact button to move the dialogue
        if(totalVisibleCharacters <= totalCharacters)
        {
            totalVisibleCharacters = totalVisibleCharacters + 1;
            dialogueTxtBox.maxVisibleCharacters = totalVisibleCharacters;
        } else
        {
            isTyping = false;
        }

        yield return new WaitForSeconds(timeBetweenChr);
    }

    private void GetDialogue()
    {
        totalVisibleCharacters = 0;
        StreamReader sr = new StreamReader(Application.dataPath + "/Dialogue/" + currentNpc.npcName + "/dialogue.txt");
        string fileContents = sr.ReadToEnd();
        fileContents = fileContents.Replace("\r\n", string.Empty);
        sr.Close();

        // stores the lines of the text file in a string array for us to traverse in the Update function
        // totalCharacters is set here to the first line in the file to start us off
        lines = fileContents.Split("***", StringSplitOptions.RemoveEmptyEntries);
        dialogueTxtBox.text = lines[lineIndex];
        dialogueTxtBox.maxVisibleCharacters = 0;
        totalCharacters = dialogueTxtBox.text.Length;
    }
}
