using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartingLevel : MonoBehaviour
{
    public Button button1;

    public void OnClick()
    {
        Debug.Log("start");
        button1.enabled = false;
    }
}
