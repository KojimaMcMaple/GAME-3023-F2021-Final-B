using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardinalDirection
{
    North = 0,
    East  = 1,
    South = 2,
    West  = 3
}

public class CharacterWalkAnimController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Rigidbody2D rigidbody;

    [SerializeField]
    private CardinalDirection facing = CardinalDirection.South;

    // Update is called once per frame
    void Update()
    {
        //If the character is mostly walking up, then set direction to up
        //If the character is not walking at all, then set isWalking to false
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        Vector2 velocity = new Vector2(inputX, inputY);
        //Vector2 velocity = rigidbody.velocity;

        bool isWalking = velocity.sqrMagnitude > 0.08;

        bool isMovingHorizontally = Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y);

        if(isMovingHorizontally)
        {
            if (velocity.x < 0)
            {
                facing = CardinalDirection.West;
            }
            else
            {
                facing = CardinalDirection.East;
            }
        } else
        {
            if (velocity.y < 0)
            {
                facing = CardinalDirection.South;
            }
            else
            {
                facing = CardinalDirection.North;
            }
        }

        //If magnitude is low or 0, then isWalking = false
        //Vector2 --> int (0,1,2,3)

        //Solution 1: Dot product with each cardinal direction, if positive, then it is in that side
        //Solution 2: Turn it into an angle, then multiply the angle so it falls within range [0,3], and round it to the nearest 

        animator.SetBool("isWalking", isWalking);
        animator.SetInteger("walkDirection", (int)facing);
    }
}
