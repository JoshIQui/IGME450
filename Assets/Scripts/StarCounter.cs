using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCounter : MonoBehaviour
{
    //[SerializeField] DEBUG SERIALIZATION
    public int starCount = 0;

    private GameObject[] starHoles = new GameObject[3];

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            starHoles[i] = GameObject.Find("StarHole" + (i + 1));
            //if (starCount > i) starHoles[i].SetActive(false); DEBUG INITIALIZE STARS
        }
    }

    public void IncreaseStarCount()
    {
        print("Star " + starCount + " Filled");
        starHoles[starCount].SetActive(false);
        starCount++;
    }

    public void ResetStarCount()
    {
        foreach (GameObject star in starHoles)
        {
            star.SetActive(true);
        }
        starCount = 0;
    }
}
