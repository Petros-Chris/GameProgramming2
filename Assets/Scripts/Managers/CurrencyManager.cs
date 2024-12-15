using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }
    public int Currency = 0;
    TextMeshProUGUI scoreText;

    void Awake()
    {
        GameObject gameObject = GameObject.FindWithTag("Score");
        scoreText = gameObject.GetComponent<TextMeshProUGUI>();

        if (Instance == null)
        {
            Instance = this;
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
}
