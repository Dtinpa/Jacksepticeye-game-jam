using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouquetController : TagController
{
    public string flowerAttr;
    public string ribbonTagPositive;
    public string ribbonTagNegative;
    public int teaModifier;

    public int GetRibbonScoreModifier()
    {
        int score = 0;
        string[] flowerTags = flowerAttr.Split(";");

        // get the score modifier for the ribbon and flower combo
        foreach(string tag in flowerTags)
        {
            if(ribbonTagNegative == tag)
            {
                score -= 1;
            }
            if(ribbonTagPositive.Contains(tag)) 
            {
                score += 1;
            }
        }

        return score;
    }
}
