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

    public bool isGhost = false;

    // Start is called before the first frame update
    void Awake()
    {
        
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.gameIsRunning)
        {
            return;
        }

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
            if (isGhost)
            {
                GetComponent<EnemyControllerTester>().ReachedCenterOfNode(currentNodeController);
            }
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
                if (currentNodeController.isGhostStartingNode && direction == "down" && (!isGhost || GetComponent<EnemyControllerTester>().ghostNodeState != EnemyControllerTester.GhostNodesStatesEnum.respawning))
                {
                    direction = lastMovingDirection;
                }
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

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetDirection(string newDirection)
    {
        direction = newDirection;

    }
}
