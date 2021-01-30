using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    public float gridSize = 20.5f / 7.0f; //2.92857142f
    public int rows = 7, cols = 7;
    public GameObject deadEnd;
    public GameObject straight;
    public GameObject turn;
    public GameObject tee;
    public GameObject cross;

    private class MazeTile
    {
        public MazeTile()
        {
            left = false;
            up = false;
            right = false;
            down = false;
        }
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
        //walls[0][0][6] = false;

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
    void Start()
    {
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

        //traverse vertically
        for (int i = 0; i < wallData[0].Length; i++)
        {
            int j = 0;
            while (j < wallData[0][i].Length)
            {
                while (j < wallData[0][i].Length && !wallData[0][i][j]) j++;
                int wallStart = j;
                while (j < wallData[0][i].Length && wallData[0][i][j]) j++;
                int wallEnd = j;
                if (wallStart < wallData[0][i].Length)
                {
                    CreateHorizontalWall(i, wallStart, wallEnd);
                }
            }
        }

        //traverse horizontally
        for (int i = 0; i < wallData[1][0].Length; i++)
        {
            int j = 0;
            while (j < wallData[1].Length)
            {
                while (j < wallData[1].Length && !wallData[1][j][i]) j++;
                int wallStart = j;
                while (j < wallData[1].Length && wallData[1][j][i]) j++;
                int wallEnd = j;
                if (wallStart < wallData[1].Length)
                {
                    CreateVerticalWall(i, wallStart, wallEnd);
                }
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
                        break;
                    case 2:
                        //straight
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
                        break;
                    case 3:
                        //tee
                        if (down && left && up)
                        {
                            spawnedPiece = Object.Instantiate(tee, new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                            spawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                        }
                        else if (left && up && right)
                        {
                            spawnedPiece = Object.Instantiate(tee, new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                            spawnedPiece.transform.Rotate(new Vector3(0, 180.0f, 0));
                        }
                        else if (up && right && down)
                        {
                            spawnedPiece = Object.Instantiate(tee, new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                            spawnedPiece.transform.Rotate(new Vector3(0, 270.0f, 0));
                        }
                        else if (right && down && left)
                        {
                            spawnedPiece = Object.Instantiate(tee, new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                        }
                        break;
                    case 4:
                        //cross
                        spawnedPiece = Object.Instantiate(cross, new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                        break;
                    default:
                        Debug.Log(numOutlets);
                        break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
