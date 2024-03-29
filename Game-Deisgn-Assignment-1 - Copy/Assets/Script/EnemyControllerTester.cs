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
    public GhostNodesStatesEnum startGhostNodeState;
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
    public NodeController nodeController;

    public GameObject startingNode;

    public bool readyToLeaveHome = false;

    public GameManager gameManager;

    public bool testRespawn = false;

    public bool isFrightened = false;

    public GameObject[] scatterNodes;
    public int scatterNodeIndex;

    public bool leftHomeBefore = false;


    public bool isVisible = true;

    public SpriteRenderer ghostSprite;
    public SpriteRenderer eyesSprite;

    public Animator animator;

    public Color color; 

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        ghostSprite = GetComponent<SpriteRenderer>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementController = GetComponent<MovementController>();
        if (ghostType == GhostType.red)
        {
            startGhostNodeState = GhostNodesStatesEnum.startNode;
            respawnState = GhostNodesStatesEnum.centerNode;
            startingNode = ghostNodeStart;
           
        }
        else if (ghostType == GhostType.pink)
        {
            startGhostNodeState = GhostNodesStatesEnum.centerNode;
            respawnState = GhostNodesStatesEnum.centerNode;
            startingNode = ghostNodeCentre;
        }
        else if (ghostType == GhostType.blue)
        {
            startGhostNodeState = GhostNodesStatesEnum.leftNode;
            respawnState = GhostNodesStatesEnum.leftNode;
            startingNode = ghostNodeLeft;
        }
        else if (ghostType == GhostType.orange)
        {
            startGhostNodeState = GhostNodesStatesEnum.rightNode;
            respawnState = GhostNodesStatesEnum.rightNode;
            startingNode = ghostNodeRight;
        }
        else if (ghostType == GhostType.pink)
        {
            readyToLeaveHome = true;
        }
       


       nodeController = GameObject.FindObjectOfType<NodeController>();

    }

    public void Setup()
    {
        animator.SetBool("moving", false);

        bool v = ghostNodeState == startGhostNodeState;
        readyToLeaveHome = false;

        movementController.currentNode = startingNode;
        transform.position = startingNode.transform.position;

        movementController.direction = "";
        movementController.lastMovingDirection = "";
     
        scatterNodeIndex = 0;
        //Set isFrightened
        isFrightened = false;

        leftHomeBefore = false;
        //Set readyToLeaveHome to be false if they are Blue/Pink
        if (ghostType == GhostType.red)
        {
            readyToLeaveHome = true;
            leftHomeBefore = true;
        }
        else if(ghostType == GhostType.pink)
        {
            readyToLeaveHome = true;
        }
        SetVisible(true);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (ghostNodeState != GhostNodesStatesEnum.movingInNodes || !gameManager.isPowerPelletRunning)
        {
            isFrightened = false;
        }

        if (isVisible)
        {
            if (ghostNodeState != GhostNodesStatesEnum.respawning)
            {
                ghostSprite.enabled = true;
            }
            else
            {
                ghostSprite.enabled = false;
            }

            eyesSprite.enabled = true;
        }
        else
        {
            ghostSprite.enabled = false;
            eyesSprite.enabled = false;
        }

        if (isFrightened)
        {
            animator.SetBool("frightened", true);
            eyesSprite.enabled = false;
            ghostSprite.color = new Color(255, 255, 255, 255);
        }
        else
        {
            animator.SetBool("frightened", false);
            animator.SetBool("frightenedBlinking", false);
            ghostSprite.color = color;
        }

        if (!gameManager.gameIsRunning)
        {
            return; 
        }

        if (gameManager.powerPelletTimer - gameManager.currentPowerPelletTime <= 3)
        {
            animator.SetBool("frightenedBlinking", true);
        }
        else
        {
            animator.SetBool("frightenedBlinking", false);
        }


        animator.SetBool("moving", true);

        if(testRespawn == true)
        {
            readyToLeaveHome = false;
            ghostNodeState = GhostNodesStatesEnum.respawning;
            testRespawn = false;
        }

        if(movementController.currentNode.GetComponent<NodeController>().isSideNode)
        {
            movementController.SetSpeed(1);
        }
        else
        {
            if(isFrightened)
            {
                movementController.SetSpeed(1);
            }
            else if(ghostNodeState == GhostNodesStatesEnum.respawning)
            {
                movementController.SetSpeed(7);
            }
            else
            {
                movementController.SetSpeed(2);
            }
              
           
        }
    }

    public void SetFrightened(bool newIsFrightened)
    {
        isFrightened = newIsFrightened;
    }

    public void ReachedCenterOfNode(NodeController nodeController)
    {
        if (ghostNodeState == GhostNodesStatesEnum.movingInNodes)
        {
            leftHomeBefore = true;
            //Scatter Mode
            if(gameManager.currentGhostMode == GameManager.GhostMode.scatter)
            {
                DetermineGhostScatterModeDirection();
               
            }
            //Frightened mode
            else if (isFrightened)
            {
                string direction = GetRandomDirection();
                movementController.SetDirection(direction);
            }
            //Chase Mode
            else
            {
                if (ghostType == GhostType.red)
                {
                    DetermineRedGhostDirection();
                }
                else if(ghostType == GhostType.pink)
                {
                    DeterminePinkGhostDirection();
                }
                else if(ghostType == GhostType.blue)
                {
                    DetermineBlueGhostDirection();
                }
                else if(ghostType == GhostType.orange)
                {
                    DetermineOrangeGhostDirection();
                }
            }
            
        }
        else if (ghostNodeState == GhostNodesStatesEnum.respawning)
        {
            string direction = "";
            //we hav reached start node move to centre
            if (transform.position.x == ghostNodeStart.transform.position.x && transform.position.y == ghostNodeStart.transform.position.y)
            {
                direction = "down";
            }//we have reached centre either finish respawn or move to left or right 
            else if (transform.position.x == ghostNodeCentre.transform.position.x && transform.position.y == ghostNodeCentre.transform.position.y)
            {
                if (respawnState == GhostNodesStatesEnum.centerNode)
                {
                    ghostNodeState = respawnState;
                }
                else if (respawnState == GhostNodesStatesEnum.leftNode)
                {
                    direction = "left";
                   // ghostNodeState = respawnState;
                }
                else if (respawnState == GhostNodesStatesEnum.rightNode)
                {
                    direction = "right";
                   // ghostNodeState = respawnState;
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

    void DetermineGhostScatterModeDirection()
    {
        // if we reached the scatter node add 1 to our scatter node index
        if (transform.position.x == scatterNodes[scatterNodeIndex].transform.position.x && transform.position.y == scatterNodes[scatterNodeIndex].transform.position.y)
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

    string GetRandomDirection()
    {
        List<string> possibleDirections = new List<string>();
        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        if(nodeController.canMoveDown && movementController.lastMovingDirection != "up")
        {
            possibleDirections.Add("down");
        }
        if (nodeController.canMoveUp && movementController.lastMovingDirection != "down")
        {
            possibleDirections.Add("up");
        }
        if (nodeController.canMoveRight && movementController.lastMovingDirection != "left")
        {
            possibleDirections.Add("right");
        }
        if (nodeController.canMoveLeft && movementController.lastMovingDirection != "right")
        {
            possibleDirections.Add("left");
        }

        string direction = "";
        int randomDirectionIndex = Random.Range(0, possibleDirections.Count - 1);
        //int randomDirectionIndex = Random.Range(0, possibleDirections.Count^1);
        direction = possibleDirections[randomDirectionIndex];
        return direction;
    }
    void DetermineRedGhostDirection()
    {
        string direction = GetClosestDirection(gameManager.pacman.transform.position);
        movementController.SetDirection(direction);
    }
    void DeterminePinkGhostDirection()
    {
        string pacmansDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distanceBetweenNodes = 0.35f;

        Vector2 target = gameManager.pacman.transform.position;
        if(pacmansDirection == "left")
        {
            target.x -= distanceBetweenNodes * 2;
        }
        else if (pacmansDirection == "right")
        {
            target.x += distanceBetweenNodes * 2;
        }
        else if (pacmansDirection == "up")
        {
            target.y += distanceBetweenNodes * 2;

        }
        else if (pacmansDirection == "down")
        {
            target.y -= distanceBetweenNodes * 2;
        }
        string direction = GetClosestDirection(target);
        movementController.SetDirection(direction);
    }
    void DetermineOrangeGhostDirection()
    {
        float distance = Vector2.Distance(gameManager.pacman.transform.position, transform.position);
        float distanceBetweenNodes = 0.35f;
        if (distance < 0)
        {
            distance *= -1;
        }
        //if we are in 8 nodes of pacMan chase him in same manner as red
        if (distance <= distanceBetweenNodes * 8)
        {
            DetermineRedGhostDirection();
        }
        //otherwise use scattermode
        else
        {
            DetermineGhostScatterModeDirection();
        }
    }
    void DetermineBlueGhostDirection()
    {
        string pacmansDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distanceBetweenNodes = 0.35f;

        Vector2 target = gameManager.pacman.transform.position;
        if (pacmansDirection == "left")
        {
            target.x -= distanceBetweenNodes * 2;
        }
        else if (pacmansDirection == "right")
        {
            target.x += distanceBetweenNodes * 2;
        }
        else if (pacmansDirection == "up")
        {
            target.y += distanceBetweenNodes * 2;

        }
        else if (pacmansDirection == "down")
        {
            target.y -= distanceBetweenNodes * 2;
        }

        GameObject redGhost = gameManager.redGhost;
        float xDistance = target.x - redGhost.transform.position.x;
        float yDistance = target.y - redGhost.transform.position.y;

        Vector2 blueTarget = new Vector2(target.x + xDistance, target.y + yDistance);
        string direction = GetClosestDirection(blueTarget);
        movementController.SetDirection(direction);
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

    public void SetVisible(bool newIsVisible)
    {
        isVisible = newIsVisible;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && ghostNodeState != GhostNodesStatesEnum.respawning)
        {
            //Get eaten
            if(isFrightened)
            {
                gameManager.GhostEaten();
                ghostNodeState = GhostNodesStatesEnum.respawning;
            }
            //Eat Player
            else
            {
                StartCoroutine(gameManager.PlayerEaten());
            }
        }
    }
}
