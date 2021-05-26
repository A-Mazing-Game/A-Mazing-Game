using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public Button continueGame;
    private int flag;
    AsyncOperation asyncLoadLevel;


    private void Start()
    {
        flag = PlayerPrefs.GetInt("continue", 0);
        if (flag == 1)
        {
            continueGame.enabled = true;
        }
        else
        {
            continueGame.enabled = false;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Scene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void smallGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("rows", 13);
        PlayerPrefs.SetInt("cols", 13);
        PlayerPrefs.SetInt("continue", 1);
        StartCoroutine(LoadLevel());


    }

    public void mediumGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("rows", 25);
        PlayerPrefs.SetInt("cols", 25);
        PlayerPrefs.SetInt("continue", 1);
        StartCoroutine(LoadLevel());

    }

    public void largeGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("rows", 37);
        PlayerPrefs.SetInt("cols", 37);
        PlayerPrefs.SetInt("continue", 1);
        StartCoroutine(LoadLevel());

    }

    IEnumerator LoadLevel()
    {
        asyncLoadLevel = SceneManager.LoadSceneAsync("Scene");
        while (!asyncLoadLevel.isDone)
        {
            print("Loading the Scene");
            yield return null;
        }
    }

}
