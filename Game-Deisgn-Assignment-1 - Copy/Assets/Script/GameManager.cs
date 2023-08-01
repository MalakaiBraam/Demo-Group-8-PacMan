using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject pacman;
    public GameObject leftWarpNode;
    public GameObject rightWarpNode;
    public AudioSource siren;
    public AudioSource munch1;
    public AudioSource munch2;
    public int currentMunch = 0;
    public int Score;
    public Text scoreText;
    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCentre;
    public GameObject ghostNodeStart;

    // Start is called before the first frame update
    void Awake()
    {
        ghostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;
       Score = 0;
       currentMunch = 0;
       siren.Play();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToScore(int amount)
    {
        Score += amount;
        scoreText.text = "Score: " + Score.ToString();
    }

    public void CollectedPallet(NodeController nodeController)
    {
        //cjecks if the currentmuch is equal to 0 and if it is, it plays the first munch sound
        if(currentMunch == 0)
        {
            munch1.Play();
            currentMunch = 1;
        }
        //cjecks if the currentmuch is equal to 1 and if it is, it plays the second munch sound
        else if (currentMunch ==1)
        {
            munch2.Play();
            currentMunch = 0;
        }

        AddToScore(10);
    }

    //Add to our score

    //Check if there are any pallets left

    //check how many pallets were eaten

    //is this a power pallet
}
