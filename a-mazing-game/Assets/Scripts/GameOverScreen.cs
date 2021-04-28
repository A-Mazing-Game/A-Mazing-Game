using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine.Serialization;


[RequireComponent(typeof(MazeConstructor))]


public class GameOverScreen : MonoBehaviour
{
    public Text pointsText;
    public Text timeText;
    // Start is called before the first frame update
    public void Setup(int score, System.TimeSpan time)
    {
        gameObject.SetActive(true);
        pointsText.text = "Enemies Killed: " + score.ToString();
        print(time.TotalSeconds);
        timeText.text = "Time: " + time.TotalSeconds.ToString();
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("Scene");
    }

}
