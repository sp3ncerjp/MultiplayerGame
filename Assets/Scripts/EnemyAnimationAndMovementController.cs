using UnityEngine;
using Unity.Netcode;

public class EnemyAnimationAndMovementController : NetworkBehaviour
{
    Animator animator;                                      //Animator component variable

    int frameCount = 0;                                     //Various tracking varibales
    GameObject target = null;
    [SerializeField] float enemySpeed = 5.0f;
    public float attackRange = 5.0f;
    public float attackCooldown = 2.0f;
    private float lastAttack = -99.0f;
    public Axe axe;                                         //Axe object for the axe script

    bool isMoving = false;                                  //Booleans for animation
    bool isAttack = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();                //Set the animator
    }

    void handleAnimation()
    {
        bool isWalking = animator.GetBool("isWalking");             //Grab the current calues of the animation parameters
        bool isAttacking = animator.GetBool("isAttacking");

        if (isMoving && !isWalking)                                 //If the enemy is moving but not walking, set the animation to true
        {
            animator.SetBool("isWalking", true);
        }
        else if (!isMoving && isWalking)                            //If the enemy is not moving but walking, set the animation to false
        {
            animator.SetBool("isWalking", false);
        }

        if (isAttack && !isAttacking)                               //If the enemy is attacking but not swinging, set the animation to true
        {
            animator.SetBool("isAttacking", true);
            axe.StartAttack();                                      //Tell the axe that it is being swung
        }
        else if (!isAttack && isAttacking)                               //If the enemy is not attacking but swinging, set the animation to false
        {
            animator.SetBool("isAttacking", false);
            axe.EndAttack();                                      //Tell the axe that it is no longer being swung
        }
    }

    GameObject findClosestPlayer()                                //Simple function that returns the closest player
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");        //Initialize an array with all the current players
        float closestDistance = Mathf.Infinity;
        GameObject closestPlayer = null;

        foreach (GameObject player in players)                                      //For each player check the distance between the enemy and them, then return the closest one
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayer < closestDistance)
            {
                closestDistance = distanceToPlayer;
                closestPlayer = player;
            }
        }

        return closestPlayer;
    }

    bool IsPlayerInRange()                                              //Function checking if the current target is within striking distance
    {
        if (target != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);       //Get the distance between the enemy and the target
            return distanceToPlayer <= attackRange;                                                     //If it is less than or equal to the range return true
        }
        return false;
    }

    void MoveTowardsTarget()                                            //Simple movement function for the enemy
    {
        if (target != null)                                             //As long as they have a target
        {
            isMoving = true;                                            //Set that they are moving for the animation

            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;    //Get the direction that the target is from the enemy

            transform.position += directionToTarget * enemySpeed * Time.deltaTime;            //Move the enemy towards the target

            transform.LookAt(target.transform.position);                    //Rotate the enemy to face the target directly
        }
        else
        {
            isMoving = false;                                           //If they dont have a target set moving animation to false
        }
    }


    void Update()
    {
        if (frameCount >= 10)                                       //Only check for the closest player every 10 frames to lessen the load
        {
            frameCount = 0;                                         //Reset the counter and set the target
            target = findClosestPlayer();
        }

        if (IsPlayerInRange() && Time.time > lastAttack + attackCooldown)   //If the player is close enough and the enemy has not attacked in the last few seconds
        {
            isAttack = true;                                                //Set the attack animation to true
            lastAttack = Time.time;                                         //Update the last attack time
        }
        else                                                             
        {
            isAttack = false;                                               //Else set the attack animation to false
        }

        handleAnimation();                                                  //Update the animations and movement
        MoveTowardsTarget();

        frameCount++;                                                       //Increment frame count for findClosestPlayer
    }
}
