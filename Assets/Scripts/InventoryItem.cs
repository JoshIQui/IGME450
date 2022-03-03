using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private float rotation;

    private Text quantity;
    private GameObject selectedObj;
    private float selectedObjPrevPosX = 0;
    private float selectedObjPrevPosY = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // get mouse position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // check if mouse button has just been pressed
        if (Input.GetMouseButtonDown(0))
        {
            // check if mouse cursor overlaps object
            Collider2D targetedObj = Physics2D.OverlapPoint(mousePos);

            // if it does, set overlapped object
            if (targetedObj && EventSystem.current.IsPointerOverGameObject())
            {
                selectedObj = targetedObj.transform.gameObject;
                Debug.Log(selectedObj.name);
                if (selectedObj.name.Contains("UI_"))
                {
                    Instantiate(prefab,
                        new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0.0f),
                        Quaternion.identity);
                }
            }
        }
        if (Input.GetMouseButtonUp(0) && selectedObj)
        {

            // remove grid system rounding position values to multiples of gridCellSize
            float posX = selectedObj.transform.position.x;
            float posY = selectedObj.transform.position.y;
            selectedObj.transform.position = new Vector3(posX, posY, selectedObj.transform.position.z);

            // check if object overlaps another object (get object center and box size)
            Collider2D selectedObjCollider = selectedObj.GetComponent<Collider2D>();
            Vector2 size = selectedObjCollider.bounds.size;
            List<Collider2D> results = new List<Collider2D>();
            ContactFilter2D filter = new ContactFilter2D();
            int numOfCollisions = Physics2D.OverlapBox(new Vector2(posX, posY), size, 0, filter.NoFilter(), results);

            // if it does, snap object back to original position
            Debug.Log(numOfCollisions);
            if ((numOfCollisions > 0 && results[0] != selectedObjCollider) || numOfCollisions > 1)
            {
                posX = selectedObjPrevPosX;
                posY = selectedObjPrevPosY;
            }
            selectedObj.transform.position = new Vector3(posX, posY, selectedObj.transform.position.z);

            selectedObj = null;
        }
        // regardless of whether mouse button has just been pressed/released, set selectedObj position if there is one
        if (selectedObj)
        {
            selectedObj.transform.position = new Vector3(mousePos.x, mousePos.y, selectedObj.transform.position.z);
        }
    }
}
