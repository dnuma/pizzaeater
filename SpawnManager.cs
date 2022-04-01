using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject pizzaPrefab;
    public GameObject speedBoosterPrefab;
    public GameObject strenghtBoosterPrefab;
    public GameObject retarderBoosterPrefab;
    public GameObject playerPrefab;

    private float[] spawnRangeX = new float[] { -8f, 8f };
    private float[] spawnRangeZ = new float[] { -16f, 8f };
    private int pizzaLeftOnGame;
    //private int enemyLeftOnGame;
    private int pizzasToSpawn = 0;
    
    private int enemyCounter = 1;

    private enum whatToSpawn : int
    {
        enemy = 0,
        food = 1
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        //Keep track of how many pizzas/enemies are left in the scene. Pizzas/Enemies will be destroyed in Pizza.cs/Enemies.cs 
        pizzaLeftOnGame = FindObjectsOfType<Pizza>().Length;
        //enemyLeftOnGame = FindObjectsOfType<Enemy>().Length;

        if (pizzaLeftOnGame < 1)
        {
            pizzasToSpawn++;
            SpawnPizza(pizzasToSpawn);
            //Spawn enemy
            if (pizzasToSpawn % 2 == 0)
            {
                enemyCounter++;
                SpawnEnemy(enemyCounter);
                SpawnBoosters();
            }
            if (pizzasToSpawn % 3 == 0)
            {
                Instantiate(retarderBoosterPrefab, GenerateSpawnPosition((int)whatToSpawn.food), retarderBoosterPrefab.transform.rotation);
            }
        }
    }

    // Spawn Strenght and Speed boosters
    private void SpawnBoosters()
    {
        Instantiate(speedBoosterPrefab, GenerateSpawnPosition((int)whatToSpawn.food), speedBoosterPrefab.transform.rotation);
        Instantiate(strenghtBoosterPrefab, GenerateSpawnPosition((int)whatToSpawn.food), strenghtBoosterPrefab.transform.rotation);
    }
    private void SpawnEnemy(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition((int)whatToSpawn.enemy), enemyPrefab.transform.rotation);
        }
    }

    private void SpawnPizza(int pizzasToSpawn)
    {
        for (int i = 0; i < pizzasToSpawn; i++)
        {
            Instantiate(pizzaPrefab, GenerateSpawnPosition((int)whatToSpawn.food), pizzaPrefab.transform.rotation);
        }
    }

    
    private Vector3 GenerateSpawnPosition(int spawnSelector)
    {
        bool farFromPlayer = false;
        float randomPosX, randomPosZ, randomPosY;
        float distanceFromPlayer;


        //Set the enemy on the ground or the food floating
        if (spawnSelector == (int)whatToSpawn.enemy) 
            randomPosY = 0f;
        //Set the food 0.5f above the ground (floats in the air)
        else
            randomPosY = 0.5f;

        //Generate positions until the element is far away from the player
        do
        {
            randomPosZ = Random.Range(spawnRangeZ[0], spawnRangeZ[1]);
            randomPosX = Random.Range(spawnRangeX[0], spawnRangeX[1]);
            distanceFromPlayer = Vector3.Distance(playerPrefab.transform.position, new Vector3(randomPosX, 0, randomPosZ));
            
            if (distanceFromPlayer > 7f)
            {
                farFromPlayer = true;
            }
        } while (!farFromPlayer);
         
        return new Vector3(randomPosX, randomPosY, randomPosZ);
    }

}
