using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        direction = this.transform.up;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ball")
        {
            collision.gameObject.GetComponent<Ball>().ConveyorMove(direction);
        }
    }
}
