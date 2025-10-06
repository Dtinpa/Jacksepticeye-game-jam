using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private GameObject chatlog;
    [SerializeField] private GameObject cartUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject endUI;
    // Start is called before the first frame update
    private void Start()
    {
        EventManager.current.ActivateDialogue += ActivateDialogue;
        EventManager.current.DeactivateDialogue += DeactivateDialogue;
        EventManager.current.ToggleChatLog += ToggleChatLog;
        EventManager.current.ToggleCart += ToggleCart;
        EventManager.current.TogglePauseUI += TogglePauseUI;
        EventManager.current.ToggleEndUI += ToggleEndUI;
    }

    private void OnDestroy()
    {
        EventManager.current.ActivateDialogue -= ActivateDialogue;
        EventManager.current.DeactivateDialogue -= DeactivateDialogue;
        EventManager.current.ToggleChatLog -= ToggleChatLog;
        EventManager.current.ToggleCart -= ToggleCart;
        EventManager.current.TogglePauseUI -= TogglePauseUI;
        EventManager.current.ToggleEndUI -= ToggleEndUI;
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

    private void ToggleCart()
    {
        cartUI.SetActive(!cartUI.activeSelf);

        // if the cart isn't active, enable player input
        if(!cartUI.activeSelf)
        {
            EventManager.current.OnEnableInput();
        }
    }

    private void TogglePauseUI()
    {
        pauseUI.SetActive(!pauseUI.activeSelf);
    }

    private void ToggleEndUI()
    {
        endUI.SetActive(true);
    }
}
