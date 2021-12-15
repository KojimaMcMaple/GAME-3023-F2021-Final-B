using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1.0f;

    [SerializeField]
    private Rigidbody2D rigidbody;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        rigidbody.velocity = new Vector2(inputX * moveSpeed, inputY * moveSpeed);
    }
}
