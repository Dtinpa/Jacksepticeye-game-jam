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

    public void removeItemFromBucket()
    {
        if (numOfItems > 0)
        {
            numOfItems -= 1;
            GameObject item = itemsInBucket.Pop();
            item.SetActive(true);
            item.transform.parent = item.transform.parent.transform.parent;
            bucketCounter.text = numOfItems.ToString();
        }
    }
}
