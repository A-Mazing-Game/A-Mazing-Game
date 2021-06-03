using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorial : MonoBehaviour
{

    public MazeConstructor mz;
    
    public void tutorialStartMessage()
    {
        /*
         * Introduce the game and the main goal (to make it to the boss portal in the opposite corner).
         * Prompt player to look down and pick up their weapon of choice.
         */
        
        Time.timeScale = 0;
    }

    public void onWeaponPickUp()
    {
        /*
         * Once a weapon is picked up: Walk through the basic controls (WASD movement, running, placing torches,
         * and pausing the game)
         */
        
        Time.timeScale = 0;
    }

    public void onPowerUpPickUp()
    {
        /*
         * Direct player to potions down main hallway (once picked up):
         * Talk about each function and how to use them (1-9 keys)
         */
        
        Time.timeScale = 0;
    }

    public void miniMap()
    {
        /*
         * Either immediately after, or when the player walks a bit further:
         * Introduce the minimap and what to look for. Point out a red dot, and prompt the player to walk over to it.
         */
        
        Time.timeScale = 0;
    }

    public void combat()
    {
        /*
         * Once enemy is in fighting range: Pause and introduce combat controls (left click to attack, right click
         * to roll, run and left click to heavy attack)
         */
        
        Time.timeScale = 0;
    }

    public void onEnemyDeath()
    {
        /*
         * Once player kills first skeleton: Point to the coin counter after the player picked up the coins
         */
        
        Time.timeScale = 0;
    }

    public void portal()
    {
        /*
         * Immediately after: Prompt the player to make it to the portal to finish the tutorial
         */
        
        Time.timeScale = 0;
    }

    public void endGame()
    {
        /*
         * Once player reaches portal: End the tutorial, tell player to try easy or medium maze for practice,
         * and hard for a good challenge.
         */
        
        Time.timeScale = 0;
    }

}
