using System;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(MazeConstructor))]

public class GameController : MonoBehaviour
{
    //1
    [SerializeField] private FpsMovement player;
    [FormerlySerializedAs("timeLabel")] [SerializeField] private Text healthLabel;
    [SerializeField] private Text scoreLabel;

    private MazeConstructor generator;

    //2
    private DateTime startTime;
    private int timeLimit;
    private int reduceLimitBy;
    public int health;
    public int maxHealth;
    private DateTime endTime;
    public TimeSpan elapsed;

    private bool showingEnd;
    public GameOverScreen GameOverScreen;
    public HealthBar healthBar;

    private int score;
    private bool goalReached;
    public int test = 1;

    public float x;

    public float y;

    public float z;
    //3
    void Start() {
        //maxHealth = 10;
        generator = GetComponent<MazeConstructor>();
        StartNewGame();
    }

    //4
    private void StartNewGame()
    {
        timeLimit = 10;
        reduceLimitBy = 5;
        startTime = DateTime.Now;
        //healthBar.SetMaxHealth(maxHealth);

        score = 0;
        scoreLabel.text = score.ToString();
        healthLabel.text = player.GetComponent<PlayerCombat>().maxHealth.ToString() + "/" + player.GetComponent<PlayerCombat>().maxHealth.ToString();

        StartNewMaze();
    }

    public void movePlayer()
    {
        x = generator.startCol * generator.hallWidth;
        y = 1;
        z = generator.startRow * generator.hallWidth;
        player.transform.position = new Vector3(z, y, z);

    }

    //5
    private void StartNewMaze()
    {
        generator.GenerateNewMaze(11, 11, OnStartTrigger, reduceHealth, endGame) ;
        x = generator.startCol * generator.hallWidth;
        y = 1;
        z = generator.startRow * generator.hallWidth;
        
        // Thread.Sleep(10000);
        Debug.Log("start row " + x + " start col " + z);
        movePlayer();
        
        goalReached = false;
        showingEnd = false;
        player.enabled = true;

        // restart timer
        timeLimit -= reduceLimitBy;
        startTime = DateTime.Now;
    }

    public void endGame(GameObject trigger, GameObject other)
    {
        /*
         * This method will end the game when the end of the
         * maze object is touched
         */

        Debug.Log(other.tag);
        if (other.tag == "Player")
        {
            healthLabel.text = "You have reached the end of the maze!";
            player.enabled = false;
            Destroy(trigger);
            endTime = DateTime.Now;
            elapsed = endTime - startTime;
            score = player.GetComponent<PlayerCombat>().score;

            //GameOverScreen.Setup(score, elapsed);
        }
        
    }
    
    void reduceHealth(GameObject trigger, GameObject other)
    {
        /*
         * Reduces the player's health
         */
        
        
        //health -= 5;
        //healthBar.SetHealth(health);
        // goalReached = true;  // todo remove
        //healthLabel.text = health.ToString();
        Debug.Log("took 5 damage");
        Destroy(trigger);
        // Invoke("StartNewGame", 1);
        
    }

    //6
    private void Update()
    {
        if (!player.enabled)
        {
            return;
        }
        score = player.GetComponent<PlayerCombat>().score;
        scoreLabel.text = score.ToString();
        healthLabel.text = player.GetComponent<PlayerCombat>().currentHealth.ToString() + "/" + player.GetComponent<PlayerCombat>().maxHealth.ToString();

        /*if (health <= 0)
        {
            healthLabel.text = "You have died!";
            endTime = DateTime.Now;
            elapsed = endTime - startTime;

            if (!showingEnd)
            {
                GameOverScreen.Setup(score, 5);
                showingEnd = true;
            }
            // player.enabled = false;
        }*/
        // Invoke("StartNewGame", 4);
    }

    //7
    private void OnGoalTrigger(GameObject trigger, GameObject other)
    {
        Debug.Log("Goal!");
        goalReached = true;

        score += 1;
        scoreLabel.text = score.ToString();
        

        Destroy(trigger);
    }

    private void OnStartTrigger(GameObject trigger, GameObject other)
    {
        if (goalReached)
        {
            Debug.Log("Finish!");
            player.enabled = false;

            Invoke("StartNewMaze", 4);
        }
    }
}
