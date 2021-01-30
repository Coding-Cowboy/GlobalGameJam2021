using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    public float gridSize = 20.5f / 7.0f; //2.92857142f
    public int rows = 7, cols = 7;
    public GameObject playerPrefab;
    public GameObject goalPrefab;
    public GameObject deadEnd;
    public GameObject straight;
    public GameObject turn;
    public GameObject tee;
    public GameObject cross;

    public bool debugMode = false;
    public GameObject debugDeadEnd;
    public GameObject debugStraight;
    public GameObject debugTurn;
    public GameObject debugTee;
    public GameObject debugCross;

    private int minCompletionDistance;
    private int startRow;
    private int startCol;
    private int endRow;
    private int endCol;

    private class MazeTile
    {
        public MazeTile()
        {
            left = false;
            up = false;
            right = false;
            down = false;
        }

        public GameObject tileGameObject;
        public bool left, up, right, down;
    }

    private bool[][][] wallData;
    private MazeTile[][] tiles;

    private bool[][][] GetWallData()
    {
        bool[][][] walls = new bool[2][][];
        walls[0] = new bool[rows - 1][];
        walls[1] = new bool[rows][];
        for (int i = 0; i < rows; i++)
        {
            if (i != rows - 1)
            {
                walls[0][i] = new bool[cols];
                for (int j = 0; j < walls[0][i].Length; j++)
                    walls[0][i][j] = true;
            }
            walls[1][i] = new bool[cols - 1];
            for (int j = 0; j < walls[1][i].Length; j++)
                walls[1][i][j] = true;
        }

        ISet<System.Tuple<int, int>> squares = new HashSet<System.Tuple<int, int>>();
        ISet<System.Tuple<int, int, int>> candidateWalls = new HashSet<System.Tuple<int, int, int>>();
        System.Tuple<int, int> startSquare =
            new System.Tuple<int, int>((int)Random.Range(0.0f, rows - 0.001f), (int)Random.Range(0.0f, cols - 0.001f));
        if (startSquare.Item1 > 0)
            candidateWalls.Add(new System.Tuple<int, int, int>(0, startSquare.Item1 - 1, startSquare.Item2));
        if (startSquare.Item2 > 0)
            candidateWalls.Add(new System.Tuple<int, int, int>(1, startSquare.Item1, startSquare.Item2 - 1));
        if (startSquare.Item1 < rows - 1)
            candidateWalls.Add(new System.Tuple<int, int, int>(0, startSquare.Item1, startSquare.Item2));
        if (startSquare.Item2 < cols - 1)
            candidateWalls.Add(new System.Tuple<int, int, int>(1, startSquare.Item1, startSquare.Item2));
        while (squares.Count < rows * cols)
        {
            int k = (int)Random.Range(0.0f, candidateWalls.Count - 0.001f);
            IEnumerator<System.Tuple<int, int, int>> e = candidateWalls.GetEnumerator();
            for (int i = 0; i <= k; i++) e.MoveNext();
            System.Tuple<int, int, int> currentWall = e.Current;
            System.Tuple<int, int> square1, square2, newsquare = null;
            square1 = new System.Tuple<int, int>(currentWall.Item2, currentWall.Item3);
            square2 = new System.Tuple<int, int>(currentWall.Item2 + 1 - currentWall.Item1, currentWall.Item3 + currentWall.Item1);
            if (!squares.Contains(square1))
                newsquare = square1;
            if (!squares.Contains(square2))
                newsquare = square2;
            if (newsquare != null)
            {
                squares.Add(newsquare);
                walls[currentWall.Item1][currentWall.Item2][currentWall.Item3] = false;
                if (newsquare.Item1 > 0 && !squares.Contains(new System.Tuple<int, int>(newsquare.Item1 - 1, newsquare.Item2)))
                    candidateWalls.Add(new System.Tuple<int, int, int>(0, newsquare.Item1 - 1, newsquare.Item2));
                if (newsquare.Item2 > 0 && !squares.Contains(new System.Tuple<int, int>(newsquare.Item1, newsquare.Item2 - 1)))
                    candidateWalls.Add(new System.Tuple<int, int, int>(1, newsquare.Item1, newsquare.Item2 - 1));
                if (newsquare.Item1 < rows - 1 && !squares.Contains(new System.Tuple<int, int>(newsquare.Item1 + 1, newsquare.Item2)))
                    candidateWalls.Add(new System.Tuple<int, int, int>(0, newsquare.Item1, newsquare.Item2));
                if (newsquare.Item2 < cols - 1 && !squares.Contains(new System.Tuple<int, int>(newsquare.Item1, newsquare.Item2 + 1)))
                    candidateWalls.Add(new System.Tuple<int, int, int>(1, newsquare.Item1, newsquare.Item2));
            }
            candidateWalls.Remove(currentWall);
        }
        return walls;
    }

    public GameObject CreateHorizontalWall(int row, int startcol, int endcol)
    {
        float squareSizeX = 20.5f / cols;
        float squareSizeZ = 20.5f / rows;
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.localScale = new Vector3((endcol - startcol) * squareSizeX + 0.5f, 2.0f, 0.5f);
        wall.transform.position = new Vector3((endcol + startcol) / 2.0f * squareSizeX - 1.5f, 1.0f, (row + 1) * squareSizeZ - 1.5f);
        return wall;
    }

    public GameObject CreateVerticalWall(int col, int startrow, int endrow)
    {
        float squareSizeX = 20.5f / cols;
        float squareSizeZ = 20.5f / rows;
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.localScale = new Vector3(0.5f, 2.0f, (endrow - startrow) * squareSizeZ + 0.5f);
        wall.transform.position = new Vector3((col + 1) * squareSizeX - 1.5f, 1.0f, (endrow + startrow) / 2.0f * squareSizeZ - 1.5f);
        return wall;
    }

    // Start is called before the first frame update
    void Awake()
    {
        minCompletionDistance = (int)(Mathf.Min(rows, cols) / 2 + 0.5f);
        wallData = GetWallData();
        tiles = new MazeTile[rows][];
        for (int i = 0; i < rows; i++)
        {
            tiles[i] = new MazeTile[cols];
            for (int j = 0; j < cols; j++)
            {
                tiles[i][j] = new MazeTile();
            }
        }

        //traverse horizontal walls
        for (int i = 0; i < wallData[0].Length; i++)
        {
            for (int j = 0; j < wallData[0][i].Length; j++)
            {
                tiles[i][j].up = !wallData[0][i][j];
                tiles[i + 1][j].down = !wallData[0][i][j];
            }
        }

        //traverse vertical walls
        for (int i = 0; i < wallData[1].Length; i++)
        {
            for (int j = 0; j < wallData[1][i].Length; j++)
            {
                tiles[i][j].right = !wallData[1][i][j];
                tiles[i][j + 1].left = !wallData[1][i][j];
            }
        }

        //place tiles
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                bool left = tiles[i][j].left;
                bool up = tiles[i][j].up;
                bool right = tiles[i][j].right;
                bool down = tiles[i][j].down;
                int numOutlets =
                    (left == true ? 1 : 0) +
                    (up == true ? 1 : 0) +
                    (right == true ? 1 : 0) +
                    (down == true ? 1 : 0);

                GameObject spawnedPiece;

                switch (numOutlets)
                {
                    case 1:
                        //dead end
                        spawnedPiece = Object.Instantiate(deadEnd, new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                        if (left)
                        {
                            spawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                        }
                        else if (up)
                        {
                            spawnedPiece.transform.Rotate(new Vector3(0, 180.0f, 0));
                        }
                        else if (right)
                        {
                            spawnedPiece.transform.Rotate(new Vector3(0, 270.0f, 0));
                        }
                        else if (down)
                        {
                            //do nothing
                        }
                        tiles[i][j].tileGameObject = spawnedPiece;
                        break;
                    case 2:
                        //straight
                        spawnedPiece = null;
                        if (left && right)
                        {
                            spawnedPiece = Object.Instantiate(straight, new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                            spawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                        }
                        else if (up && down)
                        {
                            spawnedPiece = Object.Instantiate(straight, new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                        }
                        //turn
                        else if (left && up)
                        {
                            spawnedPiece = Object.Instantiate(turn, new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                            spawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                        }
                        else if (up && right)
                        {
                            spawnedPiece = Object.Instantiate(turn, new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                            spawnedPiece.transform.Rotate(new Vector3(0, 180.0f, 0));
                        }
                        else if (right && down)
                        {
                            spawnedPiece = Object.Instantiate(turn, new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                            spawnedPiece.transform.Rotate(new Vector3(0, 270.0f, 0));
                        }
                        else if (down && left)
                        {
                            spawnedPiece = Object.Instantiate(turn, new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                        }
                        tiles[i][j].tileGameObject = spawnedPiece;
                        break;
                    case 3:
                        spawnedPiece = Object.Instantiate(tee, new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                        //tee
                        if (down && left && up)
                        {
                            spawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                        }
                        else if (left && up && right)
                        {
                            spawnedPiece.transform.Rotate(new Vector3(0, 180.0f, 0));
                        }
                        else if (up && right && down)
                        {
                            spawnedPiece.transform.Rotate(new Vector3(0, 270.0f, 0));
                        }
                        else if (right && down && left)
                        {
                            //do nothing
                        }
                        tiles[i][j].tileGameObject = spawnedPiece;
                        break;
                    case 4:
                        //cross
                        spawnedPiece = Object.Instantiate(cross, new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                        tiles[i][j].tileGameObject = spawnedPiece;
                        break;
                    default:
                        Debug.Log(numOutlets);
                        break;
                }

                if (debugMode == true)
                {
                    float debugGridSize = 2.5f;
                    switch (numOutlets)
                    {
                        case 1:
                            //dead end
                            spawnedPiece = Object.Instantiate(debugDeadEnd, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                            if (left)
                            {
                                spawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                            }
                            else if (up)
                            {
                                spawnedPiece.transform.Rotate(new Vector3(0, 180.0f, 0));
                            }
                            else if (right)
                            {
                                spawnedPiece.transform.Rotate(new Vector3(0, 270.0f, 0));
                            }
                            else if (down)
                            {
                                //do nothing
                            }
                            break;
                        case 2:
                            //straight
                            if (left && right)
                            {
                                spawnedPiece = Object.Instantiate(debugStraight, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                                spawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                            }
                            else if (up && down)
                            {
                                spawnedPiece = Object.Instantiate(debugStraight, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                            }
                            //turn
                            else if (left && up)
                            {
                                spawnedPiece = Object.Instantiate(debugTurn, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                                spawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                            }
                            else if (up && right)
                            {
                                spawnedPiece = Object.Instantiate(debugTurn, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                                spawnedPiece.transform.Rotate(new Vector3(0, 180.0f, 0));
                            }
                            else if (right && down)
                            {
                                spawnedPiece = Object.Instantiate(debugTurn, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                                spawnedPiece.transform.Rotate(new Vector3(0, 270.0f, 0));
                            }
                            else if (down && left)
                            {
                                spawnedPiece = Object.Instantiate(debugTurn, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                            }
                            break;
                        case 3:
                            //tee
                            if (down && left && up)
                            {
                                spawnedPiece = Object.Instantiate(debugTee, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                                spawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                            }
                            else if (left && up && right)
                            {
                                spawnedPiece = Object.Instantiate(debugTee, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                                spawnedPiece.transform.Rotate(new Vector3(0, 180.0f, 0));
                            }
                            else if (up && right && down)
                            {
                                spawnedPiece = Object.Instantiate(debugTee, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                                spawnedPiece.transform.Rotate(new Vector3(0, 270.0f, 0));
                            }
                            else if (right && down && left)
                            {
                                spawnedPiece = Object.Instantiate(debugTee, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                            }
                            break;
                        case 4:
                            //cross
                            spawnedPiece = Object.Instantiate(debugCross, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                            break;
                        default:
                            Debug.Log(numOutlets);
                            break;
                    }
                }
            }
        }

        //generate the start and end
        //make sure that they are a certain manhattan distance apart
        endRow = Random.Range(0, rows - 1);
        endCol = Random.Range(0, cols - 1);
        while (
                (tiles[endRow][endCol].left == true ? 1 : 0) +
                (tiles[endRow][endCol].up == true ? 1 : 0) +
                (tiles[endRow][endCol].right == true ? 1 : 0) +
                (tiles[endRow][endCol].down == true ? 1 : 0)
                != 1
            )
        {
            endRow = Random.Range(0, rows - 1);
            endCol = Random.Range(0, cols - 1);
        }
        Debug.Log("End Row: " + endRow + "End Col" + endCol);

        startRow = Random.Range(0, rows - 1);
        startCol = Random.Range(0, cols - 1);
        while (calcManhattanDistance(startRow, startCol, endRow, endCol) < minCompletionDistance)
        {
            startRow = Random.Range(0, rows - 1);
            startCol = Random.Range(0, cols - 1);
        }
        Debug.Log("Start Row: " + startRow + "Start Col" + startCol);

        Vector3 goalPosition = tiles[endRow][endCol].tileGameObject.transform.position;
        Quaternion goalRotation = tiles[endRow][endCol].tileGameObject.transform.rotation;
        Destroy(tiles[endRow][endCol].tileGameObject);

        tiles[endRow][endCol].tileGameObject = Object.Instantiate(goalPrefab, goalPosition, goalRotation);

        Vector3 playerPosition = new Vector3(startCol * gridSize, 0, startRow * gridSize);
        Vector3 spawnOffset = new Vector3(0.0f, 1.03f, 0.0f);
        Object.Instantiate(playerPrefab, playerPosition + spawnOffset, new Quaternion());
    }

    private int calcManhattanDistance(int x1, int y1, int x2, int y2)
    {
        return Mathf.Abs(x2 - x1) + Mathf.Abs(y2 - y1);
    }
}
