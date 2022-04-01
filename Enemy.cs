using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float speed = 1f;
    private float speedMultiplier = 0.5f;
    private float destroyPositionY = -10f;


    private Rigidbody enemyRb;
    private GameObject player;
    public Animator enemyAnimator;
    private PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerControllerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        enemyAnimator = GetComponent<Animator>();

        //Enemy is not walk_Static / run_static states
        enemyAnimator.SetBool("Static_b", false);
        enemyAnimator.SetFloat("Speed_f", speed);
        enemyAnimator.SetFloat("Run_Multiplier", speedMultiplier);
    }

    // Update is called once per frame
    void Update()
    {
        enemyRb.transform.LookAt(player.transform.position);

        // Destroy the enemy if it fells down
        if (transform.position.y < destroyPositionY)
        {
            Destroy(gameObject);
        }

        if(playerControllerScript.gameOver)
        {
            enemyAnimator.SetFloat("Speed_f", 0);
        } 
               
    }



}
