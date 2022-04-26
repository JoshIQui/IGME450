using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

enum ForceDirection
{
    Left = -1,
    Right = 1
}

public class Ball : MonoBehaviour
{
    [SerializeField] private float verticalForceVelocty = 10.0f;
    [SerializeField] private float horizontalForceVelocty = 5.0f;
    [SerializeField] private float springForce = 10.0f;
    [SerializeField] private float conveyorVelocity = 10.0f;

    [SerializeField] private int nextLevelIndex;

    [SerializeField] private StarCounter starCounter;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip backgroundSong;
    [SerializeField] private AudioClip springSound;
    [SerializeField] private AudioClip windSound;
    [SerializeField] private AudioClip starSound;
    [SerializeField] private AudioClip victorySound;

    private bool verticalForce = false;
    private bool horizontalForce = false;
    private ForceDirection forceDirection = ForceDirection.Left;

    private Vector2 startPosition;

    private Rigidbody2D rb;

    private int stars = 0;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = GameObject.Find("Start").transform.position;
        rb = GetComponent<Rigidbody2D>();

        // Finally, set the object position to the start position
        //rb.MovePosition(startPosition);
        rb.position = startPosition;

        gameObject.SetActive(false);
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
        if (verticalForce)
        {
            // Movement augmented by vertical force
            float addedForce = verticalForceVelocty;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + addedForce * Time.deltaTime);
        }
        else
        {
            // Movement
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }
        if (gameObject.transform.position.y < -10)
        {
            GameManager.instance.UpdateGameState(GameManager.GameState.Building);
            GameObject.Find("Play Button").GetComponent<PlayButton>().ChangeText();
        }
    }

    public void ConveyorMove(Vector3 direction)
    {
        rb.velocity = new Vector2(rb.velocity.x + (direction.x * conveyorVelocity) * Time.deltaTime, rb.velocity.y + (direction.y * conveyorVelocity) * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "End")
        {
            audioSource.PlayOneShot(victorySound, 1);
            //List<string> alreadyUnlockedLevels = new List<string>();
            string unlockedLevels = "";
            try
            {
                /* StreamReader input = new StreamReader("UnlockedLevels.txt");
                string line = null;
                while ((line = input.ReadLine()) != null)
                {
                    alreadyUnlockedLevels.Add(line);
                }
                input.Close();
                */

                unlockedLevels = PlayerPrefs.GetString("IGME450_UnlockedLevels", "");
            }
            catch { }

            try
            {
                /*StreamWriter output = new StreamWriter("UnlockedLevels.txt");
                string unlockedLevels = "";
                foreach (string unlockedlevel in alreadyUnlockedLevels) 
                {
                    output.WriteLine(unlockedlevel);
                    Debug.Log(unlockedlevel);
                }
                */
                //if (nextLevelIndex != 1)
                if (nextLevelIndex != 1 && !unlockedLevels.Contains(nextLevelIndex.ToString()))
                {
                    string level = "Level" + nextLevelIndex;
                    /*if (!alreadyUnlockedLevels.Contains(level))
                    if (!unlockedLevels.Contains(level))
                    {
                        //output.WriteLine(level);
                        //Debug.Log(level);
                        unlockedLevels += level;
                    }
                    */
                    if (unlockedLevels == "")
                    {
                        unlockedLevels += level;
                    }
                    else
                    {
                        unlockedLevels += "," + level;
                    }
                }
                //output.Close();

                if (unlockedLevels != "")
                {
                    PlayerPrefs.SetString("IGME450_UnlockedLevels", unlockedLevels);
                }

                // for debugging purposes to reset level unlocks
                //PlayerPrefs.SetString("IGME450_UnlockedLevels", "");
            }
            catch { }

            GameManager.instance.UpdateGameState(GameManager.GameState.End);
        }

        if (collision.gameObject.tag == "InvertedGravity")
        {
            // Inverts the gravity when entering an inverted gravity zone
            //rb.gravityScale *= -1;
            verticalForce = true;
        }

        if (collision.gameObject.tag == "LeftForce")
        {
            audioSource.PlayOneShot(windSound, 1);
            // Sets horizontalForce to true and sets forceDirection to Left
            horizontalForce = true;
            forceDirection = ForceDirection.Left;
        }

        if (collision.gameObject.tag == "RightForce")
        {
            audioSource.PlayOneShot(windSound, 1);
            // Sets horizontalForce to true and sets forceDirection to Right
            horizontalForce = true;
            forceDirection = ForceDirection.Right;
        }

        if (collision.gameObject.tag == "Spring")
        {
            // Add a force to the object in the direction the spring is pointing
            rb.velocity = new Vector2(0, 0);
            rb.AddForce(collision.gameObject.transform.up * springForce, ForceMode2D.Impulse);
            audioSource.PlayOneShot(springSound, 1);
        }

        if (collision.gameObject.tag == "PermanentInvertedGravity")
        {
            // Inverts the gravity when entering an inverted gravity zone
            // Is not reverted back upon exit
            rb.gravityScale *= -1;
        }

        if (collision.gameObject.tag == "Star")
        {
            audioSource.PlayOneShot(starSound, 1);
            stars++;
            collision.gameObject.SetActive(false);
            starCounter.IncreaseStarCount();
            //print("Star collected");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "InvertedGravity")
        {
            // Reverts the inverted gravity when exiting an inverted gravity zone
            //rb.gravityScale *= -1;
            verticalForce = false;
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

    // code for if conveyors apply force like wind does
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "UpwardConveyor")
        {
            // Inverts the gravity when entering an inverted gravity zone
            rb.gravityScale *= -1;
        }

        if (collision.gameObject.tag == "LeftConveyor")
        {
            audioSource.PlayOneShot(windSound, 1);
            // Sets horizontalForce to true and sets forceDirection to Left
            horizontalForce = true;
            forceDirection = ForceDirection.Left;
        }

        if (collision.gameObject.tag == "RightConveyor")
        {
            audioSource.PlayOneShot(windSound, 1);
            // Sets horizontalForce to true and sets forceDirection to Right
            horizontalForce = true;
            forceDirection = ForceDirection.Right;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "UpwardConveyor")
        {
            // Reverts the inverted gravity when exiting an inverted gravity zone
            rb.gravityScale *= -1;
        }

        if (collision.gameObject.tag == "LeftConveyor")
        {
            // Sets horizontalForce to false
            horizontalForce = false;
        }

        if (collision.gameObject.tag == "RightConveyor")
        {
            // Sets horizontalForce to false
            horizontalForce = false;
        }
    }

    // code for if conveyor belts move ball at a constant speed
    //private void OnColliderStay2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "UpwardConveyor")
    //    {
    //        rb.velocity = new Vector2(0, conveyorVelocity);
    //    }

    //    if (collision.gameObject.tag == "LeftConveyor")
    //    {
    //        rb.velocity = new Vector2(-conveyorVelocity, 0);
    //    }

    //    if (collision.gameObject.tag == "RightConveyor")
    //    {
    //        rb.velocity = new Vector2(conveyorVelocity, 0);
    //    }
    //}

    //RESTART THE LEVEL IF THE OBJECT LEAVES THE SCREEN
    //private void OnBecameInvisible()
    //{
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //}

    public void ResetPosition()
    {
        //rb.MovePosition(startPosition);
        rb.position = startPosition;
    }

    public void NextLevel()
    {
        SceneManager.LoadScene("Level" + nextLevelIndex);
    }
}
