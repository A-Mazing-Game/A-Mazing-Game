using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    public bool isPaused;
    private PlayerStats playerStats;


    public void resumeGame()
    {
        print("pushed button");
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
