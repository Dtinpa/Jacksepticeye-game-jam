using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private GameObject chatlog;
    // Start is called before the first frame update
    private void Start()
    {
        EventManager.current.ActivateDialogue += ActivateDialogue;
        EventManager.current.DeactivateDialogue += DeactivateDialogue;
        EventManager.current.ToggleChatLog += ToggleChatLog;
    }

    private void OnDestroy()
    {
        EventManager.current.ActivateDialogue -= ActivateDialogue;
        EventManager.current.DeactivateDialogue -= DeactivateDialogue;
        EventManager.current.ToggleChatLog -= ToggleChatLog;
    }

    private void ActivateDialogue(NPCController npc, PlayerController player)
    {
        dialogueUI.SetActive(true);
        EventManager.current.OnInitializeDialogue(npc, player);
    }

    private void DeactivateDialogue()
    {
        dialogueUI.SetActive(false);
        EventManager.current.OnEnableInput();
    }

    private void ToggleChatLog()
    {
        chatlog.SetActive(!chatlog.activeSelf);
    }
}
