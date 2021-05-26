
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameSettings : MonoBehaviour
{

    AsyncOperation asyncLoadLevel;


    // Start is called before the first frame update
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

