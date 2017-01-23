using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count
    {
        public int min;
        public int max;

        public Count (int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }

    public int cols = 8;
    public int rows = 8;

    public Count wallNum = new Count(5, 9);
    public Count foodNum = new Count(1, 5);

    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;
    private List<Vector3> gridPos = new List<Vector3>();

    void InitializeList()
    {
        gridPos.Clear();

        for (int col = 1; col < cols - 1; col++)
        {
            for(int row = 1; row < rows - 1; row++)
            {
                gridPos.Add(new Vector3(col, row, 0.0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int col = -1; col < cols + 1; col++)
        {
            for (int row = -1; row < rows + 1; row++)
            {
                GameObject toUse = floorTiles[Random.Range(0, floorTiles.Length)];

                if(col == -1 || col == cols || row == -1 || row == rows)
                {
                    toUse = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                // 0.0f for the Z axis since the game is in 2D. Quaturnion.identity is created with no rotation.
                GameObject instance = Instantiate(toUse, new Vector3(row, col, 0.0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPos.Count);
        Vector3 randomPosition = gridPos[randomIndex];
        gridPos.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tiles, int min, int max)
    {
        //Controls how many of a certain object to spawn
        int objectCount = Random.Range(min, max + 1);

        for(int i = 0; i < objectCount; i++)
        {
            Vector3 randomPos = RandomPosition();
            GameObject tileChoice = tiles[Random.Range(0, tiles.Length)];
            Instantiate(tileChoice, randomPos, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, wallNum.min, wallNum.max);
        LayoutObjectAtRandom(foodTiles, foodNum.min, foodNum.max);

        //Used to create enemy difficulty which follows a log difficulty progression
        int enemyCount = (int)Mathf.Log(level, 2.0f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        Instantiate(exit, new Vector3(cols - 1, rows - 1, 0.0f), Quaternion.identity);
    }
}
