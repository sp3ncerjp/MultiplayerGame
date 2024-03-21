using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemySpawner : NetworkBehaviour
{
    public List<GameObject> planes;                                     //Array of planes passed in from inspector
    public GameObject enemyPrefab;                                      //Enemy prefab to spawn in
    public float spawnRate = 2.0f;                                      //Adjustable spawn rate

    private float nextSpawnTime = 0.0f;                                 //Tracking variables

    GameObject target;

    void Update()
    {
        if (IsOwner)                                                    //If they are the server
        {
            target = GameObject.FindGameObjectWithTag("Player");        //Look to see if their is a player
            if (target != null)
            {
                if (Time.time >= nextSpawnTime)                         //If there is and the we havent spawned an enemy in a few seconds, spawn one in
                {
                    SpawnEnemyAtRandomLocation();
                    nextSpawnTime = Time.time + 1f / spawnRate;         //Reset the nextSpawnTime
                }
            }
        }
    }

    void SpawnEnemyAtRandomLocation()                                   //Function to spawn in enemy in a random location from the passed in planes
    {
        if (planes.Count == 0)
        {
            return;
        }

        GameObject selectedPlane = planes[Random.Range(0, planes.Count)];           //Choose one of the spawn planes randomly

        BoxCollider boxCollider = selectedPlane.GetComponent<BoxCollider>();        //Get the BoxCollider of the selected plane
        if (boxCollider != null)
        {
            // Generate a random position within the box collider bounds
            Vector3 randomPosition = new Vector3(Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x), 0, Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z));

            GameObject enemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);            //Spawn the enemy at the random position

            enemy.GetComponent<NetworkObject>().Spawn();                                                //Set the enemy network object spawn for the client
        }
    }
}
