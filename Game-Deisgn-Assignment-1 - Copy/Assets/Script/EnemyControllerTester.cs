using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerTester : MonoBehaviour
{
    public enum GhostNodesStatesEnum
    {
        respawning,
        leftNode,
        rightNode,
        centerNode,
        startNode,
        movingInNodes
    }

    public GhostNodesStatesEnum ghostNodeState;
    public GhostNodesStatesEnum respawnState;

    public enum GhostType
    {
        red,
        blue,
        pink,
        orange
    }

    public GhostType ghostType;
    

    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCentre;
    public GameObject ghostNodeStart;

    public MovementController movementController;

    public GameObject startingNode;

    public bool readyToLeaveHome = false;

    public GameManager gameManager;

    public bool testRespawn = false;

    public bool isFrightened = false;

    public GameObject[] scatterNodes;
    public int scatterNodeIndex;
    // Start is called before the first frame update
    void Awake()
    {
        scatterNodeIndex = 0;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementController = GetComponent<MovementController>();
        if (ghostType == GhostType.red)
        {
            ghostNodeState = GhostNodesStatesEnum.startNode;
            respawnState = GhostNodesStatesEnum.centerNode;
            startingNode = ghostNodeStart;
            readyToLeaveHome = true;
            Debug.Log("Red is at start Node");
        }
        else if (ghostType == GhostType.pink)
        {
            ghostNodeState = GhostNodesStatesEnum.centerNode;
            respawnState = GhostNodesStatesEnum.centerNode;
            startingNode = ghostNodeCentre;
        }
        else if (ghostType == GhostType.blue)
        {
            ghostNodeState = GhostNodesStatesEnum.leftNode;
            respawnState = GhostNodesStatesEnum.leftNode;
            startingNode = ghostNodeLeft;
        }
        else if (ghostType == GhostType.orange)
        {
            ghostNodeState = GhostNodesStatesEnum.rightNode;
            respawnState = GhostNodesStatesEnum.rightNode;
            startingNode = ghostNodeRight;
        }
        movementController.currentNode = startingNode;
        transform.position = startingNode.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(testRespawn == true)
        {
            readyToLeaveHome = false;
            ghostNodeState = GhostNodesStatesEnum.respawning;
            testRespawn = false;
        }

        if (movementController.currentNode.GetComponent<NodeController>().isSideNode)
        {
            movementController.SetSpeed(1);
        }
        else
        {
            movementController.SetSpeed(2);
        }
    }

    public void ReachedCenterOfNode(NodeController nodeController)
    {
        if (ghostNodeState == GhostNodesStatesEnum.movingInNodes)
        {
            //Scatter Mode
            if(gameManager.currentGhostMode == GameManager.GhostMode.scatter)
            {
               // if we reached the scatter node add 1 to our scatter node index
                if(transform.position.x == scatterNodes[scatterNodeIndex].transform.position.x && transform.position.y == scatterNodes[scatterNodeIndex].transform.position.y)
                {
                    scatterNodeIndex++;


                    if (scatterNodeIndex == scatterNodes.Length - 1)
                    {
                        scatterNodeIndex = 0;
                    }
                }
                string direction = GetClosestDirection(scatterNodes[scatterNodeIndex].transform.position);
                movementController.SetDirection(direction);
               
            }
            //Frightened mode
            else if (isFrightened)
            {

            }
            //Chase Mode
            else
            {
                if (ghostType == GhostType.red)
                {
                    DetermineRedGhostDirection();
                }
            }
            
        }
        else if (ghostNodeState == GhostNodesStatesEnum.respawning)
        {
            string direction = "";
            //we hav reached start node move to centre
            if(transform.position.x == ghostNodeStart.transform.position.x && transform.position.y == ghostNodeStart.transform.position.y)
            {
                direction = "down";
            }//we have reached centre either finish respawn or move to left or right 
            else if(transform.position.x == ghostNodeCentre.transform.position.x && transform.position.y == ghostNodeCentre.transform.position.y)
            {
                if(respawnState == GhostNodesStatesEnum.centerNode)
                {
                    ghostNodeState = respawnState;
                }
                else if (respawnState == GhostNodesStatesEnum.leftNode)
                {
                    direction = "left";
                }
                else if (respawnState == GhostNodesStatesEnum.rightNode)
                {
                    direction = "right";
                }
            }//if our respawn state is either left or right then leave home again
            else if (
                (transform.position.x == ghostNodeLeft.transform.position.x && transform.position.y == ghostNodeLeft.transform.position.y)
                || (transform.position.x == ghostNodeRight.transform.position.x && transform.position.y == ghostNodeRight.transform.position.y)
                )
            {
                ghostNodeState = respawnState;
            }
            //still in game board locate start
            else
            {

                direction = GetClosestDirection(ghostNodeStart.transform.position);
            }

            
            movementController.SetDirection(direction);
        }
        else
        {
            if (readyToLeaveHome)
            {
                if(ghostNodeState == GhostNodesStatesEnum.leftNode)
                {
                    ghostNodeState = GhostNodesStatesEnum.centerNode;
                    movementController.SetDirection("right");
                }
                else if(ghostNodeState == GhostNodesStatesEnum.rightNode)
                {
                    ghostNodeState = GhostNodesStatesEnum.centerNode;
                    movementController.SetDirection("left");
                }
                else if (ghostNodeState == GhostNodesStatesEnum.centerNode)
                {
                    ghostNodeState = GhostNodesStatesEnum.startNode;
                    movementController.SetDirection("up");
                }
                else if (ghostNodeState == GhostNodesStatesEnum.startNode)
                {
                    ghostNodeState = GhostNodesStatesEnum.movingInNodes;
                    movementController.SetDirection("left");
                }
            }
        }
    }

    void DetermineRedGhostDirection()
    {
        string direction = GetClosestDirection(gameManager.pacman.transform.position);
        movementController.SetDirection(direction);
    }
    void DeterminePinkGhostDirection()
    {

    }
    void DetermineOrangeGhostDirection()
    {

    }
    void DetermineBlueGhostDirection()
    {

    }

    string GetClosestDirection(Vector2 target)
    {
        float shortestDistance = 0;
        string lastMovingDirection = movementController.lastMovingDirection;
        string newDirection = "";

        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();
        //if ghost can move up but not reverse
        if (nodeController.canMoveUp && lastMovingDirection != "down")
        {
            //get node above us
            GameObject nodeUp = nodeController.nodeUp;
            //get the distance between top node and pacman
            float distance = Vector2.Distance(nodeUp.transform.position, target);
            //if shortest distance, set ghost direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "up";
            }
        }

        //if ghost can move up but not reverse
        if (nodeController.canMoveDown && lastMovingDirection != "up")
        {
            //get node above us
            GameObject nodeDown = nodeController.nodeDown;
            //get the distance between top node and pacman
            float distance = Vector2.Distance(nodeDown.transform.position, target);
            //if shortest distance, set ghost direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "down";
            }
        }

        //if ghost can move up but not reverse
        if (nodeController.canMoveLeft && lastMovingDirection != "right")
        {
            //get node above us
            GameObject nodeLeft = nodeController.nodeLeft;
            //get the distance between top node and pacman
            float distance = Vector2.Distance(nodeLeft.transform.position, target);
            //if shortest distance, set ghost direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "left";
            }
        }

        //if ghost can move up but not reverse
        if (nodeController.canMoveRight && lastMovingDirection != "left")
        {
            //get node above us
            GameObject nodeRight = nodeController.nodeRight;
            //get the distance between top node and pacman
            float distance = Vector2.Distance(nodeRight.transform.position, target);
            //if shortest distance, set ghost direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "right";
            }
        }

        return newDirection;
    }
}
