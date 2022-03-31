using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject level1Button;
    private List<string> unlockedLevels = new List<string>();
    [SerializeField] private float buttonGapMultiplier = 1;

    // Start is called before the first frame update
    void Start()
    {
        // get list of unlocked levels (for now, manually add placeholders)
        try {
            StreamReader input = new StreamReader("UnlockedLevels.txt");
            string line = null;
            while ((line = input.ReadLine()) != null)
            {
                unlockedLevels.Add(line);
            }
            input.Close();
        }
        catch { }

        // get position of original button
        float originalButtonPosY = level1Button.GetComponent<RectTransform>().anchoredPosition.y;
        float buttonGapDistance = originalButtonPosY;

        // add onclick function to original button
        level1Button.GetComponent<Button>().onClick.AddListener(delegate { btnClicked("Level1"); });

        // loop through unlocked list and instantiate buttons for all of them (as children of content)
        int iteration = 1;
        foreach(string level in unlockedLevels)
        {
            // create button and set text
            GameObject newButton = Instantiate(level1Button, content.transform);
            newButton.GetComponentInChildren<Text>().text = string.Join("l ", level.Split('l'));

            // set position of new button
            newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, (originalButtonPosY + buttonGapDistance * buttonGapMultiplier * iteration), 0);

            // add onclick function to new button
            newButton.GetComponent<Button>().onClick.AddListener(delegate { btnClicked(level); });

            iteration++;
        }

        content.GetComponent<RectTransform>().sizeDelta = new Vector2(400, -originalButtonPosY * buttonGapMultiplier * iteration + 10);
    }

    void btnClicked(string levelSceneName)
    {
        SceneManager.LoadScene(levelSceneName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
