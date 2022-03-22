using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState state;
    public static event Action<GameState> OnGameStateChanged;

    [SerializeField] private GameObject ball;
    private GameObject[] stars;

    private bool sceneReset = true;

    public static bool objectsColliding = false;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.Building);
        sceneReset = false;
        stars = GameObject.FindGameObjectsWithTag("Star");
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (state == GameState.Building && !objectsColliding)
            {
                UpdateGameState(GameState.Live);
            }
            else if (state == GameState.Live)
            {
                ball.GetComponent<Ball>().ResetPosition();
                UpdateGameState(GameState.Building);
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;
        switch(newState)
        {
            case GameState.Building:
                if (!sceneReset)
                {
                    ball.SetActive(true);
                    ball.GetComponent<Ball>().ResetPosition();
                    ball.SetActive(false);
                    foreach (GameObject star in stars)
                    {
                        star.SetActive(true);
                    }
                }
                break;
            case GameState.Live:
                ball.SetActive(true);
                ball.GetComponent<Ball>().ResetPosition();
                //gameObject.SetActive(false);
                break;
            case GameState.End:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    public enum GameState
    {
        Building,
        Live,
        End
    }
}
