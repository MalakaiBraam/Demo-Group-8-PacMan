using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject currentNode;
    public float speed = 4f;

    public string direction = "";
    public string lastMovingDirection = "";

    public bool canWarp = true;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        NodeController currentNodeController = currentNode.GetComponent<NodeController>();

        transform.position = Vector2.MoveTowards(transform.position, currentNode.transform.position, speed * Time.deltaTime);

        bool reverseDirection = false;
        if(
           (direction == "left" && lastMovingDirection == "right")
            || (direction == "right" && lastMovingDirection == "left")
            || (direction == "up" && lastMovingDirection == "down")
            || (direction == "down" && lastMovingDirection == "up")
           )
        {
            reverseDirection = true;
        }
        //Check if we are at the center of the currentNode
        if ((transform.position.x == currentNode.transform.position.x && transform.position.y == currentNode.transform.position.y) ||reverseDirection)
        {
            //if we reached the center of left warp, warp to right warp
            if (currentNodeController.isWarpLeftNode && canWarp)
            {
                currentNode = gameManager.rightWarpNode;
                direction = "left";
                lastMovingDirection = "left";
                transform.position = currentNode.transform.position;
                canWarp = false;
            }
            //if we reached the center of right warp, warp to left warp
            else if (currentNodeController.isWarpRightNode && canWarp)
            {
                currentNode = gameManager.leftWarpNode;
                direction = "right";
                lastMovingDirection = "right";
                transform.position = currentNode.transform.position;
                canWarp = false;
            }
            //Otherwise find the next node we are moving towards
            else
            {
                //Gets the next node from the node controller using the current direction
                GameObject newNode = currentNodeController.GetNodeFromDirection(direction);

                //checks if we can move in the direction we want it to move in
                if (newNode != null)
                {
                    currentNode = newNode;
                    lastMovingDirection = direction;
                }
                //we can't move in the desired direction but tries to move in the last direction
                else
                {
                    direction = lastMovingDirection;
                    newNode = currentNodeController.GetNodeFromDirection(direction);
                    if (newNode != null)
                    {
                        currentNode = newNode;
                    }
                }
            }

            
        }
        //We arent in the center of a node
        else
        {
            canWarp = true;
        }
    }

    public void SetDirection(string newDirection)
    {
        direction = newDirection;

    }
}
