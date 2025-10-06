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
    private AudioSource[] audioSources;

    private string metadataDelimeter = ";";
    private string fileStopDialogueDelimeter = "<stop>";

    private string filePath = "";

    // I'm gonna die if I have to do one more special variable
    private bool conductorDontRepeat = false;

    [SerializeField] private TextMeshProUGUI nameTxtField;
    [SerializeField] private TextMeshProUGUI dialogueTxtBox;
    [SerializeField] private TextMeshProUGUI chatlogTxtBox;
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private GameObject cartUI;
    [SerializeField] private GameObject cursor;

    [SerializeField] private float timeBetweenChr = 0.2f;

    // Start is called before the first frame update
    void Awake()
    {
        audioSources = gameObject.GetComponents<AudioSource>();
        totalCharacters = 0;
        totalVisibleCharacters = 0;

        EventManager.current.InitializeDialogue += InitializeDialogue;
        EventManager.current.CompleteDialogue += CompleteDialogue;
    }

    private void OnDestroy()
    {
        EventManager.current.InitializeDialogue -= InitializeDialogue;
        EventManager.current.CompleteDialogue -= CompleteDialogue;
    }

    // Update is called once per frame

    void Update()
    {
        if(playerObj.playerInput.actions["Interact"].WasPressedThisFrame() && conductorDontRepeat && lineIndex >= lines.Length) {
            EventManager.current.OnToggleEndUI();
        }

        // if the player hits the interact button, we move to the next bit of dialogue and reset variables to track where we are in the script
        // if the line we're on contains a <stop> tag, it means we cannot advance the dialogue until the minigames are finished or until we've selected a dialogue option
        // also, only advance dialogue when outside of chatbox
        if (playerObj.playerInput.actions["Interact"].WasPressedThisFrame() && lineIndex < lines.Length - 1 && !chatlogTxtBox.isActiveAndEnabled)
        {
            if (!lines[lineIndex].Contains(fileStopDialogueDelimeter)) {
                AdvanceDialogue();
            }
            if(GameManager.current.inventory.Count > 0 && currentNpc.isntHelped && currentNpc.name != "Conductor")
            {
                currentNpc.isntHelped = false;
                EventManager.current.OnGiveItemFromInventory(currentNpc.gameObject);
            } else if(playerObj.playerInput.actions["Interact"].WasPressedThisFrame() && currentNpc.name == "Conductor" && lines[lineIndex].Contains(fileStopDialogueDelimeter) && !conductorDontRepeat)
            {
                lineIndex = 0;
                currentNpc.isntHelped = false;
                conductorDontRepeat = true;
                GetDialogue(GameManager.current.helpedLeaveTrain);
            }
        } 
        else if(playerObj.playerInput.actions["Interact"].WasPressedThisFrame() && currentNpc.name != "Conductor" && lines[lineIndex].Contains(fileStopDialogueDelimeter) && GameManager.current.inventory.Count > 0)
        {
            currentNpc.isntHelped = false;
            EventManager.current.OnGiveItemFromInventory(currentNpc.gameObject);
        }

        if (isTyping)
        {
            StartCoroutine(TextVisible());
        }
    }

    // initialize the dialogue UI with the text for the particular NPC
    private void InitializeDialogue(NPCController npc, PlayerController player)
    {
        playerObj = player;
        currentNpc = npc;
        nameTxtField.text = npc.npcName;

        filePath = Application.dataPath + "/StreamingAssets/" + currentNpc.npcName;

        GetDialogueMetadata();
        GetDialogue(currentNpc.score);
        GetChatlog();

        if(currentNpc.name == "Conductor")
        {
            audioSources[2].Play();
        }

        isTyping = true;
    }

    private void OnDisable()
    {
        UpdateDialogueMetadata(lineIndex);
        SaveChatlog();
        lines = new string[] { };
        lineIndex = 0;
    }

    private IEnumerator TextVisible()
    {
        // if the total visible characters doesn't match the total characters in the sentence, continue revealing letters
        // else, we're done typing the sentence for now, and we wait for the player to press the interact button to move the dialogue
        // if we've helped the NPC and we're at the end of the dialogue, play the open door audio to let them leave
        if(totalVisibleCharacters <= totalCharacters)
        {
            totalVisibleCharacters = totalVisibleCharacters + 1;
            dialogueTxtBox.maxVisibleCharacters = totalVisibleCharacters;
        } else if(lines[lineIndex].Contains(fileStopDialogueDelimeter) && !currentNpc.isntHelped && currentNpc.name != "Conductor")
        {
            audioSources[0].Play();
            GameManager.current.audioSources.Play();
            Destroy(currentNpc.gameObject);
            EventManager.current.OnDeactivateDialogue();
        }
        else
        {
            isTyping = false;
            chatlogTxtBox.text += lines[lineIndex].Replace(fileStopDialogueDelimeter, "") + "\n***\n";
        }

        yield return new WaitForSeconds(timeBetweenChr);
    }

    private void GetDialogue(int score = 0)
    {
        string fileName = "";
        if (score == 0 && !currentNpc.isntHelped)
        {
            fileName = filePath + "/dialogue_Bad.txt";
        } 
        else if((score == 1 || score == 2) && !currentNpc.isntHelped)
        {
            fileName = filePath + "/dialogue_Okay.txt";
        }
        else if(score >= 3 && !currentNpc.isntHelped)
        {
            fileName = filePath + "/dialogue_Good.txt";
        }
        else
        {
            fileName = filePath + "/dialogue.txt";
        }

        totalVisibleCharacters = 0;
        StreamReader sr = new StreamReader(fileName);
        string fileContents = sr.ReadToEnd();
        fileContents = fileContents.Replace("\n", string.Empty);
        sr.Close();

        // stores the lines of the text file in a string array for us to traverse in the Update function
        // totalCharacters is set here to the first line in the file to start us off
        lines = fileContents.Split("***", StringSplitOptions.RemoveEmptyEntries);

        if(lineIndex >= lines.Length && !currentNpc.isntHelped)
        {
            lineIndex = lines.Length - 1;
        } else
        {
            lineIndex = 0;
        }

        dialogueTxtBox.text = lines[lineIndex].Replace(fileStopDialogueDelimeter, "");
        dialogueTxtBox.maxVisibleCharacters = 0;
        totalCharacters = dialogueTxtBox.text.Length;
    }

    private void UpdateDialogueMetadata(int lineIndex)
    {
        CheckMetadataExists();
        File.WriteAllText(filePath + "/dialogue_metadata.txt", "lineIndex: " + lineIndex + metadataDelimeter);
    }

    // get the current line we're on with the NPC and the score we have with them
    private void GetDialogueMetadata()
    {
        CheckMetadataExists();

        StreamReader sr = new StreamReader(filePath + "/dialogue_metadata.txt");
        string fileContents = sr.ReadToEnd();
        fileContents = fileContents.Replace("\r\n", string.Empty);
        sr.Close();

        string[] lines = fileContents.Split(";", StringSplitOptions.RemoveEmptyEntries);

        foreach(string line in lines)
        {
            string[] metadata = line.Split(": ", StringSplitOptions.RemoveEmptyEntries);

            switch(metadata[0])
            {
                case "lineIndex":
                    lineIndex = int.Parse(metadata[1]);
                    break;
                default:
                    lineIndex = 0;
                    break;
            }
        }

        if(lines.Length == 0)
        {
            lineIndex = 0;
        }
    }

    // get the dialogue based on how well the player did
    private void CompleteDialogue(int score)
    {
        currentNpc.isntHelped = false;
        currentNpc.score = score;
        audioSources[1].Play();

        GetDialogue(score);
        AdvanceDialogue();
    }

    private void AdvanceDialogue()
    {
        totalVisibleCharacters = 0;
        isTyping = true;
        lineIndex++;

        while (lines[lineIndex] == string.Empty)
        {
            lineIndex++;
        }

        UpdateDialogueMetadata(lineIndex);

        dialogueTxtBox.text = lines[lineIndex].Replace(fileStopDialogueDelimeter, "");
        dialogueTxtBox.maxVisibleCharacters = 0;
        totalCharacters = dialogueTxtBox.text.Length;
    }

    private void GetChatlog()
    {
        CheckChatLogExists();

        StreamReader sr = new StreamReader(filePath + "/dialogue_chatlog.txt");
        string fileContents = sr.ReadToEnd();
        sr.Close();

        chatlogTxtBox.text = fileContents;
    }
    private void CheckMetadataExists()
    {
        // if this file doesn't exist, create one
        if (!File.Exists(filePath + "/dialogue_metadata.txt"))
        {
            StreamWriter sw = File.CreateText(filePath + "/dialogue_metadata.txt");
            sw.WriteLine("lineIndex: " + lineIndex + metadataDelimeter);
            sw.Close();
        }
    }

    private void CheckChatLogExists()
    {
        // if this file doesn't exist, create one
        if (!File.Exists(filePath + "/dialogue_chatlog.txt"))
        {
            StreamWriter sw = File.CreateText(filePath + "/dialogue_chatlog.txt");
            sw.WriteLine("");
            sw.Close();
        }
    }

    private void SaveChatlog()
    {
        File.WriteAllText(filePath + "/dialogue_chatlog.txt", chatlogTxtBox.text);
    }
}
