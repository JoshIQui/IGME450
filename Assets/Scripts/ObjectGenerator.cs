using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    [SerializeField] private ClickAndDrag clickAndDragManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateObject()
    {
        if(transform.parent.gameObject.GetComponent<ItemCounter>().ReduceCounter())
        {
            clickAndDragManager.AttachToMouse(Instantiate(prefab, transform.position, transform.rotation));
        }
    }
}
