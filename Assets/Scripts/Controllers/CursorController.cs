using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using TMPro;
using System.IO;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    public PlayerInput playerInput { get; private set; }

    [SerializeField] private Button chatLog;
    [SerializeField] private Button exitDialogue;
    [SerializeField] private Button exitCart;
    [SerializeField] private RectTransform[] bucketRectTransArray;
    [SerializeField] private TextMeshProUGUI summaryTxt;
    [SerializeField] private GameObject summaryBox;
    [SerializeField] private float speed = 1.5f;

    private RectTransform transformCursor;
    private RectTransform chatLogRect;
    private RectTransform exitDialogueRect;
    private RectTransform exitCartRect;
    private RectTransform[] buttonRectArray;

    private Rect canvasRect;
    private bool isPressEnabled = true;
    private string currentHover = "";

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        transformCursor = GetComponent<RectTransform>();
        chatLogRect = chatLog.GetComponentInParent<RectTransform>();
        exitDialogueRect = exitDialogue.GetComponentInParent<RectTransform>();
        exitCartRect = exitCart.GetComponentInParent<RectTransform>();

        canvasRect = transform.parent.GetComponentInParent<Canvas>().pixelRect;

        buttonRectArray = new RectTransform[] { chatLogRect, exitDialogueRect, exitCartRect };

    }

    private void Update()
    {
        Movement();
        GetHoverText();

        if (playerInput.actions["GeneralAction"].WasPressedThisFrame())
        {
            CheckOverlap();
        }

        if (playerInput.actions["GeneralAction"].IsPressed())
        {

        }
    }

    private void Movement()
    {
        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
        transformCursor.position = new Vector2(transform.position.x + input.x * speed, transform.position.y + input.y * speed);

        // keeps the cursor within the bounds of the canvas
        Vector2 clamp = transformCursor.position;
        clamp.x = Mathf.Clamp(clamp.x, transformCursor.rect.width / 2, canvasRect.width - transformCursor.rect.width / 2);
        clamp.y = Mathf.Clamp(clamp.y, transformCursor.rect.height / 2, canvasRect.height - transformCursor.rect.height / 2);

        transformCursor.position = clamp;
    }

    private void CheckOverlap()
    {
        if (isPressEnabled)
        {
            isPressEnabled = false;
            bool exitEarly = false;
            
            foreach (RectTransform rect in buttonRectArray)
            {
                if (rect != null)
                {
                    Rect rect1 = new Rect(transformCursor.localPosition.x, transformCursor.localPosition.y, transformCursor.rect.width, transformCursor.rect.height);
                    Rect rect2 = new Rect(rect.localPosition.x, rect.localPosition.y, rect.rect.width, rect.rect.height);

                    // if our cursor overlaps one of the buttons, click the button
                    if (rect1.Overlaps(rect2))
                    {
                        Button button = rect.gameObject.GetComponent<Button>();
                        button.onClick.Invoke();
                        exitEarly = true;
                        break;
                    }
                }
            }

            if (!exitEarly)
            {
                foreach (RectTransform rect in bucketRectTransArray)
                {
                    if (rect != null)
                    {
                        Vector3 distance = (rect.transform.parent.position - transform.position);
                        
                        // if our cursor overlaps one of the buttons, click the button
                        if (Mathf.Abs(distance.x) < 60 && Mathf.Abs(distance.y) < 60)
                        {
                            GameObject item = rect.gameObject;
                            item.transform.parent.GetComponent<Bucket>().removeItemFromBucket();
                            break;
                        }
                    }
                }
            }

            isPressEnabled = true;
        }
    }

    private void GetHoverText()
    {
        string look = "";
        string scent = "";
        string folklore = "";
        string hover = "";
        string name = "";

        foreach (RectTransform rect in bucketRectTransArray)
        {
            Vector3 distance = (rect.transform.parent.position - transform.position);
            GameObject item = rect.gameObject;

            // if our cursor overlaps one of the buttons, click the button
            if (Mathf.Abs(distance.x) < 60 && Mathf.Abs(distance.y) < 60)
            {
                look = item.transform.parent.GetComponent<Bucket>().getLook();
                scent = item.transform.parent.GetComponent<Bucket>().getScent();
                folklore = item.transform.parent.GetComponent<Bucket>().getFolklore();
                hover = item.transform.parent.GetComponent<Bucket>().getHover();
                name = item.transform.parent.GetComponent<Bucket>().getName();
                break;
            }
        }

        // if the list of objects
        if (scent != "")
        {
            currentHover = name;
            summaryTxt.text = "Name: " + name + "\n\n" + hover + "\n\nScent: " + scent + "\nFolklore: " + folklore;
            summaryBox.SetActive(true);
        } else
        {
            summaryTxt.text = "";
            summaryBox.SetActive(false);
            currentHover = "";
        }
    }
}
