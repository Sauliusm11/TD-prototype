using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Lives : MonoBehaviour
{
    int lives;
    TMP_Text livesDisplay;
    // Start is called before the first frame update
    void Start()
    {
        ResetLives();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ResetLives()
    {
        lives = 20;
        UpdateLivesDisplay();
    }
    public bool HasEnoughLives(int cost)
    {
        return lives >= cost;
    }
    public bool RemoveLives(int cost)
    {
        if (HasEnoughLives(cost))
        {
            lives -= cost;
            UpdateLivesDisplay();
            return true;
        }
        return false;

    }
    void UpdateLivesDisplay()
    {
        if (livesDisplay == null)
        {
            livesDisplay = GameObject.Find("LivesCount").GetComponent<TMP_Text>();
        }
        livesDisplay.text = lives.ToString();
    }
}
