using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class Player : MovingObject {

    public int wallDamage = 1;
    public int foodPoints = 10;
    public int pointsPop = 20;
    public float restartLevelDelay = 1.0f;

    private Animator animator;
    private int food;

	// Use this for initialization
	protected override void Start ()
    {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;

        base.Start();	
	}

    public void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }

    void Update()
    {
        if(!GameManager.instance.playersTurn)
        {
            return;
        }

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        //prevents diagonal player movement
        if(horizontal != 0)
        {
            vertical = 0;
        }

        if(horizontal != 0 || vertical != 0)
        {
            //By passing in the Wall object you are expecting the player to reach walls it can interact with
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if(other.tag == "food")
        {
            food += foodPoints;
            other.gameObject.SetActive(false);
        }
        else if(other.tag == "Pop")
        {
            food += pointsPop;
            other.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    //food is lost when enemy attacks the player
    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if(food <= 0)
        {
            GameManager.instance.GameOver();
        }
    }
}
