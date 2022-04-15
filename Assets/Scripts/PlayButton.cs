using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    private Text text;

    private bool isPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<Text>();
    }

    private void Update()
    {
        //print(isPlaying);
    }

    public void ToggleButton()
    {
        // If the gamestate is successfully changed
        if(GameManager.instance.Play())
        {
            ChangeText();
        }
    }

    public void ChangeText()
    {
        if (isPlaying)
        {
            text.text = "PLAY";
        }
        else
        {
            text.text = "STOP";
        }
        isPlaying = !isPlaying;
    }
}
