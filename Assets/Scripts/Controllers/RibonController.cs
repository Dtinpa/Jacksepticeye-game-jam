using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RibonController : TagController
{
    [SerializeField] private TagSet negativeTag;
    [SerializeField] private TagSet[] positiveTag;
    [SerializeField] private string name;
    [SerializeField] private string desc;

    public string GetNegativeTag()
    {
        return negativeTag.ToString();
    }

    public string GetPositiveTag()
    {
        return positiveTag[0] + ";" + positiveTag[1];
    }

    public string GetName()
    {
        return name;
    }

    public string GetDesc()
    {
        return desc;
    }

}
