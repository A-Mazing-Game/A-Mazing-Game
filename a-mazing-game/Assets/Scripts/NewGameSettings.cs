
using System.Collections;
using System.Threading;
using Maze;
using Maze.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameSettings : MonoBehaviour
{

    AsyncOperation asyncLoadLevel;
    float sliderSensitivity;

    /// <summary>
    /// The default mouse sensitivity
    /// </summary>
    private const float DefaultMouseSensitivity = 2f;
    public void smallGame()
    {
        sliderSensitivity = PlayerPrefs.GetFloat("sensitivity", DefaultMouseSensitivity);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("continue", 1);
        PlayerPrefs.SetFloat("sensitivity", sliderSensitivity);
        AudioListener.pause = false;
        StartCoroutine(LoadLevel());

        NewMazeConfiguration.SetMazeType(MazeTypeEnum.Small);
        NewMazeConfiguration.SetRows(13);
        NewMazeConfiguration.SetColumns(13);
    }

    public void mediumGame()
    {
        sliderSensitivity = PlayerPrefs.GetFloat("sensitivity", DefaultMouseSensitivity);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("continue", 1);
        PlayerPrefs.SetFloat("sensitivity", sliderSensitivity);
        StartCoroutine(LoadLevel());

        NewMazeConfiguration.SetMazeType(MazeTypeEnum.Medium);
        NewMazeConfiguration.SetRows(25);
        NewMazeConfiguration.SetColumns(25);
    }

    public void largeGame()
    {
        sliderSensitivity = PlayerPrefs.GetFloat("sensitivity", DefaultMouseSensitivity);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("continue", 1);
        PlayerPrefs.SetFloat("sensitivity", sliderSensitivity);
        StartCoroutine(LoadLevel());

        NewMazeConfiguration.SetMazeType(MazeTypeEnum.Large);
        NewMazeConfiguration.SetRows(37);
        NewMazeConfiguration.SetColumns(37);
    }

    public void tutorialLevel()
    {
        sliderSensitivity = PlayerPrefs.GetFloat("sensitivity", DefaultMouseSensitivity);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("continue", 1);
        PlayerPrefs.SetFloat("sensitivity", sliderSensitivity);
        StartCoroutine(LoadLevel());

        NewMazeConfiguration.SetMazeType(MazeTypeEnum.Tutorial);
        NewMazeConfiguration.SetRows(13);
        NewMazeConfiguration.SetColumns(13);
    }

    IEnumerator LoadLevel()
    {
        asyncLoadLevel = SceneManager.LoadSceneAsync("Scene");
        while (!asyncLoadLevel.isDone)
        {
            // print("Loading the Scene");
            yield return null;
        }
        Thread.Sleep(2000);
    }
}

