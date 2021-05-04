/*
 * written by Joseph Hocking 2017
 * released under MIT license
 * text of license https://opensource.org/licenses/MIT
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class MazeConstructor : MonoBehaviour
{
    public bool showDebug;
    private int[] col;
    public int[] row;
    private int[] enemies; // the spawn location of enemies
    public int length;

    

    [SerializeField] private Material mazeMat1;
    [SerializeField] private Material mazeMat2;
    [SerializeField] private Material startMat;
    [SerializeField] private Material treasureMat;
    [SerializeField] private Material endGoal;
    [SerializeField] private Material testSpawn;
    private GameController player;
    public GameObject start;
    public GameObject enemy;
    public GameObject skeleton;
    public AIMovement ai;
    public NavMeshAgent agent;
    private MeshRenderer mr;
    private MeshFilter tt;
    private MeshRenderer ttt;
    private MeshCollider tttt;
    private int testRow;
    private int testCol;
    private int[] deadEndCol;
    private int[] deadEndRow;

    public int[,] data
    {
        get; private set;
    }

    public float hallWidth
    {
        get; private set;
    }
    public float hallHeight
    {
        get; private set;
    }

    public int startRow
    {
        get; private set;
    }
    public int startCol
    {
        get; private set;
    }

    public int goalRow
    {
        get; private set;
    }
    public int goalCol
    {
        get; private set;
    }

    private MazeDataGenerator dataGenerator;
    private MazeMeshGenerator meshGenerator;

    void Awake()
    {
        dataGenerator = new MazeDataGenerator();
        meshGenerator = new MazeMeshGenerator();
        player = GetComponent<GameController>();
        length = 0;
        agent = GetComponent<NavMeshAgent>();

        // default to walls surrounding a single empty cell
        data = new int[,]
        {
            {1, 1, 1},
            {1, 0, 1},
            {1, 1, 1}
        };
        
    }

    public void GenerateNewMaze(int sizeRows, int sizeCols,
        TriggerEventHandler startCallback=null, TriggerEventHandler goalCallback=null, TriggerEventHandler endGame=null)
    {
        if (sizeRows % 2 == 0 && sizeCols % 2 == 0)
        {
            Debug.LogError("Odd numbers work better for dungeon size.");
        }

        DisposeOldMaze();

        data = dataGenerator.FromDimensions(sizeRows, sizeCols);

        FindStartPosition();
        FindGoalPosition();
        // FindDeadEnd();
        printMaze();

        // store values used to generate this mesh
        hallWidth = meshGenerator.width;
        hallHeight = meshGenerator.height;

        DisplayMaze();
        
        // PlaceStartTrigger(startCallback);
        enemies = new int[length];
        for (int i = 0; i < 6; i++)
        {
            System.Random random = new System.Random();
            int temp = random.Next(0, length - 1);
            while(enemies.Contains(temp))
                temp = random.Next(0, length - 1);
            // if(!(enemies.Contains(temp)))
            // PlaceGoalTrigger(col[temp], row[temp], goalCallback);
            enemies[i] = temp;
        }
        
        int l = deadEndCol.Length;
        Debug.Log("L " + l);
        for(int i = 0; i < l; i++)
            PlaceTestTrigger(deadEndCol[i], deadEndRow[i], null);
        PlaceEndTrigger(col[0], row[0], endGame);
    }

    private void DisplayMaze()
    {
        GameObject go = new GameObject();
        go.transform.position = Vector3.zero;
        go.name = "Procedural Maze";
        go.tag = "Generated";
        
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = meshGenerator.FromData(data);

        MeshCollider mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;

        mr = go.AddComponent<MeshRenderer>();
        mr.materials = new Material[2] {mazeMat1, mazeMat2};
        // go.AddComponent<NavMeshMo>()
    }
    
    void printMaze()
    {
        int rMax = data.GetUpperBound(0);  // 1
        int cMax = data.GetUpperBound(1);  // 2

        for (int i = 0; i <= rMax; i++)
        {
            string str = "";
            for (int j = 0; j <= cMax; j++)
            {
                str += data[i, j];
                str += " ";
            }   
            Debug.Log(str);
        }
    }

    public void DisposeOldMaze()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated");
        foreach (GameObject go in objects) {
            Destroy(go);
        }
    }

    private void FindStartPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    startRow = i;
                    startCol = j;
                    return;
                }
            }
        }
    }
    
    private void FindDeadEnd()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);
        int length = 0;

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
                    if(left == 1 && right == 1 && (front == 1 || back == 1))
                    {
                        length++;
                        // testRow = i;
                        // testCol = j;
                        // return;
                    }
                    
                }
            }
        }

        deadEndCol = new int[length];
        deadEndRow = new int[length];
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
                    if(left == 1 && right == 1 && (front == 1 || back == 1))
                    {
                        deadEndCol[itter] = j;
                        deadEndRow[itter] = i;
                        // testRow = i;
                        // testCol = j;
                        // return;
                    }
                    
                }
            }
        }

        for (int i = 0; i < length; i++)
        {
            Debug.Log(deadEndCol[i] + " " + deadEndRow[i]);
        }
    }

    private void FindGoalPosition()
    {
        int[,] maze = data;
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
                    // goalRow = i;
                    // goalCol = j;
                    // return;
                }
            }
        }
        Debug.Log("rmax " + rMax + " cmax " + cMax + " length " + length);

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
    
    private void PlaceTestTrigger(int column, int newRow, TriggerEventHandler callback)
    {
        Debug.Log("Test spawn " + column + " " + newRow);
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(column * hallWidth, .5f, newRow * hallWidth);
        go.name = "Test";
        go.tag = "Generated";
        
        // go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().sharedMaterial = testSpawn;
        
        // TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        // tc.callback = callback;
        
    }

    private void PlaceStartTrigger(TriggerEventHandler callback)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(startCol * hallWidth, .5f, startRow * hallWidth);
        go.name = "Start Trigger";
        go.tag = "Generated";

        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().sharedMaterial = startMat;

        TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }

    private void PlaceGoalTrigger(int column, int newRow, TriggerEventHandler callback)
    {
        // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject sk = Instantiate(skeleton) as GameObject;
        // sk.AddComponent<NavMeshAgent>();
        sk.transform.position = new Vector3(column * hallWidth, .1f, newRow * hallWidth);
        sk.AddComponent<MeshCollider>();
        sk.SetActive(true);
        MeshCollider t = sk.GetComponent<MeshCollider>();
        // t.material = mr.materials[0];
        
        // Instantiate(skeleton);
        sk.name = "Skeleton";
        sk.tag = "Generated";
        
        // sk.GetComponent<BoxCollider>().isTrigger = true;
        // skeleton.GetComponent<MeshRenderer>().sharedMaterial = treasureMat;
        
        // TriggerEventRouter tc = skeleton.AddComponent<TriggerEventRouter>();
        // tc.callback = callback;
        
    }
    
    private void PlaceEndTrigger(int column, int newRow, TriggerEventHandler callback)
    {
        Debug.Log("End trigger: " + column + " " + newRow);
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(column * hallWidth, .5f, newRow * hallWidth);
        go.name = "End";
        go.tag = "Generated";
        
        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().sharedMaterial = endGoal;
        
        TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
        
    }

    // top-down debug display
    void OnGUI()
    {
        if (!showDebug)
        {
            return;
        }

        int[,] maze = data;
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
                    msg += "....";
                }
                else
                {
                    msg += "==";
                }
            }
            msg += "\n";
        }

        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }
}
