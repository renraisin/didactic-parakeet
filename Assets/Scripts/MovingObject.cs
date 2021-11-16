using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour //abstract lets us create incomplete classes
{
    public float moveTime = 0.1f; //time in seconds for movement
    public LayerMask blockingLayer; //checks collision here

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime; //makes movement more efficient

    protected virtual void Start()
    {
        //gets boxCollider2D reference
        boxCollider = GetComponent<BoxCollider2D>();

        //gets rigidbody reference
        rb2D = GetComponent<Rigidbody2D>();

        //make movement efficient
        inverseMoveTime = 1f / moveTime;
    }

    //returns true if able to move and false if not
    //takes x y and RaycastHit2D for collision check

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit2D)
    {
        //start position
        Vector2 start = transform.position;

        //calculate end position based on direction paramaters xdir and ydir
        Vector2 end = start + new Vector2(xDir, yDir);

        //disables boxcollider so that you can't hit your own
        boxCollider.enabled = false;

        //casts a line from start point to end point, checking collision
        hit2D = Physics2D.Linecast(start, end, blockingLayer);

        //re enables boxCollider
        boxCollider.enabled = true;

        //check if anything was hit

        if (hit2D.transform == null)
        {
            //if nothing hit, start SmoothMovement co-routine passing in the Vector2 end as the destination
            StartCoroutine(SmoothMovement(end));

            //return true to say that Move was successful

            return true;
        }

        //return false if somethin was hit
        return false;
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        //calculates remaining distance to move based on the square magnitutde of difference btwn sart and end
        //sq magnitude is used instead of magnitude bc it's cheaper to compute
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //while that distance is greater than a very small amount (almost zero)
        while(sqrRemainingDistance > float.Epsilon)
        {
            // finds position closer to the end, based on moveTime
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            //calls MovePosition on attached Rigidbody2D and moves it to calculated position
            rb2D.MovePosition(newPosition);

            //recalculates remaining distance
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //loops until sqrRemainingDistance is close enough to 0 to end the function
            yield return null;
        }

    }
    
    //the virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword
    //AttemptMove takes a generic parameter T to specify the type of component our unit will interact with if blocked (e.g. player and enemies, wall and player)

    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T : Component
    {
        //hit stores where linecast hits when Move is called
        RaycastHit2D hit;

        //sets canMove to True if Move is successful, false if failed
        bool canMove = Move(xDir, yDir, out hit);

        // check if nothing was hit by linecast
        if(hit.transform == null)
        {
            //if nothing was hit, return and stop
            return;
        }

        //gets component reference to the component of type T attached to the object that was hit
        T hitComponent = hit.transform.GetComponent<T>();

        //if canMove is false and hitComponent is not null, MovingOBject is blocked and has hit something it can interact with
        if(!canMove && hitComponent != null)
        {
            //call the OnCantMove function and pass it hitComponent
            OnCantMove(hitComponent);
        }
    }

    //signifies that the thing being modified is missing or incomplete implementation
    //this is overridden by functions in the inheriting classes
    protected abstract void OnCantMove <T> (T component)
   
        where T : Component;
    


  
}
