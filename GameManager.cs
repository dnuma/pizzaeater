using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerController playerController;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    public int pizzasEaten;


    // Start is called before the first frame update
    void Start()
    {
        //pizza = gameObject.GetComponent<Pizza>();
        pizzasEaten = 0;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = $"Score: {pizzasEaten}";
        if(playerController.gameOver)
            gameOverText.gameObject.SetActive(true);
            
    }
}
