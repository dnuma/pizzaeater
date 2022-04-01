using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    public Animator playerAnimator;
    public Animator enemyAnimator;
    public ParticleSystem speedupIndicator;
    public GameObject strenghtupIndicator;
    public GameManager gameManager;
    public AudioSource foodEaten;

    private float speed = 1f;
    private float speedMultiplier;
    private float rotateSpeed = 180f;
    private float[] xBounds = new float[] { -9.5f, 9.5f };
    private float[] zBounds = new float[] { -17f, 9f }; 
    private float powerupTimer = 13f;
    private float enemyDeathTimer = 1f;
    public float powerupStrenght = 30f;

    public bool speedBoost = false;
    public bool strenghtBoost = false;
    public bool retarderBoost = false;
    public bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();

        //Character is not walk_Static / run_static states
        playerAnimator.SetBool("Static_b", false);
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //Modify the speed depending on which booster the user has
        speedMultiplier = speedBoost ? 3f : (retarderBoost ? 0.5f : 1f);

        // Do not allow the user to move out of the square
        LimitBounds();

        //Activate the speed indicator
        if(speedBoost) 
        {
            speedupIndicator.Play();
            
        }
        else
        {
            speedupIndicator.Stop();
        }

        //Activate the streght indicator
        strenghtupIndicator.transform.position = strenghtBoost ? transform.position + new Vector3(0, 0.1f, 0) : transform.position + new Vector3(0, -10f, 0);

        if (!gameOver)
        {
            MovePlayer(verticalInput, horizontalInput);
        }
        

    }
    
    // Move the player around the plane
    private void MovePlayer(float verticalInput, float horizontalInput)
    {
        // Rotate the player according to the position
        transform.Rotate(0, Input.GetAxis("Rotate") * rotateSpeed * Time.deltaTime, 0);

        //Start running animator
        if (verticalInput > 0 || horizontalInput > 0)
        {
            playerAnimator.SetFloat("Speed_f", speed);
            playerAnimator.SetFloat("Run_Multiplier", speedMultiplier);
        }
        else
        {
            playerAnimator.SetFloat("Speed_f", 0);
        }
    }

    //IEnumerator = interface, to enable the timer outside the Update loop Method with coroutines
    //powerupSelector = 0, user has more speed
    //powerupSelector = 1, user is in god mode
    //powerupSelector = 2, user is slow
    IEnumerator PowerupCountdownRoutine(int powerupSelector)
    {
        yield return new WaitForSeconds(powerupTimer);

        switch(powerupSelector)
        {
            case 0:
                speedBoost = false;
                speedupIndicator.Stop();
                break;
            case 1:
                strenghtBoost = false;
                strenghtupIndicator.SetActive(false);
                break ;
            case 2:
                retarderBoost = false;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the enemy hits the user and it doesn't have strenght Booster, game over
        if (other.gameObject.CompareTag("Enemy") && !strenghtBoost)
        {
            //Destroy(gameObject);
            Destroy(speedupIndicator.gameObject);
            Destroy(strenghtupIndicator.gameObject);
        }
        // Activate speed booster then delete the booster
        else if (other.gameObject.CompareTag("SpeedBooster"))
        {
            //Deactivate retarder booster
            retarderBoost = false;

            //Activate speed booster
            speedBoost = true;
            speedupIndicator.Play();
            StartCoroutine(PowerupCountdownRoutine(0));
            Destroy(other.gameObject);
            foodEaten.Play();
        }
        // Activate streght booster then delete the booster
        else if (other.gameObject.CompareTag("StrenghtBooster"))
        {
            strenghtBoost = true;
            strenghtupIndicator.SetActive(true);
            StartCoroutine(PowerupCountdownRoutine(1));
            Destroy(other.gameObject);
            foodEaten.Play();
        }
        // Activate retarder booster then delete the booster
        else if (other.gameObject.CompareTag("RetarderBooster"))
        {
            //Deactivate speed Bddooster
            speedBoost = false;
            speedupIndicator.Stop();

            //Activate retarder booster
            retarderBoost = true;
            StartCoroutine(PowerupCountdownRoutine(2));
            Destroy(other.gameObject);
            foodEaten.Play();
        }
        // Destroy pizza, increment pizza counter
        else if (other.gameObject.CompareTag("Pizza"))
        {
            Destroy(other.gameObject);
            gameManager.pizzasEaten++;
            foodEaten.Play();
        }
    }

    // Avoid the user to go off bounds
    private void LimitBounds()
    {   
        if (transform.position.x < xBounds[0])
        {
            transform.position = new Vector3(xBounds[0], transform.position.y, transform.position.z);
        }
        if (transform.position.x > xBounds[1])
        {
            transform.position = new Vector3(xBounds[1], transform.position.y, transform.position.z);
        }
        if (transform.position.z < zBounds[0])
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zBounds[0]);
        }
        if (transform.position.z > zBounds[1])
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zBounds[1]);
        }
    }


    // Kill enemies or kill the player
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && strenghtBoost)
        {
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();

            // Stop the enemy and start death animation
            enemyAnimator = collision.gameObject.GetComponent<Animator>();
            enemyAnimator.SetFloat("Speed_f", 0);
            enemyAnimator.SetBool("Death_b", true);
            enemyAnimator.SetInteger("DeathType_int", 1);
            enemyRb.transform.LookAt(collision.gameObject.transform.position); 

            //Start coroutine to delete the enemy's corpse >:D
            StartCoroutine(DeleteCorpse(collision.gameObject));

        }
        else if (collision.gameObject.CompareTag("Enemy") && !strenghtBoost)
        {
            gameOver = true;
            playerAnimator.SetBool("Death_b", true);
            playerAnimator.SetInteger("DeathType_int", 1);
            speedupIndicator.Stop();
            speedBoost = false;
            strenghtBoost = false;
            strenghtupIndicator.SetActive(false);
            playerRb.freezeRotation = true;
        }
        else if (collision.gameObject.CompareTag("Pizza"))
        {

        }
    }

    //Delete the enemy's corpse
    IEnumerator DeleteCorpse(GameObject corpse)
    {
        yield return new WaitForSeconds(enemyDeathTimer);
        Destroy(corpse);
    }
}
