using UnityEngine;

public class MazeConstructor : MonoBehaviour
{
    //1
    public bool showDebug;
    private MazeDataGenerator dataGenerator;

    
    [SerializeField] private Material mazeMat1;
    [SerializeField] private Material mazeMat2;
    [SerializeField] private Material startMat;
    [SerializeField] private Material treasureMat;

    //2
    public int[,] data
    {
        get; private set;
    }

    //3
    void Awake()
    {
        // default to walls surrounding a single empty cell
        data = new int[,]
        {
            {1, 1, 1},  // 1 means wall, 0 means empty
            {1, 0, 1},
            {1, 1, 1}
        };
        dataGenerator = new MazeDataGenerator();

    }
    
    public void GenerateNewMaze(int sizeRows, int sizeCols)
    {
        if (sizeRows % 2 == 0 && sizeCols % 2 == 0)
        {
            Debug.LogError("Odd numbers work better for dungeon size.");
        }

        data = dataGenerator.FromDimensions(sizeRows, sizeCols);

    }
    
    void OnGUI()
    {
        //1
        if (!showDebug)
        {
            return;
        }

        //2
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        string msg = "";

        //3
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

        //4
        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }

}