using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    private Text text;

    bool isPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<Text>();
    }

    public void ToggleButton()
    {
        if (isPlaying)
        {
            text.text = "Play";
            isPlaying = false;
        }
        else
        {
            text.text = "Stop";
            isPlaying = true;
        }
    }
}
