using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Game Variables
    public DiskController[] disks;
    public int gameTime = 120;
    public GameObject gameOverPanel;
    public GameObject statsPanel;
    public GameObject infoPanel;
    public GameObject exitPanel;
    public Image timerBar;
    public Text timerText;
    public Text gameOverText;
    public Text resultsText;
    public Text movesText;
    public StickController initialStick;
    public StickController winningStick;
    public EffectsManager effectsManager;

    // Game over and game paused checks
    public bool gameOver   = false;
    public bool gamePaused = false;

    float _timeLeft;
    int _moves;
    string _gameOverTxt;
    string _resultTxt;

    // Singleton instance
    public static GameManager instance = null;
    
    // Use this for initialization
    void Awake ()
    {
        // Singleton initialization
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
        // Hide the game over and info UI
        gameOverPanel.SetActive(false);
        infoPanel.SetActive(false);
        exitPanel.SetActive(false);
        
        _timeLeft = gameTime;

        _moves = 0;

        // Register to the dropped event to increment the moves counter
        foreach (var disk in disks)
        {
            disk.DroppedSuccess += Disk_Dropped; ;
        }

        // Initialize the initial stick with the list of disks
        initialStick.Initialize(disks);
    }

    // Update is called once per frame
    void Update()
    {
        if (gamePaused || gameOver)
        {
            // Don't execute an logic if game is paused or over
            return;
        }

        // Decrease the timer
        _timeLeft -= Time.deltaTime;

        // Update the UI
        timerText.text = GetFormattedTime(_timeLeft);
        if (_timeLeft < 11) timerText.color = Color.red; //color timer red if less than 10s
        movesText.text = _moves.ToString();
        timerBar.fillAmount = _timeLeft / gameTime;

        if (CheckIfGameOver())
        {
            // Game is over
            gameOver = true;

            // Show game over screen and hide other UI
            timerText.text = string.Empty;
            gameOverText.text = _gameOverTxt;
            resultsText.text = _resultTxt;
            gameOverPanel.SetActive(true);
            statsPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Quit the game, ask confirmation if necessary
    /// </summary>
    public void ExitGame(bool askConfirmation)
    {
        if (askConfirmation)
        {
            // Show exit panel
            exitPanel.SetActive(true);
        }
        else
        {
            Application.Quit();
        }        
    }

    /// <summary>
    /// Replay the default scene
    /// </summary>
    public void ReplayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Show or hide the information panel. Also pause the game (or resume it while hiding the panel)
    /// </summary>
    /// <param name="show"></param>
    public void ToggleInfoPanel(bool show)
    {
        // Pause/Resume the game
        gamePaused = show;

        // Show/Hide the information panel
        infoPanel.SetActive(show);
    }

    void Disk_Dropped(object sender, EventArgs e)
    {
        // Increase moves when a disk is dropped on a stick
        _moves++;

        // and play a sound
        effectsManager.PlayDropSound();
    }
    
    string GetFormattedTime(float timeLeft)
    {
        // translate in minuts and seconds the remaining time
        TimeSpan t = TimeSpan.FromSeconds(timeLeft);
        string min = t.Minutes != 0 ? string.Format("{0:D2}m ", t.Minutes) : string.Empty;
        string sec = string.Format("{0:D2}s", t.Seconds);
        return string.Format("{0}{1}", min, sec);
    }

    bool CheckIfGameOver()
    {
        if (_timeLeft < 0)
        {
            // Set timeout text
            _gameOverTxt = "Time Expired :(";
            _resultTxt = string.Format("Total Moves {0}", _moves);
            effectsManager.PlayEndSound();
            
            return true;
        }

        // All the disks are stacked in the winning stick
        if (winningStick.DiskCount() == disks.Length)
        {
            // Set winning text
            _gameOverTxt = "Congratulations :)";
            _resultTxt = string.Format("Time {0}{1}Total Moves {2}", GetFormattedTime(gameTime - _timeLeft), Environment.NewLine, _moves);
            effectsManager.PlayWinningSounds();

            return true;
        }

        return false;
    }
}
