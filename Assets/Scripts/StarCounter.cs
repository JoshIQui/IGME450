using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarCounter : MonoBehaviour
{
    public int starCount = 0;

    [SerializeField]
    private Image[] starHoles = new Image[3];

    private void Start()
    {
    }

    public void IncreaseStarCount()
    {
        print("Star " + starCount + " Filled");
        starHoles[starCount].color = Color.white;
        starCount++;
    }

    public void ResetStarCount()
    {
        foreach (Image star in starHoles)
        {
            star.color = Color.gray;
        }
        starCount = 0;
    }
}
