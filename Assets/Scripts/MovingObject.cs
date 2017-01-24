using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f;
    //Used to check is space is reachable by player
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMovetime;

	// Can be overridden by inherited classes
	protected virtual void Start ()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMovetime = 1.0f / moveTime;
	}

    //out keyword causes arguments to be passed by reference. Used to return more than one value from move function.
    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
    {
        //Vector 2 has no Z axis data
        Vector2 start = transform.position;
        Vector2 end = new Vector2(xDir, yDir);

        //ensures raycast does not hit objects own collider
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        //if space we cast our line into was open & available to move into
        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        return false;
    }
	
    protected IEnumerator SmoothMovement (Vector3 end)
    {
        //Computationally cheaper than magnitude
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            //Moves point in the straight line towards a target point
            Vector3 newPos = Vector3.MoveTowards(rb2D.position, end, inverseMovetime * Time.deltaTime);
            rb2D.MovePosition(newPos);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            //Wait for frame before re-evaluating the condition
            yield return null;
        }
    }

    //T is used to specify the type of component the unit should interact with if blocked.
    //The where keyword is used to specify a component
    //The generic parameter is used since both the player and the enemy are going to inherit
    //from MovingObject. Player will need to interact with walls while enemy will need to interact
    //with the player. We do not know what type of hit component to interact with and using generic
    //will allow the classes to act accordingly depending on the type.
    protected virtual void AttemptMove <T> (int xDir, int yDir) where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
        {
            return;
        }

        T hitComponent = hit.transform.GetComponent<T>();

        //Move object is blocked
        if(!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
    }

    protected abstract void OnCantMove<T>(T component);
}
