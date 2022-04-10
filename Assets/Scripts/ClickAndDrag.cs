using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private float selectedObjPrevPosX = 0;
    private float selectedObjPrevPosY = 0;
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
    private GameObject gridPoint;

    public List<GameObject> movableObjectList = new List<GameObject>();

    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject objOverlapWarningText;

    private bool canBuild = false;
    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameManager.GameState obj)
    {
        if (obj == GameManager.GameState.Building)
        {
            canBuild = true;
        }
        else
        {
            canBuild = false;
        }
    }
    
    void Start()
    {
        objOverlapWarningText.SetActive(false);
    }

    public void DragHandler(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;

        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            pointerData.position,
            canvas.worldCamera,
            out position);

        transform.position = canvas.transform.TransformPoint(position);
        // if mouse button has been released and object is selected, deselect it
        if (Input.GetMouseButtonUp(0))
        {
            // in place of a grid system, round position values to the nearest multiple of gridCellSize (current value is 1)
            float posX = Mathf.Round(this.transform.position.x / gridCellSize) * gridCellSize;
            float posY = Mathf.Round(this.transform.position.y / gridCellSize) * gridCellSize;
            this.transform.position = new Vector3(posX, posY, this.transform.position.z);

            // check if object overlaps another object (get object center and box size)
            Collider2D selectedObjCollider = this.GetComponent<Collider2D>();
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
                posX = this.transform.position.x;
                posY = this.transform.position.y;

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
            this.transform.position = new Vector3(posX, posY, selectedObj.transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canBuild)
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
                    // deselect object if selectedObj is immovable despite having a Collider2D
                    if (selectedObj.name == "Start" || selectedObj.name == "End" || selectedObj.name.ToUpper().Contains("IMMOBILE") || selectedObj.name.ToUpper().Contains("UI_"))
                    {
                        selectedObj = null;
                    }
                    // [OLD CODE] set previous position values to current values of selected object
                    // [NEW CODE] place selected object in front of everything else w/ z position value whenever an object is selected
                    else
                    {
                        // selectedObjPrevPosX = selectedObj.transform.position.x;
                        // selectedObjPrevPosY = selectedObj.transform.position.y;

                        foreach (GameObject obj in movableObjectList)
                        {
                            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, 90);
                        }
                        selectedObj.transform.position = new Vector3(selectedObj.transform.position.x, selectedObj.transform.position.y, 89);

                        GameManager.liveModeDisabled = true;
                    }
                }
            }
            // regardless of whether mouse button has just been pressed/released, set selectedObj position if there is one
            if (selectedObj)
            {
                GameManager.liveModeDisabled = false;
                selectedObj.transform.position = new Vector3(mousePos.x, mousePos.y, selectedObj.transform.position.z);
                for (int currentObjIndex = 0; currentObjIndex < movableObjectList.Count; currentObjIndex++)
                {
                    float posX = movableObjectList[currentObjIndex].transform.position.x;
                    float posY = movableObjectList[currentObjIndex].transform.position.y;
                    Collider2D selectedObjCollider = movableObjectList[currentObjIndex].GetComponent<Collider2D>();
                    Vector2 size = selectedObjCollider.bounds.size;
                    List<Collider2D> results = new List<Collider2D>();
                    ContactFilter2D filter = new ContactFilter2D();
                    int numOfCollisions = Physics2D.OverlapBox(new Vector2(posX, posY), size, 0, filter.NoFilter(), results);
                    if ((numOfCollisions > 0 && results[0] != selectedObjCollider) || numOfCollisions > 1)
                    {
                        Debug.Log("object collision detected");
                        movableObjectList[currentObjIndex].GetComponent<SpriteRenderer>().color = Color.red;

                        GameManager.liveModeDisabled = true;
                    }
                    else
                    {
                        movableObjectList[currentObjIndex].GetComponent<SpriteRenderer>().color = Color.white;
                    }

                    /*for (int comparedObjIndex = currentObjIndex + 1; comparedObjIndex < movableObjectList.Count; comparedObjIndex++)
                    {
                        // check if objects overlap
                        GameObject firstObj = movableObjectList[currentObjIndex];
                        GameObject secondObj = movableObjectList[comparedObjIndex];
                    }*/
                }
            }
            // if mouse button has been released and object is selected, deselect it
            if (Input.GetMouseButtonUp(0) && selectedObj)
            {
                // replace this logic with using list of objects and checking their collision
                // remove grid system rounding position values to multiples of gridCellSize
                /*
                float posX = selectedObj.transform.position.x;
                float posY = selectedObj.transform.position.y;
                //selectedObj.transform.position = new Vector3(posX, posY, selectedObj.transform.position.z);

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
                    selectedObj.transform.position = new Vector3(posX, posY, selectedObj.transform.position.z);
                }*/

                /*
                GameManager.liveModeDisabled = false;                
                for (int currentObjIndex = 0; currentObjIndex < movableObjectList.Count; currentObjIndex++)
                {
                    float posX = movableObjectList[currentObjIndex].transform.position.x;
                    float posY = movableObjectList[currentObjIndex].transform.position.y;
                    Collider2D selectedObjCollider = movableObjectList[currentObjIndex].GetComponent<Collider2D>();
                    Vector2 size = selectedObjCollider.bounds.size;
                    List<Collider2D> results = new List<Collider2D>();
                    ContactFilter2D filter = new ContactFilter2D();
                    int numOfCollisions = Physics2D.OverlapBox(new Vector2(posX, posY), size, 0, filter.NoFilter(), results);
                    if ((numOfCollisions > 0 && results[0] != selectedObjCollider) || numOfCollisions > 1)
                    {
                        Debug.Log("object collision detected");
                        movableObjectList[currentObjIndex].GetComponent<SpriteRenderer>().color = Color.red;

                        GameManager.liveModeDisabled = true;
                    }
                    else
                    {
                        movableObjectList[currentObjIndex].GetComponent<SpriteRenderer>().color = Color.white;
                    }

                    /*for (int comparedObjIndex = currentObjIndex + 1; comparedObjIndex < movableObjectList.Count; comparedObjIndex++)
                    {
                        // check if objects overlap
                        GameObject firstObj = movableObjectList[currentObjIndex];
                        GameObject secondObj = movableObjectList[comparedObjIndex];
                    }
                }*/
                if (GameManager.liveModeDisabled)
                {
                    objOverlapWarningText.SetActive(true);
                }
                else
                {
                    objOverlapWarningText.SetActive(false);
                }

                selectedObj = null;
            }
            /*
            // regardless of whether mouse button has just been pressed/released, set selectedObj position if there is one
            if (selectedObj)
            {
                GameManager.liveModeDisabled = false;
                selectedObj.transform.position = new Vector3(mousePos.x, mousePos.y, selectedObj.transform.position.z);
                for (int currentObjIndex = 0; currentObjIndex < movableObjectList.Count; currentObjIndex++)
                {
                    float posX = movableObjectList[currentObjIndex].transform.position.x;
                    float posY = movableObjectList[currentObjIndex].transform.position.y;
                    Collider2D selectedObjCollider = movableObjectList[currentObjIndex].GetComponent<Collider2D>();
                    Vector2 size = selectedObjCollider.bounds.size;
                    List<Collider2D> results = new List<Collider2D>();
                    ContactFilter2D filter = new ContactFilter2D();
                    int numOfCollisions = Physics2D.OverlapBox(new Vector2(posX, posY), size, 0, filter.NoFilter(), results);
                    if ((numOfCollisions > 0 && results[0] != selectedObjCollider) || numOfCollisions > 1)
                    {
                        Debug.Log("object collision detected");
                        movableObjectList[currentObjIndex].GetComponent<SpriteRenderer>().color = Color.red;

                        GameManager.liveModeDisabled = true;
                    }
                    else
                    {
                        movableObjectList[currentObjIndex].GetComponent<SpriteRenderer>().color = Color.white;
                    }

                    /*for (int comparedObjIndex = currentObjIndex + 1; comparedObjIndex < movableObjectList.Count; comparedObjIndex++)
                    {
                        // check if objects overlap
                        GameObject firstObj = movableObjectList[currentObjIndex];
                        GameObject secondObj = movableObjectList[comparedObjIndex];
                    }
                }                
            }
            */
        }
    }

    public void AttachToMouse(GameObject item)
    {
        selectedObj = item.transform.gameObject;
    }
}
