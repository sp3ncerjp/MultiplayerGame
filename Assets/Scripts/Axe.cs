using UnityEngine;

public class Axe : MonoBehaviour
{
    public AnimationAndMovementController playerController;         //Script object for the player
    private bool isAttack = false;                                  //Bool to track if swinging

    public void StartAttack()                   //Function to call from enemy to notify the axe its swinging
    {
        isAttack = true;
    }

    public void EndAttack()                   //Function to call from enemy to notify the axe its not swinging
    {
        isAttack = false;
    }

    void OnTriggerEnter(Collider other)                 //When the axe collider hits another collider
    {
        if (isAttack && other.CompareTag("Player"))     //If it is attacking and hitting a player
        {
            playerController.takeDamage();            //Call the player and damage them, not working
        }
    }
}