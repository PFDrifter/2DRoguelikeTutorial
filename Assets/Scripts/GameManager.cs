using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public BoardManager board;

    private int level = 3;

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
        board = GetComponent<BoardManager>();
        InitGame();
	}
	
    void InitGame()
    {
        board.SetupScene(level);
    }

	// Update is called once per frame
	void Update () {
	
	}
}
