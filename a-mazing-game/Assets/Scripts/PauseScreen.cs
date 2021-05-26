using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    bool isPaused;
    private PlayerStats playerStats;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isPaused)
            {

                pauseGame();
            }
            else
            {
                resumeGame();
            }
        }
    }

    public void resumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        gameObject.SetActive(false);

    }

    public void pauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        gameObject.SetActive(true);

    }

    public void returnToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");

    }

}
