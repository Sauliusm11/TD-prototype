using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// Lives tracking class
/// Should only be attached to the LivesHandler object
/// </summary>
public class Lives : MonoBehaviour
{
    int lives;
    GameManager gameManager;
    TMP_Text livesDisplay;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        ResetLives();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Reset current lives
    /// TODO: will be used by load level method
    /// </summary>
    public void ResetLives()
    {
        lives = 20;
        UpdateLivesDisplay();
    }
    /// <summary>
    /// Checks if the player has enough lives to survive an enemy
    /// TODO: might want to make it return false if it hits 0
    /// </summary>
    /// <param name="cost">Lives cost of the enemy</param>
    /// <returns>True if the player has enough lives, false if not</returns>
    public bool HasEnoughLives(int cost)
    {
        return lives >= cost;
    }
    /// <summary>
    /// Called by an enemy that has reached the castle
    /// </summary>
    /// <param name="cost">Lives cost of the enemy</param>
    /// <returns>If the player has enough lives</returns>
    public bool RemoveLives(int cost)
    {
        if (HasEnoughLives(cost))
        {
            lives -= cost;
            UpdateLivesDisplay();
            return true;
        }
        gameManager.ActivateGameOver();
        return false;

    }
    /// <summary>
    /// Update the lives display text in the ui
    /// </summary>
    void UpdateLivesDisplay()
    {
        if (livesDisplay == null)
        {
            livesDisplay = GameObject.Find("LivesCount").GetComponent<TMP_Text>();
        }
        livesDisplay.text = lives.ToString();
    }
}
