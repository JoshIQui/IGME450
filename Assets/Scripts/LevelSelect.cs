using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        unlockedLevels.Add("Level-2");
        unlockedLevels.Add("Level-3");
        unlockedLevels.Add("Level-4");
        unlockedLevels.Add("Level-5");
        unlockedLevels.Add("Level-6");

        // get position of original button
        float originalButtonPosY = level1Button.GetComponent<RectTransform>().anchoredPosition.y;
        float buttonGapDistance = originalButtonPosY;

        // add onclick function to original button

        // loop through unlocked list and instantiate buttons for all of them (as children of content)
        int iteration = 1;
        foreach(string level in unlockedLevels)
        {
            // create button and set text
            GameObject newButton = Instantiate(level1Button, content.transform);
            newButton.GetComponentInChildren<Text>().text = string.Join(" ", level.Split('-'));

            // set position of new button
            newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, (originalButtonPosY + buttonGapDistance * buttonGapMultiplier * iteration), 0);

            // add onclick function to new button

            iteration++;

            Debug.Log(originalButtonPosY);
            Debug.Log(buttonGapDistance * buttonGapMultiplier * iteration);
        }

        content.GetComponent<RectTransform>().sizeDelta = new Vector2(400, -originalButtonPosY * buttonGapMultiplier * iteration + 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
