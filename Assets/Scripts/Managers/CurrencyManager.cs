using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }
    public int Currency = 0;
    TextMeshProUGUI scoreText;

    void Start()
    {
        GameObject gameObject = GameObject.FindWithTag("Score");
        scoreText = gameObject.GetComponent<TextMeshProUGUI>();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        scoreText.text = "Money: " + Currency;
    }
    //Put slider in corder, next to press l to skip
    // Show enemies left at top middle of scrren
    // Wave coutner that shows how many waves in a game
    
    // i nbuild screen, add text that says how much money you need to place tower when you don't have enough at top middle
    
}
