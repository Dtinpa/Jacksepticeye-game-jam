using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;

public class Bucket : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bucketCounter;
    [SerializeField] private GameObject[] items;
    [SerializeField] private string look;
    [SerializeField] private string scent;
    [SerializeField] private string folklore;
    [SerializeField] private string hover;
    [SerializeField] private string name;
    [SerializeField] private string type = "Flower";

    private Stack<GameObject> itemsInBucket;

    private int numOfItems;
    void Start()
    {
        numOfItems = int.Parse(bucketCounter.text);
        itemsInBucket = new Stack<GameObject>(items);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string getLook()
    {
        return look;
    }

    public string getScent()
    {
        return scent;
    }

    public string getFolklore()
    {
        return folklore;
    }

    public string getHover()
    {
        return hover;
    }

    public string getName()
    {
        return name;
    }

    public string GetTypeBucket()
    {
        return type;
    }

    public GameObject[] GetItems()
    {
        return items;
    }

    public GameObject removeItemFromBucket()
    {
        GameObject obj = new GameObject();
        if (numOfItems > 0)
        {
            numOfItems -= 1;
            GameObject item = itemsInBucket.Pop();
            obj = item;

            item.SetActive(true);
            item.transform.SetParent(item.transform.parent.transform.parent);
            bucketCounter.text = numOfItems.ToString();
        }

        return obj;
    }
}
