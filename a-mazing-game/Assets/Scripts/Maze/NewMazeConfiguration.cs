using Maze.Enums;

namespace Maze
{
    /// <summary>
    /// The configuration of how to generate a new game / maze
    /// </summary>
    public static class NewMazeConfiguration
    {
        #region Maze Type
        
        /// <summary>
        /// The type of maze the <see cref="MazeConstructor"/> will generate
        /// </summary>
        public static MazeTypeEnum MazeType { get; private set; }

        /// <summary>
        /// And a boring getter for <see cref="MazeType"/>
        /// </summary>
        /// <param name="type"></param>
        public static void SetMazeType(MazeTypeEnum type) => MazeType = type;
        
        #endregion Maze Type
        
        #region Maze Size
        
        /// <summary>
        /// The number of rows in the maze
        /// </summary>
        public static int Rows { get; private set; }
        
        /// <summary>
        /// The number of columns in the maze
        /// </summary>
        public static int Columns { get; private set; }

        /// <summary>
        /// Setter for <see cref="Rows"/>
        /// </summary>
        /// <param name="numRows"></param>
        public static void SetRows(int numRows) => Rows = numRows;

        /// <summary>
        /// Setter for <see cref="Columns"/>
        /// </summary>
        /// <param name="numColumns"></param>
        public static void SetColumns(int numColumns) => Columns = numColumns;

        #endregion Maze Size
    }
}