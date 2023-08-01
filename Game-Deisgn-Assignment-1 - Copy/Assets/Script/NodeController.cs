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

    public SpriteRenderer palletSprite;
    public GameManager gameManager;

    public bool isGhostStartingNode = false;
    //if the node contains a pallet when the game starts
    public bool isPalletNode = false;
    //if the node has a pallet
    public bool hasPallet = false;

    public bool isSideNode = false;


    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (transform.childCount > 0)
        {
            gameManager.GotPelletFromNodeController();
            hasPallet = true;
            isPalletNode = true;
            palletSprite = GetComponentInChildren<SpriteRenderer>(); 
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && hasPallet)
        {
            hasPallet = false;
            palletSprite.enabled = false;
            gameManager.CollectedPallet(this);
        }
    }
}
