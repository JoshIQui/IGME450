using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

enum ForceDirection
{
    Left = -1,
    Right = 1
}

public class Ball : MonoBehaviour
{
    [SerializeField] private float horizontalForceVelocty = 5.0f;
    [SerializeField] private float springForce = 10.0f;

    [SerializeField] private int nextLevelIndex;

    [SerializeField] private StarCounter starCounter;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip backgroundSong;
    [SerializeField] private AudioClip springSound;
    [SerializeField] private AudioClip windSound;
    [SerializeField] private AudioClip starSound;
    [SerializeField] private AudioClip victorySound;


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
        else
        {
            // Movement
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }
        if (gameObject.transform.position.y < -10)
        {
            GameManager.instance.UpdateGameState(GameManager.GameState.Building);
        }
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
            rb.gravityScale *= -1;
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
