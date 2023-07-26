using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    NavMeshAgent agent;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Debug.Log(rb.velocity.y);
        Vector2 CombinedMove = new Vector2(moveX, moveY);
        Vector2 NormalizedVelo = CombinedMove.normalized;
        

        // Move the player horizontally and vertically
        //rb.velocity = new Vector2(moveX * moveSpeed, moveY * moveSpeed);
        rb.velocity = new Vector2(CombinedMove.x,CombinedMove.y)*moveSpeed;

        if (rb.velocity.x == 0)
        {
            rb.velocity = new Vector2(CombinedMove.x +0.0001f, CombinedMove.y) * moveSpeed;
        }
    }

    private void FixedUpdate()
    {
        agent.nextPosition = transform.position;
    }
}
