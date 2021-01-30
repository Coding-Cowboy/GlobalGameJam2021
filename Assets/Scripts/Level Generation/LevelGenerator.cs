using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    //public float maxSpawnTimer = 10;
    //public float minSpawnTimer = 5;
    //public GameObject[] enemyPrefabs;
    public float gridSize = 23.5f;
    public int rows = 7, cols = 7;
    public float campFireSpawnChance = 0.15f;
    public float landmarkSpawnChance = 0.15f;
    public float pickupSpawnChance = 0.15f;
    public GameObject playerPrefab;
    public GameObject goalPrefab;
    public GameObject[] deadEnd;
    public GameObject[] straight;
    public GameObject[] turn;
    public GameObject[] tee;
    public GameObject[] cross;

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
    private GameObject player;
    private float spawnTimer = 0;

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
            new System.Tuple<int, int>((int)UnityEngine.Random.Range(0.0f, rows - 0.001f), (int)UnityEngine.Random.Range(0.0f, cols - 0.001f));
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
            int k = (int)UnityEngine.Random.Range(0.0f, candidateWalls.Count - 0.001f);
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
                GameObject spawnedPiece = spawnTile(i, j);
                if (UnityEngine.Random.Range(0.0f, 1.0f) <= campFireSpawnChance)
                {
                    CampfireSpawner spawner = spawnedPiece.GetComponent<CampfireSpawner>();
                    if (spawner != null)
                        spawner.SpawnCampfire();
                }
                if (UnityEngine.Random.Range(0.0f, 1.0f) <= landmarkSpawnChance)
                {
                    LandmarkSpawner spawner = spawnedPiece.GetComponent<LandmarkSpawner>();
                    if (spawner != null)
                        spawner.SpawnLandmark();
                }
                if (UnityEngine.Random.Range(0.0f, 1.0f) <= pickupSpawnChance)
                {
                    PickupSpawner spawner = spawnedPiece.GetComponent<PickupSpawner>();
                    if (spawner != null)
                        spawner.SpawnPickup();
                }

                if (debugMode == true)
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
                    GameObject debugSpawnedPiece = null;

                    float debugGridSize = 2.5f;
                    switch (numOutlets)
                    {
                        case 1:
                            //dead end
                            debugSpawnedPiece = UnityEngine.Object.Instantiate(debugDeadEnd, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                            if (left)
                            {
                                debugSpawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                            }
                            else if (up)
                            {
                                debugSpawnedPiece.transform.Rotate(new Vector3(0, 180.0f, 0));
                            }
                            else if (right)
                            {
                                debugSpawnedPiece.transform.Rotate(new Vector3(0, 270.0f, 0));
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
                                debugSpawnedPiece = UnityEngine.Object.Instantiate(debugStraight, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                                debugSpawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                            }
                            else if (up && down)
                            {
                                debugSpawnedPiece = UnityEngine.Object.Instantiate(debugStraight, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                            }
                            //turn
                            else if (left && up)
                            {
                                debugSpawnedPiece = UnityEngine.Object.Instantiate(debugTurn, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                                debugSpawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                            }
                            else if (up && right)
                            {
                                debugSpawnedPiece = UnityEngine.Object.Instantiate(debugTurn, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                                debugSpawnedPiece.transform.Rotate(new Vector3(0, 180.0f, 0));
                            }
                            else if (right && down)
                            {
                                debugSpawnedPiece = UnityEngine.Object.Instantiate(debugTurn, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                                debugSpawnedPiece.transform.Rotate(new Vector3(0, 270.0f, 0));
                            }
                            else if (down && left)
                            {
                                debugSpawnedPiece = UnityEngine.Object.Instantiate(debugTurn, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                            }
                            break;
                        case 3:
                            //tee
                            if (down && left && up)
                            {
                                debugSpawnedPiece = UnityEngine.Object.Instantiate(debugTee, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                                debugSpawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                            }
                            else if (left && up && right)
                            {
                                debugSpawnedPiece = UnityEngine.Object.Instantiate(debugTee, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                                debugSpawnedPiece.transform.Rotate(new Vector3(0, 180.0f, 0));
                            }
                            else if (up && right && down)
                            {
                                debugSpawnedPiece = UnityEngine.Object.Instantiate(debugTee, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                                debugSpawnedPiece.transform.Rotate(new Vector3(0, 270.0f, 0));
                            }
                            else if (right && down && left)
                            {
                                debugSpawnedPiece = UnityEngine.Object.Instantiate(debugTee, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
                            }
                            break;
                        case 4:
                            //cross
                            debugSpawnedPiece = UnityEngine.Object.Instantiate(debugCross, new Vector3(j * debugGridSize, 15, i * debugGridSize), new Quaternion());
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
        endRow = UnityEngine.Random.Range(0, rows - 1);
        endCol = UnityEngine.Random.Range(0, cols - 1);
        while (
                (tiles[endRow][endCol].left == true ? 1 : 0) +
                (tiles[endRow][endCol].up == true ? 1 : 0) +
                (tiles[endRow][endCol].right == true ? 1 : 0) +
                (tiles[endRow][endCol].down == true ? 1 : 0)
                != 1
            )
        {
            endRow = UnityEngine.Random.Range(0, rows - 1);
            endCol = UnityEngine.Random.Range(0, cols - 1);
        }
        Debug.Log("End Row: " + endRow + "End Col" + endCol);

        startRow = UnityEngine.Random.Range(0, rows - 1);
        startCol = UnityEngine.Random.Range(0, cols - 1);
        while (calcManhattanDistance(startRow, startCol, endRow, endCol) < minCompletionDistance)
        {
            startRow = UnityEngine.Random.Range(0, rows - 1);
            startCol = UnityEngine.Random.Range(0, cols - 1);
        }
        Debug.Log("Start Row: " + startRow + "Start Col" + startCol);

        //spawn goal tile
        Vector3 goalPosition = tiles[endRow][endCol].tileGameObject.transform.position;
        Quaternion goalRotation = tiles[endRow][endCol].tileGameObject.transform.rotation;
        Destroy(tiles[endRow][endCol].tileGameObject);

        tiles[endRow][endCol].tileGameObject = UnityEngine.Object.Instantiate(goalPrefab, goalPosition, goalRotation);

        //make player spawn tile safe
        Destroy(tiles[startRow][startCol].tileGameObject);
        spawnSafeTile(startRow, startCol);


        //spawn player
        Vector3 playerPosition = new Vector3(startCol * gridSize, 0, startRow * gridSize);
        Vector3 spawnOffset = new Vector3(0.0f, 1.03f, 0.0f);
        player = UnityEngine.Object.Instantiate(playerPrefab, playerPosition + spawnOffset, new Quaternion());

        // for (int i = 0; i < tiles.Length; i++)
        // {
        //     Debug.Log("Tile: " + i + " " + "Up: " + tiles[3][i].up + " " + "Down: " + tiles[3][i].down);
        // }

        // for (int i = 0; i < tiles.Length; i++)
        // {
        //     Debug.Log("Tile: " + i + " " + "Right: " + tiles[i][3].right + " " + "Left: " + tiles[i][3].left);
        // }
    }

    GameObject spawnTile(int i, int j)
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

        GameObject spawnedPiece = null;

        switch (numOutlets)
        {
            case 1:
                //dead end
                spawnedPiece = UnityEngine.Object.Instantiate(deadEnd[UnityEngine.Random.Range(0, deadEnd.Length)], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
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
                    spawnedPiece = UnityEngine.Object.Instantiate(straight[UnityEngine.Random.Range(0, straight.Length)], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                    spawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                }
                else if (up && down)
                {
                    spawnedPiece = UnityEngine.Object.Instantiate(straight[UnityEngine.Random.Range(0, straight.Length)], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                }
                //turn
                else if (left && up)
                {
                    spawnedPiece = UnityEngine.Object.Instantiate(turn[UnityEngine.Random.Range(0, turn.Length)], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                    spawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                }
                else if (up && right)
                {
                    spawnedPiece = UnityEngine.Object.Instantiate(turn[UnityEngine.Random.Range(0, turn.Length)], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                    spawnedPiece.transform.Rotate(new Vector3(0, 180.0f, 0));
                }
                else if (right && down)
                {
                    spawnedPiece = UnityEngine.Object.Instantiate(turn[UnityEngine.Random.Range(0, turn.Length)], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                    spawnedPiece.transform.Rotate(new Vector3(0, 270.0f, 0));
                }
                else if (down && left)
                {
                    spawnedPiece = UnityEngine.Object.Instantiate(turn[UnityEngine.Random.Range(0, turn.Length)], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                }
                tiles[i][j].tileGameObject = spawnedPiece;
                break;
            case 3:
                spawnedPiece = UnityEngine.Object.Instantiate(tee[UnityEngine.Random.Range(0, tee.Length)], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
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
                spawnedPiece = UnityEngine.Object.Instantiate(cross[UnityEngine.Random.Range(0, cross.Length)], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                tiles[i][j].tileGameObject = spawnedPiece;
                break;
            default:
                Debug.Log(numOutlets);
                break;
        }

        return spawnedPiece;
    }

    GameObject spawnSafeTile(int i, int j)
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

        GameObject spawnedPiece = null;

        switch (numOutlets)
        {
            case 1:
                //dead end
                spawnedPiece = UnityEngine.Object.Instantiate(deadEnd[0], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
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
                    spawnedPiece = UnityEngine.Object.Instantiate(straight[0], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                    spawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                }
                else if (up && down)
                {
                    spawnedPiece = UnityEngine.Object.Instantiate(straight[0], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                }
                //turn
                else if (left && up)
                {
                    spawnedPiece = UnityEngine.Object.Instantiate(turn[0], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                    spawnedPiece.transform.Rotate(new Vector3(0, 90.0f, 0));
                }
                else if (up && right)
                {
                    spawnedPiece = UnityEngine.Object.Instantiate(turn[0], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                    spawnedPiece.transform.Rotate(new Vector3(0, 180.0f, 0));
                }
                else if (right && down)
                {
                    spawnedPiece = UnityEngine.Object.Instantiate(turn[0], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                    spawnedPiece.transform.Rotate(new Vector3(0, 270.0f, 0));
                }
                else if (down && left)
                {
                    spawnedPiece = UnityEngine.Object.Instantiate(turn[0], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                }
                tiles[i][j].tileGameObject = spawnedPiece;
                break;
            case 3:
                spawnedPiece = UnityEngine.Object.Instantiate(tee[0], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
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
                spawnedPiece = UnityEngine.Object.Instantiate(cross[0], new Vector3(j * gridSize, 0, i * gridSize), new Quaternion());
                tiles[i][j].tileGameObject = spawnedPiece;
                break;
            default:
                Debug.Log(numOutlets);
                break;
        }

        return spawnedPiece;
    }

    // void Update()
    // {
    //     bool canSpawn = false;
    //     spawnTimer -= Time.deltaTime;
    //     if (spawnTimer <= 0)
    //     {
    //         canSpawn = true;
    //         //reset the timer
    //         spawnTimer = UnityEngine.Random.Range(minSpawnTimer, maxSpawnTimer);
    //         spawnTimer = 1000000.0f;
    //     }


    //     if (canSpawn)
    //     {
    //         //calculate the tile that the player is currently on
    //         //find the min distance in the list of tiles
    //         int closestTileRow = 0;
    //         int closestTileCol = 0;
    //         float closestDistance = (tiles[0][0].tileGameObject.transform.position - player.transform.position).magnitude;
    //         for (int i = 0; i < tiles.Length; i++)
    //         {
    //             for (int j = 0; j < tiles[i].Length; j++)
    //             {
    //                 float newDistance = (tiles[i][j].tileGameObject.transform.position - player.transform.position).magnitude;
    //                 if (newDistance < closestDistance)
    //                 {
    //                     closestDistance = newDistance;
    //                     closestTileRow = i;
    //                     closestTileCol = j;
    //                 }
    //             }
    //         }

    //         //travel out 2 tiles from the current tile in a random direction, and spawn
    //         int travelDistance = 2;
    //         int searchSize = travelDistance * 2 + 1;
    //         int midPoint = travelDistance + 1;
    //         bool continueSearching = true;
    //         bool spawnSuccess = false;

    //         bool[][] visitedTiles = new bool[searchSize][];
    //         for (int k = 0; k < searchSize; k++)
    //         {
    //             visitedTiles[k] = new bool[searchSize];
    //         }

    //         visitedTiles[midPoint][midPoint] = true;

    //         int rowOffset = 0;
    //         int colOffset = 0;
    //         Stack<Tuple<int, int>> traverseHistory = new Stack<Tuple<int, int>>();
    //         List<Tuple<int, int>> directionOptions = new List<Tuple<int, int>>();

    //         while (continueSearching)
    //         {
    //             //add all possible directions to a list
    //             if (rowOffset < travelDistance)
    //             {
    //                 if (tiles[closestTileRow + rowOffset][closestTileCol + colOffset].up && !visitedTiles[midPoint + rowOffset + 1][midPoint + colOffset])
    //                 {
    //                     directionOptions.Add(new Tuple<int, int>(1, 0));
    //                 }
    //             }

    //             if (rowOffset > -travelDistance)
    //             {
    //                 if (tiles[closestTileRow + rowOffset][closestTileCol + colOffset].down && !visitedTiles[midPoint + rowOffset - 1][midPoint + colOffset])
    //                 {
    //                     directionOptions.Add(new Tuple<int, int>(-1, 0));
    //                 }
    //             }

    //             if (colOffset < travelDistance)
    //             {
    //                 if (tiles[closestTileRow + rowOffset][closestTileCol + colOffset].right && !visitedTiles[midPoint + rowOffset][midPoint + colOffset + 1])
    //                 {
    //                     directionOptions.Add(new Tuple<int, int>(0, 1));
    //                 }
    //             }

    //             if (colOffset > -travelDistance)
    //             {
    //                 if (tiles[closestTileRow + rowOffset][closestTileCol + colOffset].left && !visitedTiles[midPoint + rowOffset][midPoint + colOffset - 1])
    //                 {
    //                     directionOptions.Add(new Tuple<int, int>(0, -1));
    //                 }
    //             }


    //             //if there are no possible directions to go in, then backtrack
    //             if (directionOptions.Count == 0)
    //             {
    //                 //if there are no items to backtrack to, then exit (failure)
    //                 if (traverseHistory.Count == 0)
    //                 {
    //                     continueSearching = false;
    //                     spawnSuccess = false;
    //                 }
    //                 //otherwise pop an item to backtrack to
    //                 else
    //                 {
    //                     Tuple<int, int> backtrackDirection = traverseHistory.Pop();

    //                     rowOffset -= backtrackDirection.Item1;
    //                     colOffset -= backtrackDirection.Item2;
    //                 }
    //             }
    //             //otherwise search in a random direction
    //             else
    //             {
    //                 //pick a random direction to go in
    //                 int randomIndex = UnityEngine.Random.Range(0, directionOptions.Count - 1);

    //                 //go in that direction
    //                 traverseHistory.Push(new Tuple<int, int>(directionOptions[randomIndex].Item1, directionOptions[randomIndex].Item2));
    //                 rowOffset += directionOptions[randomIndex].Item1;
    //                 colOffset += directionOptions[randomIndex].Item2;
    //                 visitedTiles[rowOffset][colOffset] = true;

    //                 //if the selected tile is travelDistance distance away from the midpoint (AKA origin of the search), then exit (success)
    //                 if (Mathf.Abs(rowOffset) + Mathf.Abs(colOffset) >= travelDistance)
    //                 {
    //                     continueSearching = false;
    //                     spawnSuccess = true;
    //                 }
    //             }
    //         }

    //         //if a place was found to spawn the enemy, then spawn it
    //         if (spawnSuccess)
    //         {
    //             Debug.Log("Closest Tile: ");
    //             Debug.Log(new Vector2(closestTileRow, closestTileCol));
    //             float spawnRadius = 10.0f;
    //             //spawn the enemy
    //             Vector3 spawnLocation = new Vector3((closestTileCol + colOffset) * gridSize, 0, (closestTileRow + rowOffset) * gridSize);
    //             Vector3 spawnLocationOffset = new Vector3(UnityEngine.Random.Range(0, spawnRadius), 1.0f, UnityEngine.Random.Range(0, spawnRadius));

    //             int enemyChoice = UnityEngine.Random.Range(0, enemyPrefabs.Length - 1);
    //             UnityEngine.Object.Instantiate(enemyPrefabs[enemyChoice], spawnLocation /*+ spawnLocationOffset*/, new Quaternion());
    //         }
    //     }
    // }

    private int calcManhattanDistance(int x1, int y1, int x2, int y2)
    {
        return Mathf.Abs(x2 - x1) + Mathf.Abs(y2 - y1);
    }
}
