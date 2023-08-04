using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public bool canMoveLeft = false;
    public bool canMoveRight= false;
    public bool canMoveUp= false;
    public bool canMoveDown = false;

    public GameObject nodeLeft;
    public GameObject nodeRight;
    public GameObject nodeUp;
    public GameObject nodeDown;

    public bool isWarpRightNode = false;
    public bool isWarpLeftNode = false;

    public SpriteRenderer pelletSprite;

    public GameManager gameManager;

    public bool isGhostStartingNode = false;
    //if the node contains a pallet when the game starts
    public bool isPelletNode = false;
    //if the node has a pallet
    public bool hasPellet = false;

    public bool isSideNode = false;

    public bool isPowerPellet = false;

    public float powerPelletBlinkingTimer = 0;


    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (transform.childCount > 0)
        {
            gameManager.GotPelletFromNodeController(this);
            hasPellet = true;
            isPelletNode = true;
            pelletSprite = GetComponentInChildren<SpriteRenderer>(); 
        }

        RaycastHit2D[] hitsDown;
        //Shoot a raycast line going down
        hitsDown = Physics2D.RaycastAll(transform.position, -Vector2.up);

        //Loop through all gameobjects that the raycast hits 
        for(int i = 0; i < hitsDown.Length; i++)
        {
            float distance = Mathf.Abs(hitsDown[i].point.y - transform.position.y);
            if (distance < 0.4f)
            {
                canMoveDown = true;
                nodeDown = hitsDown[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitsUp;
        //Shoot a raycast line going down
        hitsUp = Physics2D.RaycastAll(transform.position, Vector2.up);

        //Loop through all gameobjects that the raycast hits 
        for (int i = 0; i < hitsUp.Length; i++)
        {
            float distance = Mathf.Abs(hitsUp[i].point.y - transform.position.y);
            if (distance < 0.4f)
            {
                canMoveUp = true;
                nodeUp = hitsUp[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitsRight;
        //Shoot a raycast line going down
        hitsRight = Physics2D.RaycastAll(transform.position, Vector2.right);

        

        //Loop through all gameobjects that the raycast hits 
        for (int i = 0; i < hitsRight.Length; i++)
        {
            //Debug.DrawRay(transform.position, Vector2.right, Color.red,100f);
            //Debug.DrawRay(transform.position, -Vector2.right, Color.blue,100f);
            //Debug.DrawLine(transform.position, hitsRight[i].point, Color.red,100f);

            float distance = Mathf.Abs(hitsRight[i].point.x - transform.position.x);
            if (distance < 0.4f)
            {
              
                canMoveRight = true;
                nodeRight = hitsRight[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitsLeft;
        //Shoot a raycast line going down
        hitsLeft = Physics2D.RaycastAll(transform.position, -Vector2.right);

        //Loop through all gameobjects that the raycast hits 
        for (int i = 0; i < hitsLeft.Length; i++)
        {
            float distance = Mathf.Abs(hitsLeft[i].point.x - transform.position.x);
            if (distance < 0.4f)
            {
                canMoveLeft = true;
                nodeLeft = hitsLeft[i].collider.gameObject;
            }
        }

        if (isGhostStartingNode)
        {
            canMoveDown = true;
            nodeDown = gameManager.ghostNodeCentre;
        }
    }

    // Update is called once per frame
    void Update()
    {
       

        if (!gameManager.gameIsRunning)
        {
            return;
        }

        if(isPowerPellet && hasPellet)
        {
            powerPelletBlinkingTimer += Time.deltaTime;
            if(powerPelletBlinkingTimer >= 0.1f)
            {
                powerPelletBlinkingTimer = 0;
                pelletSprite.enabled = !pelletSprite.enabled;
            }
        }
    }

    public GameObject GetNodeFromDirection(string direction)
    {
      if (direction == "left" && canMoveLeft)
        {
            return nodeLeft;
        }
      else if (direction == "right" && canMoveRight)
        {
            return nodeRight;
        }
      else if (direction == "up" && canMoveUp)
        {
            return nodeUp;
        }
      else if (direction == "down" && canMoveDown)
        {
            return nodeDown;
        }
      else
        {
            return null;
        }
    }

    public void RespawnPellet()
    {
        if(isPelletNode)
        {
            hasPellet = true;
            pelletSprite.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && hasPellet)
        {
            hasPellet = false;
            pelletSprite.enabled = false;
            StartCoroutine(gameManager.CollectedPellet(this));
        }
    }
}
