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
    [SerializeField] private GameObject cookMat;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private bool isCart = true;

    private RectTransform transformCursor;
    private RectTransform chatLogRect;
    private RectTransform exitDialogueRect;
    private RectTransform exitCartRect;
    private RectTransform[] buttonRectArray;

    private Rect canvasRect;
    private bool isPressEnabled = true;
    private List<GameObject> itemsRetrieved = new List<GameObject>();

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

        if (isCart)
        {
            GetHoverText();
        }

        if (playerInput.actions["GeneralAction"].WasPressedThisFrame())
        {
            CheckOverlap();
        }

        if (playerInput.actions["GeneralAction"].IsPressed() && isCart)
        {
            GrabItem();
            CookBouquet();
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
                            GameObject obj = item.transform.parent.GetComponent<Bucket>().removeItemFromBucket();
                            itemsRetrieved.Add(obj);
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

        string text = "";

        foreach (RectTransform rect in bucketRectTransArray)
        {
            Vector3 distance = (rect.transform.parent.position - transform.position);
            GameObject item = rect.gameObject;

            // if our cursor overlaps one of the buttons, click the button
            if (Mathf.Abs(distance.x) < 60 && Mathf.Abs(distance.y) < 60)
            {
                Bucket bucket = item.transform.parent.GetComponent<Bucket>();
                look = bucket.getLook();
                scent = bucket.getScent();
                folklore = bucket.getFolklore();
                hover = bucket.getHover();
                name = bucket.getName();

               
                if(bucket.GetTypeBucket() == "Flower")
                {
                    text = "Name: " + name + "\n\n" + hover + "\n\nScent: " + scent + "\nFolklore: " + folklore;
                } else
                {
                    text = "Name: " + name + "\n\nFolklore: " + folklore;
                }
                
                break;
            }
        }

        // if the list of objects
        if (name != "")
        {
            summaryTxt.text = text;
            summaryBox.SetActive(true);
        } else
        {
            summaryTxt.text = "";
            summaryBox.SetActive(false);
        }
    }

    private void GrabItem()
    {
        foreach (GameObject obj in itemsRetrieved)
        {
            Vector3 distance = (obj.transform.position - transform.position);

            // if our cursor overlaps one of the buttons, click the button
            if (Mathf.Abs(distance.x) < 60 && Mathf.Abs(distance.y) < 60)
            {
                obj.transform.position = transform.position;
            }
        }
    }

    private void CookBouquet()
    {
        List<GameObject> collection = new List<GameObject>();
        List<string> tags = new List<string>();
        int index = 0;
        bool gotRibbon = false;
        Stack<int> itemsRetrievedIndexStack = new Stack<int>();
        int itemsRetrievedIndex = 0;

        foreach (GameObject obj in itemsRetrieved)
        {
            
            if(obj == null)
            {
                itemsRetrievedIndex++;
                break;
            }

            Vector3 dist = (obj.transform.position - transform.position);

            if (Mathf.Abs(dist.x) < 60 && Mathf.Abs(dist.y) < 60)
            {
                if (index > 2)
                {
                    break;
                }

                collection.Add(obj);
                itemsRetrievedIndexStack.Push(itemsRetrievedIndex);

                // if its a flower, get the tag using the flower controller
                // if its a ribbon, get the tags from the ribbon controller
                // if we already have a ribbon, then exit this function
                if (obj.tag == "Flower")
                {
                    tags.Add("Flowers:" + obj.GetComponent<FlowerController>().GetTag());
                }
                else if (!gotRibbon)
                {
                    RibonController ribbon = obj.GetComponent<RibonController>();
                    tags.Add("Ribbon:" + ribbon.GetPositiveTag() + "," + ribbon.GetNegativeTag());
                    gotRibbon = true;
                }

                index++;

            }

            itemsRetrievedIndex++;
        }

        Vector3 distance = (cookMat.transform.position - transform.position);

        // if we're near the cook mat and we have 3 items from our tray in it, then cook a bouquet
        // yes you don't cook flowers
        // I am very stressed
        if (Mathf.Abs(distance.x) < 60 && Mathf.Abs(distance.y) < 60 && collection.Count == 3)
        {
            string flowerAttr = "";
            string tagPositive = "";
            string tagNegative = "";
            foreach (string tag in tags)
            {
                if (tag.Contains("Flowers"))
                {
                    flowerAttr += tag.Replace("Flowers:", "") + ";";
                }
                else
                {
                    string rib = tag.Replace("Ribbon:", "");
                    int charLocation = rib.IndexOf(",");

                    tagPositive = rib;
                    tagPositive = rib.Substring(0, charLocation);
                    tagNegative = rib.Substring(charLocation + 1, rib.Length - charLocation - 1);
                }
            }

            EventManager.current.OnAddInventory(flowerAttr, tagPositive, tagNegative, "1");

            foreach (int ind in itemsRetrievedIndexStack)
            {
                GameObject obj = (itemsRetrieved[ind]);
                itemsRetrieved.RemoveAt(ind);
                Destroy(obj);
            }

            // I'm desperate woman
            // may God have mercy on me in the next life
            foreach (GameObject obj in collection)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }
    }
}
