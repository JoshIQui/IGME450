using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Directions
{
    Up,
    Right,
    Down,
    Left
}

public class ClickAndDrag : MonoBehaviour
{
    private GameObject selectedObj;
    [SerializeField]
    private float gridCellSize = 1;
    [SerializeField]
    private int gridHalfWidth = 10;
    [SerializeField]
    private int gridHalfHeight = 10;
    [SerializeField]
    private float originX = 0;
    [SerializeField]
    private float originY = 0;
    [SerializeField]
    private GameObject gridCell;

    void Start()
    {
        for (int i = -gridHalfWidth; i <= gridHalfWidth; i++)
        {
            for (int j = -gridHalfHeight; j <= gridHalfHeight; j++)
            {
                Instantiate(gridCell, new Vector3(i * gridCellSize + originX, j * gridCellSize + originY, 0.1f), Quaternion.identity);
            }
        }
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
            if (targetedObj)
            {
                selectedObj = targetedObj.transform.gameObject;
            }
        }
        // if mouse button has been released and object is selected, deselect it
        if (Input.GetMouseButtonUp(0) && selectedObj)
        {
            // in place of a grid system, round position values to the nearest multiple of gridCellSize (current value is 1)
            float posX = Mathf.Round(selectedObj.transform.position.x / gridCellSize) * gridCellSize;
            float posY = Mathf.Round(selectedObj.transform.position.y / gridCellSize) * gridCellSize;
            selectedObj.transform.position = new Vector3(posX, posY, selectedObj.transform.position.z);

            // check if object overlaps another object (get object center and box size)
            Collider2D selectedObjCollider = selectedObj.GetComponent<Collider2D>();
            Vector2 size = selectedObjCollider.bounds.size;
            List<Collider2D> results = new List<Collider2D>();
            ContactFilter2D filter = new ContactFilter2D();
            int numOfCollisions = Physics2D.OverlapBox(new Vector2(posX, posY), size, 0, filter.NoFilter(), results);
            // if it does, move it up/right/down/left by a multiple of gridCellSize
            int cellSizeMultiple = 1;
            Directions direction = Directions.Up;
            Debug.Log(numOfCollisions);
            while ((numOfCollisions > 0 && results[0] != selectedObjCollider) || numOfCollisions > 1)
            {
                // reset position values
                posX = selectedObj.transform.position.x;
                posY = selectedObj.transform.position.y;

                // move by the correct multiple in the correct direction
                switch (direction)
                {
                    case Directions.Up:
                        Debug.Log("object shifted up " + gridCellSize);
                        posY += cellSizeMultiple * gridCellSize;
                        break;
                    case Directions.Right:
                        Debug.Log("object shifted right " + gridCellSize);
                        posX += cellSizeMultiple * gridCellSize;
                        break;
                    case Directions.Down:
                        Debug.Log("object shifted down " + gridCellSize);
                        posY -= cellSizeMultiple * gridCellSize;
                        break;
                    case Directions.Left:
                        Debug.Log("object shifted left " + gridCellSize);
                        posX -= cellSizeMultiple * gridCellSize;                        
                        break;
                }
                direction++;
                if (direction > Directions.Left)
                {
                    direction = Directions.Up;
                    // if none of these four directions worked, move object over two units instead of one
                    // keep incrementing until it works in a direction
                    cellSizeMultiple++;
                }

                // check if new position collides with anything before setting new position
                numOfCollisions = Physics2D.OverlapBox(new Vector2(posX, posY), size, 0, filter.NoFilter(), results);
                Debug.Log(numOfCollisions);
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
