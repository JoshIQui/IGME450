using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartingLevel : MonoBehaviour
{
    public Button button1;

    [SerializeField] private GameObject ball;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            OnClick();
            print("test");
        }
    }

    public void OnClick()
    {
        ball.SetActive(true);
        ball.GetComponent<Ball>().ResetPosition();
        gameObject.SetActive(false);
    }
}
