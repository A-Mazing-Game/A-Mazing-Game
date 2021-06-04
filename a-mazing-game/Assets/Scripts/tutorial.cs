using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class tutorial : MonoBehaviour
{

    public MazeConstructor mz;

    public bool weaponPickup;
    public bool powerUpPickup;
    public bool encounteredCombat;
    public bool enemyDeath;
    public GameObject tutorialScreen;
    public Text tutorialText;
    private int startTut;
    float sliderSensitivity;
    bool finishedTutorial;
    public bool miniMapTut;
    public bool portalTut;

    public Button MenuButton;

    public Button ContinueButton;
    AsyncOperation asyncLoadLevel;

    void Start()
    {
        weaponPickup = false;
        powerUpPickup = false;
        encounteredCombat = false;
        enemyDeath = false;
        portalTut = false;
        miniMapTut = false;
        startTut = mz.GetComponent<MazeConstructor>().loadTutorial;
        if(startTut == 1)
            Cursor.lockState = CursorLockMode.None;
        Debug.Log("start tut: " + startTut);
        finishedTutorial = false;
        MenuButton.gameObject.SetActive(false);
    }
    public void tutorialStartMessage(int load=0)
    {
        /*
         * Introduce the game and the main goal (to make it to the boss portal in the opposite corner).
         * Prompt player to look down and pick up their weapon of choice.
         */

        Debug.Log("why: " + startTut);
        if(load == 0)
            return;
        
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        tutorialText.text = "Welcome to the A-Maze-ing Game Tutorial Level. Your main goal will be to make it to the " +
            "boss portal in the opposite corner. When you reach this portal in the tutorial level, the tutorial " +
            "will conclude. Look Down and pick up a weapon, you can move the player with the WASD keys and sprint " +
            "by pressing shift while walking, note: this does use your stamina (yellow bar)!";
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

        if(startTut == 0)
            return;
        
        if (!weaponPickup)
        {
            Debug.Log("onWeaponPickUp");
            weaponPickup = true;
            
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            tutorialText.text =
                "You have picked up your first weapon! You can also place torches on the wall of the maze by pressing T to keep " +
                "track of where you have already been. The game can be paused at any time by pressing P. Now go pick " +
                "up the red and blue potions in front of you.";
            tutorialScreen.SetActive(true);
        }
       
    }

    public void onPowerUpPickUp()  // make sure to tell player to pick up blue potion
    {
        /*
         * Direct player to potions down main hallway (once picked up):
         * Talk about each function and how to use them (1-9 keys)
         */

        if(startTut == 0)
            return;
        
        if (!powerUpPickup)
        {
            Debug.Log("onPowerUpPickUp");
            powerUpPickup = true;
            
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            tutorialText.text = "There are two main potions spawned within the maze. Red potions are health potions and " +
                                "blue potions are shield potions. Shield health will be consumed before player health." +
                                " Once you pick up a spawned item, that item is placed in your inventory, you can use " +
                                "your number keys (1-9) on the keyboard to use that specific item or to swap weapons. " +
                                "Potions / pickups are spawned in dead end hallways within the maze. Be careful though, " +
                                "arrows do not glow, so they are hard to see when you're on the hunt for ammo!" +
                                " Make sure to pick up the last potion!";
                
            tutorialScreen.SetActive(true);
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

        if (!miniMapTut)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            tutorialText.text = "In the top left of the screen is the level minimap. You are the green dot in the center " +
                                "of the map. Enemies appear on the minimap as red dots and powerups appear as smaller " +
                                "dots on the map as their associated color. There are enemies on your minimap now, " +
                                "go and try to fight them!";
                
            tutorialScreen.SetActive(true);
            StartCoroutine(mz.GetComponent<MazeConstructor>().SpawnCoRoutine());
            miniMapTut = true;
        }
        
        StopCoroutine(miniMap());
    }

    public void combat()
    {
        /*
         * Once enemy is in fighting range: Pause and introduce combat controls (left click to attack, right click
         * to roll, run and left click to heavy attack)
         */

        if(startTut == 0)
            return;
        
        if (!encounteredCombat)
        {
            Debug.Log("combat");
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            tutorialText.text = "You have been spotted! Melee weapons have two attack modes, quick and " +
                                "a jumping attacks. You can press the left mouse button to perform a quick attack, " +
                                "you can also perform a jumping attack by sprinting and pressing the left mouse button at " +
                                "the same time. You can also doge enemies by pressing the right mouse button. " +
                                "The bow is a ranged weapon that needs arrows to shoot, I bet you'll find some if you" +
                                " look around!";
                
            tutorialScreen.SetActive(true);
            encounteredCombat = true;
        }
    }

    public void onEnemyDeath()
    {
        /*
         * Once player kills first skeleton: Point to the coin counter after the player picked up the coins
         */

        if(startTut == 0)
            return;
        
        if (!enemyDeath)
        {
            Debug.Log("onEnemyDeath");
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            tutorialText.text = "Nice job! Enemies also drop coins when you kill them! Coins can be used" +
                                " to purchase player upgrades such as increasing the player's max health. " +
                                "Enemies are continuously being spawned within the maze, and in greater numbers. " +
                                "Make sure you don't stay in the maze too long or you could be overrun! Or you may want " +
                                "to explore the maze some more to stockpile your potions and arrows...";
                
            tutorialScreen.SetActive(true);
            enemyDeath = true;
        }

        StartCoroutine(portal());
    }

    public IEnumerator portal()
    {
        /*
         * Immediately after: Prompt the player to make it to the portal to finish the tutorial
         */
        
        yield return new WaitForSeconds(10);
        Debug.Log("portal");

        if (!portalTut)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            tutorialText.text = "When you're ready to leave the maze, you can walk through the portal. In a normal game mode, " +
                                "once you enter the portal, you will be teleported to the boss fight. For this tutorial, " +
                                "the game will end. Have fun!";
            portalTut = true;
                
            tutorialScreen.SetActive(true);
        }
        
        StopCoroutine(portal());
    }

    public void endGame()
    {
        /*
         * Once player reaches portal: End the tutorial, tell player to try easy or medium maze for practice,
         * and hard for a good challenge.
         */
        if(startTut == 0)
            return;
        
        Cursor.lockState = CursorLockMode.None;
        ContinueButton.gameObject.SetActive(false);
        MenuButton.gameObject.SetActive(true);
        Time.timeScale = 0;
        tutorialText.text = "Congratulations! You've reached the end of the tutorial! Play the easy or medium maze for practive" +
            "or the hard mode maze for a challenge. ";
        tutorialScreen.SetActive(true);
    }

    public void backToMenu()
    {
        StartCoroutine(loadMenu());
        sliderSensitivity = PlayerPrefs.GetFloat("sensitivity", 4f);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("sensitivity", sliderSensitivity);
        Time.timeScale = 1;
        tutorialScreen.SetActive(false);

    }

    IEnumerator loadMenu()
    {
        yield return SceneManager.LoadSceneAsync("Menu");

    }

}
