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
    public GameObject player;
    public GameObject controller;
    public Text pointsText;
    public Text timeText;
    public Text coinsCollected;
    public Text remainingCoins;
    public Text healthStat;
    public BaseStats stats;
    private int maxHealth;
    private int numCoins;

    // Start is called before the first frame update

    private void Awake()
    {
        //player = GameObject.Find("Player");
        //controller = GameObject.Find("Controller");

    }
    public void Setup()
    {
        gameObject.SetActive(true);
        pointsText.text = "Enemies Killed: " + player.GetComponent<PlayerStats>().enemiesKilled.ToString();
        timeText.text = "Time: ";
        maxHealth = controller.GetComponent<BaseStats>().maxHealth;
        numCoins = player.GetComponent<Inventory>().numCoins;
        healthStat.text = maxHealth.ToString();
        remainingCoins.text = numCoins.ToString();
    }

    public void TakeDown()
    {
        gameObject.SetActive(false);

    }


    public void RestartButton()
    {
        SceneManager.LoadScene("Scene");
    }

    public void increaseHealth()
    {
        maxHealth++;
        numCoins--;
        remainingCoins.text = numCoins.ToString();
        healthStat.text = maxHealth.ToString();

    }

}
