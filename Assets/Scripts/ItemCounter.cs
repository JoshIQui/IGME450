using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCounter : MonoBehaviour
{
    [SerializeField] private int count;

    private Text countText;

    // Start is called before the first frame update
    void Start()
    {
        countText = GetComponent<Text>();
        countText.text = count.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool ReduceCounter()
    {
        if (count > 0)
        {
            count--;
            countText.text = count.ToString();
            return true;
        }
        return false;
    }
}
