/*
 * written by Joseph Hocking 2017
 * released under MIT license
 * text of license https://opensource.org/licenses/MIT
 *
 * Modified VERY heavily by Jacob
 */

using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using Maze.Enums;
using Powerups;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Maze
{
    public class MazeConstructor : MonoBehaviour
    {
        public bool showDebug;
        private int[] col;
        public int[] row;
        private int[] enemies; // the spawn location of enemies
        public int length;

    

        [SerializeField] private Material mazeMat1;
        [SerializeField] private Material mazeMat2;
        public GameObject skeleton;  // Skeleton prefab
        public GameObject healthPotion;  // health prefab
        public GameObject shieldPotion;  // shield prefab
        public GameObject gate;
        private MeshRenderer mr;  // mesh renderer
        private int[] deadEndCol;  // stores dead column indices
        private int[] deadEndRow;  // stores dead end row indicies
        public int desiredEnemies;  // number of enemies to initially spawn in the maze
        public GameObject player;  // player gameobject
        private GameObject Portal => GameObject.FindGameObjectWithTag("Portal");  // portal game object to get location
        public GameObject arrow;
        private int spawnDistance;
        public GameObject mage;

        private int[,] tutorialMaze;
    
    
        private int[,] Data { get; set; }

        public float HallWidth { get; private set; }
        public int StartRow { get; private set; }
        public int StartCol { get; private set; }

        /// <summary>
        /// The type of maze to generate. <see cref="MazeTypeEnum"/>
        /// </summary>
        public MazeTypeEnum MazeType => NewMazeConfiguration.MazeType;

        private MazeDataGenerator dataGenerator;
        private MazeMeshGenerator meshGenerator;
        public tutorial tutorialScript;

        public MazeConstructor(int[,] data)
        {
            Data = data;
        }

        void Awake()
        {
            dataGenerator = new MazeDataGenerator();
            meshGenerator = new MazeMeshGenerator();
            length = 0;
            spawnDistance = 10;
            tutorialScript = GetComponent<tutorial>();
        
            switch (MazeType)
            {
                case MazeTypeEnum.Small:
                case MazeTypeEnum.Tutorial:
                    desiredEnemies = 5;
                    break;
            
                case MazeTypeEnum.Medium:
                    desiredEnemies = 10;
                    break;
            
                case MazeTypeEnum.Large:
                    desiredEnemies = 37;
                    break;
            
                // We should never get here, catch it and throw an exception
                case MazeTypeEnum.None:
                default:
                    throw new InvalidOperationException();
            
            }
            
            player = GameObject.FindGameObjectWithTag("Player");
        

            // default to walls surrounding a single empty cell
            Data = new[,]
            {
                {1, 1, 1},
                {1, 0, 1},
                {1, 1, 1}
            };
        
            /*
         * This is the tutorial maze so it is not randomly generated each time
         */
            tutorialMaze = new[,]  // use 11 for column and row
            {
                // {1,1,1,1,1,1,1,1,1,1,1,1,1},
                // {1,0,1,0,1,0,0,0,1,0,0,0,1},
                // {1,0,1,0,1,0,1,1,1,0,1,0,1},
                // {1,0,0,0,1,0,1,0,0,0,1,0,1},
                // {1,0,1,0,1,0,1,0,1,1,1,0,1},
                // {1,0,1,0,0,0,0,0,1,0,0,0,1},
                // {1,0,0,0,1,0,1,1,1,0,1,0,1},
                // {1,0,0,0,1,0,1,0,0,0,1,0,1},
                // {1,1,1,0,1,1,1,1,1,1,1,0,1},
                // {1,0,1,0,0,0,0,0,1,0,1,0,1},
                // {1,0,1,0,1,0,1,1,1,1,1,0,1},
                // {1,0,0,0,1,0,0,0,0,0,0,0,1},
                // {1,1,1,1,1,1,1,1,1,1,1,1,1}
            
                {1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,0,1,1,1,0,1,0,1,1,1,0,1},
                {1,0,1,1,1,0,1,0,1,1,1,0,1},
                {1,0,1,0,1,0,0,0,0,0,0,0,1},
                {1,0,1,0,1,1,1,0,1,1,1,1,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,1,1,0,1,0,0,0,0,0,1,1,1},
                {1,0,1,0,1,0,0,0,0,0,0,0,1},
                {1,0,1,0,1,1,1,1,1,0,0,0,1},
                {1,0,0,0,1,0,0,0,0,0,0,0,1},
                {1,0,1,1,0,0,1,0,1,0,1,0,1},
                {1,0,0,0,0,0,1,0,1,0,1,0,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1}
            
            

            };
        }

        public void GenerateNewMaze(int sizeRows, int sizeCols)
        {
            Debug.Log("Row and cols: " + sizeRows + " " + sizeCols);

            if (sizeRows % 2 == 0 && sizeCols % 2 == 0)
            {
                Debug.LogError("Odd numbers work better for dungeon size.");
            }

            DisposeOldMaze();

            switch (MazeType)
            {
                case MazeTypeEnum.Tutorial:
                    Data = tutorialMaze;
                    break;
                
                case MazeTypeEnum.Large:
                case MazeTypeEnum.Medium:
                case MazeTypeEnum.Small:
                    Data = dataGenerator.FromDimensions(sizeRows, sizeCols);
                    break;
                    
            }
        

            FindStartPosition();
            FindGoalPosition();
            FindDeadEnd();
            Debug.Log("For tut: col: " + col[0] + " row: " + row[0]);
            // printMaze();

            // store values used to generate this mesh
            HallWidth = meshGenerator.width;

            DisplayMaze();
            PlaceEndPortal(col[0], row[0]);
            SpawnPowerUp();
            
            switch (MazeType)
            {
                case MazeTypeEnum.Tutorial:
                TutorialPowerUpSpawn();
                tutorialScript.GetComponent<tutorial>().tutorialStartMessage(1);
                break;
                
                case MazeTypeEnum.Small:
                case MazeTypeEnum.Medium:
                case MazeTypeEnum.Large:
                    StartCoroutine(SpawnCoRoutine());
                    break;
            }
        }
        
        /// <summary>
        /// Spawn enemies within the maze until the end of time
        /// </summary>
        /// <returns></returns>
        public IEnumerator SpawnCoRoutine()
        {
            while (true)
            {
                GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
                int numEnemies = allEnemies.Length;
                int enemiesToSpawn = desiredEnemies - numEnemies;

                switch (MazeType)
                {
                    case MazeTypeEnum.Tutorial:
                        TutorialSpawnEnemy();
                        StopCoroutine(SpawnCoRoutine());
                        break;
                    
                    default:
                        SpawnEnemy(enemiesToSpawn + 2);
                        break;
                }

                if (spawnDistance != 0)
                {
                    spawnDistance -= 2;
                }
                yield return new WaitForSeconds(30);
            }
        }

        void TutorialSpawnEnemy()
        {
            PlaceEnemy(col[63], row[63]);
            PlaceEnemy(col[60], row[60]);
            PlaceEnemy(col[22], row[22]);
            PlaceEnemy(col[56], row[56]);
            PlaceEnemy(col[53], row[53]);
            PlaceEnemy(col[16], row[16]);
            PlaceEnemy(col[50], row[50]);
        }
        
        void SpawnEnemy(int numEnemies)
        {
            /*
         * Simple method that spawns enemies in the maze
         */
        
            enemies = new int[length];
            for (int i = 0; i < numEnemies; i++)
            {
                System.Random random = new System.Random();
                int temp = random.Next(0, length - 1);
                while(enemies.Contains(temp))
                    temp = random.Next(0, length - 1);
                // if(!(enemies.Contains(temp)))
                PlaceEnemy(col[temp], row[temp]);
                enemies[i] = temp;
            }
        }

        private void TutorialPowerUpSpawn()
        {
            PlaceItem(1, 3, healthPotion, "health potion");
            PlaceItem(1, 3, shieldPotion, "shield potion");
        }

        void SpawnPowerUp()
        {
            /*
         * Spawn power ups within the maze
         */
        
            int l = deadEndCol.Length;
            System.Random random = new System.Random();
        
            // guarantee spawning 2 piles of arrows
            for (int i = 0; i < 3; i++)
            {
                if (i > l)
                {
                    break;
                }
                PlaceItem(deadEndCol[i], deadEndRow[i], arrow, "arrow pile");
            }
        
            for (int i = 3; i < l; i++)
            {
                int droppableType = random.Next((int)DroppableTypeEnum.Health, (int)DroppableTypeEnum.Arrow);

                switch ((DroppableTypeEnum)droppableType)
                {
                    case DroppableTypeEnum.Health:
                        PlaceItem(deadEndCol[i], deadEndRow[i], healthPotion, "health potion");
                        break;
                    
                    case DroppableTypeEnum.Arrow:
                        PlaceItem(deadEndCol[i], deadEndRow[i], arrow, "arrow pile");
                        break;
                    
                    case DroppableTypeEnum.Overshield:
                        PlaceItem(deadEndCol[i], deadEndRow[i], shieldPotion, "shield potion");
                        break;
                }
            }
        }

        private void DisplayMaze()
        {
            GameObject go = new GameObject();
            go.transform.position = Vector3.zero;
            go.name = "Procedural Maze";
            go.tag = "Generated";
        
            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.mesh = meshGenerator.FromData(Data);

            MeshCollider mc = go.AddComponent<MeshCollider>();
            mc.sharedMesh = mf.mesh;

            mr = go.AddComponent<MeshRenderer>();
            mr.materials = new[] {mazeMat1, mazeMat2};
            // go.AddComponent<NavMeshMo>()
        }

        private void DisposeOldMaze()
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated");
            foreach (GameObject go in objects) {
                Destroy(go);
            }
        }

        private void FindStartPosition()
        {
            int[,] maze = Data;
            int rMax = maze.GetUpperBound(0);
            int cMax = maze.GetUpperBound(1);

            for (int i = 0; i <= rMax; i++)
            {
                for (int j = 0; j <= cMax; j++)
                {
                    if (maze[i, j] == 0)
                    {
                        StartRow = i;
                        StartCol = j;
                        return;
                    }
                }
            }
        }
    
        private void FindDeadEnd()
        {
            int[,] maze = Data;
            int rMax = maze.GetUpperBound(0);
            int cMax = maze.GetUpperBound(1);
            int tempLength = 0;

            for (int currentRow = 1; currentRow < rMax; currentRow++)
            {
                for (int currentCol = 1; currentCol < cMax; currentCol++)
                {
                    if (maze[currentRow, currentCol] == 0)
                    {
                        int left = maze[currentRow, currentCol - 1];
                        int right = maze[currentRow, currentCol + 1];
                        int front = maze[currentRow + 1, currentCol];
                        int back = maze[currentRow - 1, currentCol];
                    
                        if((left == 1 && right == 1) && (front == 1 && back == 0))
                        {
                            // Debug.Log("Check case 1");
                            tempLength++;
                            // break;
                        }

                        if ((left == 0 && right == 1) && (front == 1 && back == 1))
                        {
                            // Debug.Log("Check case 2");
                            tempLength++;
                            // break;
                        }

                        if ((left == 1 && right == 0) && (front == 1 && back == 1))
                        {
                            // Debug.Log("Check case 3");
                            tempLength++;
                            // break;
                        }

                        if ((left == 1 && right == 1) && (front == 0 && back == 1))
                        {
                            // Debug.Log("Check case 4");
                            tempLength++;
                            // break;
                        }
                    }
                }
            }

            deadEndCol = new int[tempLength];
            deadEndRow = new int[tempLength];
            int itter = 0;
        
        
            for (int i = 1; i < rMax; i++)
            {
                for (int j = 1; j < cMax; j++)
                {
                    if (maze[i, j] == 0)
                    {
                        int left = maze[i, j - 1];
                        int right = maze[i, j + 1];
                        int front = maze[i + 1, j];
                        int back = maze[i - 1, j];

                        if ((left == 1 && right == 1) && (front == 1 && back == 0))
                        {
                            deadEndCol[itter] = j;
                            deadEndRow[itter] = i;
                            // Debug.Log("Case 1");
                            itter++;
                            // break;
                        }

                        if ((left == 0 && right == 1) && (front == 1 && back == 1))
                        {
                            deadEndCol[itter] = j;
                            deadEndRow[itter] = i;
                            // Debug.Log("Case 2");
                            itter++;
                            // break;
                        }

                        if ((left == 1 && right == 0) && (front == 1 && back == 1))
                        {
                            deadEndCol[itter] = j;
                            deadEndRow[itter] = i;
                            // Debug.Log("Case 3");
                            itter++;
                            // break;
                        }
                    
                        if ((left == 1 && right == 1) && (front == 0 && back == 1))
                        {
                            deadEndCol[itter] = j;
                            deadEndRow[itter] = i;
                            // Debug.Log("Case 4");
                            itter++;
                            // break;
                        }
                    }
                }
            }
        }

        private void FindGoalPosition()
        {
            int[,] maze = Data;
            int rMax = maze.GetUpperBound(0);
            int cMax = maze.GetUpperBound(1);
        

            // loop top to bottom, right to left
            for (int i = rMax; i >= 0; i--)
            {
                for (int j = cMax; j >= 0; j--)
                {
                    if (maze[i, j] == 0)
                    {
                        length++;
                    }
                }
            }

            col = new int[length];
            row = new int [length];
            int itter = 0;
        
            for (int i = rMax; i >= 0; i--)
            {
                for (int j = cMax; j >= 0; j--)
                {
                    if (maze[i, j] == 0)
                    {
                        row[itter] = i;
                        col[itter] = j;
                        itter++;
                    
                        // return;
                    }
                }
            }
        }

        /// <summary>
        /// Spawn a pickup given the location
        /// </summary>
        /// <param name="columnIndex">the int value of the column index</param>
        /// <param name="rowIndex">the int value of the row index</param>
        /// <param name="pickupName">the name to give the gameobject</param>
        /// <param name="pickup">the tag to give the gameobject</param>
        private void PlaceItem(int columnIndex, int rowIndex, GameObject pickup, string pickupName)
        {
            // Where to spawn the pickup
            Vector3 spawnPosition = new Vector3(columnIndex * HallWidth, -.5f, rowIndex * HallWidth);
        
            // get the distance between the pickup and portal
            float playerDistance = Vector3.Distance(player.transform.position, spawnPosition);
            float portalDistance = Vector3.Distance(Portal.transform.position, spawnPosition);
        
            // Don't want a pickup to spawn on the player or portal
            if (playerDistance < 3  || portalDistance < 3)
            {
                return;
            }

            // Instantiate then set the meta data
            GameObject go = Instantiate(pickup);
            go.transform.position = new Vector3(columnIndex * HallWidth, -0.0f, rowIndex * HallWidth);
            go.SetActive(true);
            go.name = pickupName;
        }
        public IEnumerator SpawnEnemyAutzen()
        {

            while (true)
            {
                for (int i = 0; i < 6; i++)
                {
                    GameObject sk;
                    System.Random random = new System.Random();
                    int temp = random.Next(1, 3);
                    int x = random.Next(-370, -340);
                    int y = random.Next(30, 40);
                    if (temp == 1)
                    {
                        sk = Instantiate(mage);
                        sk.name = "Mage";
                        sk.tag = "Mage";
                    }
                    else
                    {
                        sk = Instantiate(skeleton);
                        sk.name = "Skeleton";
                        sk.tag = "Enemy";
                    }
        
                    // Debug.Log("X: " + x + " y: " + y);
                    sk.transform.position = new Vector3(x, .1f, y);
                    // Debug.Log("autzen distance " + distance);
            
                    sk.SetActive(false);
                }
            
                yield return new WaitForSeconds(20);
            }
        }
        private void PlaceEnemy(int column, int newRow)
        {
            // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);

            System.Random random = new System.Random();
            int temp = random.Next(1, 8);
            GameObject sk;
            Debug.Log("temp " + temp);
            if (temp >= 5f && MazeType != MazeTypeEnum.Tutorial)
            {
                sk = Instantiate(mage);
                sk.name = "Mage";
                sk.tag = "Mage";
            }
            else
            {
                sk = Instantiate(skeleton);
                sk.name = "Skeleton";
                sk.tag = "Enemy";
            }
            Debug.Log("Random spawn is " + temp);
            // sk.AddComponent<NavMeshAgent>();
            sk.transform.position = new Vector3(column * HallWidth, .1f, newRow * HallWidth);
            float distance = Math.Abs(sk.transform.position.x - player.transform.position.x);
            Debug.Log("distance: " + distance);
            if (distance < spawnDistance)
            {
                Debug.Log("Enemy too close, not spawning at location " + sk.transform.position);
                Destroy(sk);
                return;
            }
        
            sk.AddComponent<MeshCollider>();
            // sk.enabled = true;
            sk.SetActive(true);

        }
    
        /// <summary>
        /// Spawn the portal at the end of the maze
        /// </summary>
        /// <param name="column">column int index</param>
        /// <param name="newRow">row int index</param>
        private void PlaceEndPortal(int column, int newRow)
        {
            Vector3 gatePos = new Vector3(column * HallWidth + HallWidth / 2, .5f, newRow * HallWidth);

            Instantiate(gate, gatePos, Quaternion.Euler(-90, 135, 0));
            gate.tag = "Portal";
            gate.name = "MazePortal";
        }

        // top-down debug display
        void OnGUI()
        {
            if (!showDebug)
            {
                return;
            }

            int[,] maze = Data;
            int rMax = maze.GetUpperBound(0);
            int cMax = maze.GetUpperBound(1);

            string msg = "";

            // loop top to bottom, left to right
            for (int i = rMax; i >= 0; i--)
            {
                for (int j = 0; j <= cMax; j++)
                {
                    if (maze[i, j] == 0)
                    {
                        msg += "0";
                    }
                    else
                    {
                        msg += "+";
                    }

                    msg += " ";
                }
                msg += "\n";
            }

            GUI.Label(new Rect(20, 20, 500, 500), msg);
        }
    }
}
