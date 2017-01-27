using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public float turnDelay = 0.1f;
    public static GameManager instance = null;
    public BoardManager board;
    public int playerFoodPoints = 100;

    private int level = 3;
    private List<Enemy> enemies;
    private bool enemiesMoving;

    //hidden from inspector view
    [HideInInspector]public bool playersTurn = true;

	// Use this for initialization
	void Awake ()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        //Allows game object persistence between scenes
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        board = GetComponent<BoardManager>();
        InitGame();
	}
	
    void InitGame()
    {
        //clears out enemies from the last level
        enemies.Clear();
        board.SetupScene(level);
    }

    public void GameOver()
    {
        enabled = false;
    }

	// Update is called once per frame
	void Update ()
    {
        if(playersTurn || enemiesMoving)
        {
            return;
        }

        StartCoroutine(MoveEnemies());
	}

    //Used to have enemies registered with game manager so that it can issue movement orders
    public void AddEnemiesToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);

        if(enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        foreach (Enemy enemy in enemies)
        {
            enemy.MoveEnemy();
            yield return new WaitForSeconds(enemy.moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}
