using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState state;
    private GameState beforePausedState;
    public static event Action<GameState> OnGameStateChanged;
    public GameObject finishPanel;
    public GameObject pausePanel;

    [SerializeField] private GameObject ball;
    private GameObject[] stars;

    [SerializeField] private StarCounter starCounter;

    private bool sceneReset = true;

    public static bool liveModeDisabled = false;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Debug.Log("reset");
        UpdateGameState(GameState.Building);
        sceneReset = false;
        stars = GameObject.FindGameObjectsWithTag("Star");
        starCounter = GameObject.Find("StarCounter").GetComponentInChildren<StarCounter>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P) && state != GameState.Pause && state != GameState.End)
        {
            if (state == GameState.Building && !liveModeDisabled)
            {
                UpdateGameState(GameState.Live);
            }
            else if (state == GameState.Live)
            {
                ball.GetComponent<Ball>().ResetPosition();
                UpdateGameState(GameState.Building);
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && state != GameState.Pause && state != GameState.End)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (Input.GetKeyDown(KeyCode.Escape) && state != GameState.Pause && state != GameState.End)
        {
            beforePausedState = state;
            UpdateGameState(GameState.Pause);
        }
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RetryLevel()
    {
        ball.GetComponent<Ball>().ResetPosition();
        UpdateGameState(GameState.Building);
        GameObject.Find("Play Button").GetComponent<PlayButton>().ChangeText();
    }

    public bool Play()
    {
        if (state == GameState.Building && !liveModeDisabled)
        {
            UpdateGameState(GameState.Live);
            return true;
        }
        else if (state == GameState.Live)
        {
            ball.GetComponent<Ball>().ResetPosition();
            UpdateGameState(GameState.Building);
            return true;
        }
        return false;
    }

    public void UnPause()
    {
        pausePanel.SetActive(false);
        UpdateGameState(beforePausedState);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;
        switch(newState)
        {
            case GameState.Building:
                finishPanel.SetActive(false);
                if (!sceneReset)
                {
                    ball.SetActive(true);
                    ball.GetComponent<Ball>().ResetPosition();
                    ball.SetActive(false);
                    foreach (GameObject star in stars)
                    {
                        star.SetActive(true);
                    }
                    starCounter.ResetStarCount();
                }
                break;
            case GameState.Live:
                finishPanel.SetActive(false);
                ball.SetActive(true);
                ball.GetComponent<Ball>().ResetPosition();
                break;
            case GameState.Pause:
                finishPanel.SetActive(false);
                ball.SetActive(false);
                pausePanel.SetActive(true);
                break;
            case GameState.End:
                ball.SetActive(false);
                finishPanel.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    public void LevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }


    public enum GameState
    {
        Building,
        Live,
        Pause,
        End
    }
}
