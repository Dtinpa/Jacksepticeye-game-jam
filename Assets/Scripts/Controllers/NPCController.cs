using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : TagController
{
    [SerializeField] private GameObject interactButton;
    [SerializeField] private TagSet[] thisObjectsTag;

    public string npcName = "Bob";
    public bool isntHelped = true;
    public bool isInRange { get; private set; } = false;

    public string GetTagSet()
    {
        return thisObjectsTag[0] + ";" + thisObjectsTag[1];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            interactButton.SetActive(true);
            isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            interactButton.SetActive(false);
            isInRange = false;
        }
    }
}
