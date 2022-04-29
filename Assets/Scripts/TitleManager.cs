using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    private void Start()
    {
        // if final build hasn't been opened before on user's computer, wipe pre-existing level unlocks data
        if (PlayerPrefs.GetString("IGME450_FinalBuildPlayed") == "" || PlayerPrefs.GetString("IGME450_FinalBuildPlayed") == null)
        {
            PlayerPrefs.SetString("IGME450_UnlockedLevels", "");
            PlayerPrefs.SetString("IGME450_FinalBuildPlayed", "true");
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
