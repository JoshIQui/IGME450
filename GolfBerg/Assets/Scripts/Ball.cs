using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ForceDirection
{
    Left = -1,
    Right = 1
}

public class Ball : MonoBehaviour
{
    [SerializeField] private float horizontalForceVelocty = 1.0f;

    private bool horizontalForce = false;
    private ForceDirection forceDirection = ForceDirection.Left;

    private Vector2 startPosition;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = GameObject.Find("Start").transform.position;
        rb = GetComponent<Rigidbody2D>();

        // Finally, set the object position to the start position
        rb.MovePosition(startPosition);
    }

    // Update is called once per frame
    void Update()
    {
        if (horizontalForce)
        {
            // Movement augmented by horizontal force
            float addedForce = horizontalForceVelocty * (int)forceDirection;
            rb.velocity = new Vector2(rb.velocity.x + addedForce * Time.deltaTime, rb.velocity.y);
        }
        else
        {
            // Movement
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "End")
        {
            //EndLevel();
            print("Level End Here");
        }

        if (collision.gameObject.tag == "InvertedGravity")
        {
            // Inverts the gravity when entering an inverted gravity zone
            rb.gravityScale *= -1;
        }

        if (collision.gameObject.tag == "LeftForce")
        {
            // Sets horizontalForce to true and sets forceDirection to Left
            horizontalForce = true;
            forceDirection = ForceDirection.Left;
        }

        if (collision.gameObject.tag == "RightForce")
        {
            // Sets horizontalForce to true and sets forceDirection to Right
            horizontalForce = true;
            forceDirection = ForceDirection.Right;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "InvertedGravity")
        {
            // Reverts the inverted gravity when exiting an inverted gravity zone
            rb.gravityScale *= -1;
        }

        if (collision.gameObject.tag == "LeftForce")
        {
            // Sets horizontalForce to false
            horizontalForce = false;
        }

        if (collision.gameObject.tag == "RightForce")
        {
            // Sets horizontalForce to false
            horizontalForce = false;
        }
    }
}
