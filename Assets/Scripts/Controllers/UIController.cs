using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject dialogueUI;
    // Start is called before the first frame update
    private void Start()
    {
        EventManager.current.ActivateDialogue += ActivateDialogue;
        EventManager.current.DeactivateDialogue += DeactivateDialogue;
    }

    private void OnDestroy()
    {
        EventManager.current.ActivateDialogue -= ActivateDialogue;
        EventManager.current.DeactivateDialogue -= DeactivateDialogue;
    }

    private void ActivateDialogue(GameObject npc, PlayerController player)
    {
        dialogueUI.SetActive(true);
        EventManager.current.OnInitializeDialogue(npc, player);
    }

    private void DeactivateDialogue()
    {
        dialogueUI.SetActive(false);
        EventManager.current.OnEnableInput();
    }
}
