using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerController : TagController
{
    [SerializeField] private TagSet thisObjectsTag;

    public string GetTag()
    {
        return thisObjectsTag.ToString();
    }
}
