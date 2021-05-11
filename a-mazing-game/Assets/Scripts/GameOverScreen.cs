using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
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
    private PlayerStats playerStats;
    private Inventory inventory;
    private BaseStats baseStats;

    // TODO
    public NavMeshSurface surface;

    // Start is called before the first frame update

    private void Awake()
    {
        playerStats = player.GetComponent<PlayerStats>();
        baseStats = controller.GetComponent<BaseStats>();
        inventory = player.GetComponent<Inventory>();
        //player = GameObject.Find("Player");
        //controller = GameObject.Find("Controller");

    }
    public void Setup()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
        pointsText.text = "Enemies Killed: " + playerStats.enemiesKilled.ToString();
        timeText.text = "Time: ";
        maxHealth = playerStats.maxHealth;
        numCoins = inventory.numCoins;
        healthStat.text = maxHealth.ToString();
        remainingCoins.text = numCoins.ToString();
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void TakeDown()
    {
        setStats();
        PlayerPrefs.SetInt("health", maxHealth);
        gameObject.SetActive(false);

    }


    public void ContinueButton()
    {
        setStats();
        Time.timeScale = 1;
        SceneManager.LoadScene("Scene");
    }

    public void MenuButton()
    {
        setStats();
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void increaseHealth()
    {
        if (numCoins > 0)
        {
            maxHealth++;
            numCoins--;
            remainingCoins.text = numCoins.ToString();
            healthStat.text = maxHealth.ToString();
        }

    }

    private void setInventory()
    {
        //gonna add later
    }

    private void setStats()
    {
        PlayerPrefs.SetInt("health", maxHealth);
    }


}
