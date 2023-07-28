using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    MovementController movementContoller;

    // Start is called before the first frame update
    void Start()
    {
        movementContoller = GetComponent<MovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movementContoller.SetDirection("left");
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movementContoller.SetDirection("right");
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            movementContoller.SetDirection("up");
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movementContoller.SetDirection("down");
        }
    }
}
