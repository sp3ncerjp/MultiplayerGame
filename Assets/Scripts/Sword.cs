using UnityEngine;
using Unity.Netcode;

public class Sword : NetworkBehaviour
{
    private bool isAttack = false;                  //Bool to track if attacking

    public void StartAttack()                       //Function to call from player to notify the sword its swinging
    {
        isAttack = true;
    }

    public void EndAttack()                        //Function to call from player to notify the sword is no longer swinging
    {
        isAttack = false;
    }

    void OnTriggerEnter(Collider other)                     //When the sword collider collides with another collider
    {
        if (isAttack && other.CompareTag("Enemy"))          //If we are currently swinging and hitting an enemy
        {
            RequestDestroyEnemyServerRpc(other.gameObject.GetComponent<NetworkObject>().NetworkObjectId);            //Then destroy the enemy GameObject from the server level
        }
    }

    [ServerRpc]                                                 //Only on the server
    public void RequestDestroyEnemyServerRpc(ulong enemyNetworkObjectId)
    {
        //Find the enemy object on the server using its NetworkObjectId
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(enemyNetworkObjectId, out NetworkObject enemyNetworkObject))
        {
            enemyNetworkObject.Despawn(); //Destroy the object across all clients
        }
    }
}
