using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    private ClickAndDrag clickAndDragManager;

    // Start is called before the first frame update
    void Start()
    {
        clickAndDragManager = GameObject.Find("ClickAndDragManager").GetComponent<ClickAndDrag>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateObject()
    {
        if(transform.parent.gameObject.GetComponent<ItemCounter>().ReduceCounter())
        {
            // clickAndDragManager.AttachToMouse(Instantiate(prefab, transform.position, transform.rotation));
            GameObject newObj = Instantiate(prefab, transform.position, transform.rotation);
            clickAndDragManager.AttachToMouse(newObj);
            clickAndDragManager.movableObjectList.Add(newObj);
            Debug.Log("Count: " + clickAndDragManager.movableObjectList.Count);
        }
    }
}
