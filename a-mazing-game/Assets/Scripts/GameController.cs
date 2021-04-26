using System;
using System.Security.Cryptography;
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
    private int health;

    private int score;
    private bool goalReached;

    //3
    void Start() {
        generator = GetComponent<MazeConstructor>();
        StartNewGame();
    }

    //4
    private void StartNewGame()
    {
        timeLimit = 10;
        reduceLimitBy = 5;
        startTime = DateTime.Now;
        health = 5;

        score = 0;
        scoreLabel.text = score.ToString();

        StartNewMaze();
    }

    //5
    private void StartNewMaze()
    {
        generator.GenerateNewMaze(7, 5, OnStartTrigger, reduceHealth);

        float x = generator.startCol * generator.hallWidth;
        float y = 1;
        float z = generator.startRow * generator.hallWidth;
        player.transform.position = new Vector3(x, y, z);

        goalReached = false;
        player.enabled = true;

        // restart timer
        timeLimit -= reduceLimitBy;
        startTime = DateTime.Now;
    }

    void reduceHealth(GameObject trigger, GameObject other)
    {
        /*
         * Reduces the player's health
         */
        
        
        health -= 5;
        // goalReached = true;  // todo remove
        healthLabel.text = health.ToString();
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

        if (health <= 0)
        {
            healthLabel.text = "You have died!";
            // player.enabled = false;
        }
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
