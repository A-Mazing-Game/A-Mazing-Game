using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tutorial : MonoBehaviour
{

    public MazeConstructor mz;

    public bool weaponPickup;
    public bool powerUpPickup;
    public bool encounteredCombat;
    public bool enemyDeath;
    public GameObject tutorialScreen;
    public Text tutorialText;
    

    void Start()
    {
        weaponPickup = false;
        powerUpPickup = false;
        encounteredCombat = false;
        enemyDeath = false;
    }
    public void tutorialStartMessage()
    {
        /*
         * Introduce the game and the main goal (to make it to the boss portal in the opposite corner).
         * Prompt player to look down and pick up their weapon of choice.
         */

        Cursor.lockState = CursorLockMode.None;
        //Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;
        tutorialText.text = "Welcome to the A-Maze-ing Game Tutorial Level. Your main goal will be to make it to the " +
            "boss portal in the opposite corner. When you reach this portal in the tutorial level, the tutorial" +
            "will conclude. Look Down and pick up a weapon";
        tutorialScreen.SetActive(true);

        Debug.Log("tutorialStartMessage");
    }

    public void continueTutorial()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        tutorialScreen.SetActive(false);

    }

    public void onWeaponPickUp()
    {
        /*
         * Once a weapon is picked up: Walk through the basic controls (WASD movement, running, placing torches,
         * and pausing the game)
         */

        if (!weaponPickup)
        {
            Debug.Log("onWeaponPickUp");
            weaponPickup = true;
        }
       
    }

    public void onPowerUpPickUp()  // make sure to tell player to pick up blue potion
    {
        /*
         * Direct player to potions down main hallway (once picked up):
         * Talk about each function and how to use them (1-9 keys)
         */

        if (!powerUpPickup)
        {
            Debug.Log("onPowerUpPickUp");
            powerUpPickup = true;
        }
        else
        {
            StartCoroutine(miniMap());
        }
    }

    public IEnumerator miniMap()
    {
        /*
         * Either immediately after, or when the player walks a bit further:
         * Introduce the minimap and what to look for. Point out a red dot, and prompt the player to walk over to it.
         */

        yield return new WaitForSeconds(1);
        Debug.Log("miniMap");
        StartCoroutine(mz.GetComponent<MazeConstructor>().SpawnCoRoutine());
        StopCoroutine(miniMap());
    }

    public void combat()
    {
        /*
         * Once enemy is in fighting range: Pause and introduce combat controls (left click to attack, right click
         * to roll, run and left click to heavy attack)
         */

        if (!encounteredCombat)
        {
            Debug.Log("combat");
            encounteredCombat = true;
        }
    }

    public void onEnemyDeath()
    {
        /*
         * Once player kills first skeleton: Point to the coin counter after the player picked up the coins
         */

        if (!enemyDeath)
        {
            Debug.Log("onEnemyDeath");
            enemyDeath = true;
        }

        StartCoroutine(portal());
    }

    public IEnumerator portal()
    {
        /*
         * Immediately after: Prompt the player to make it to the portal to finish the tutorial
         */
        
        yield return new WaitForSeconds(2);
        Debug.Log("portal");
        StopCoroutine(portal());
    }

    public void endGame()
    {
        /*
         * Once player reaches portal: End the tutorial, tell player to try easy or medium maze for practice,
         * and hard for a good challenge.
         */
        
        Debug.Log("endGame");
    }

}
