using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
       public static CurrencyManager Instance { get; private set; }
      public int Currency = 0;
      TextMeshProUGUI scoreText;
    // Start is called before the first frame update
   void Start()
{
    GameObject gameObject = GameObject.FindWithTag("Score");
    scoreText = gameObject.GetComponent<TextMeshProUGUI>();
    if (Instance != null && Instance != this) 
    { 
        Destroy(gameObject); // Destroy the current GameObject, not just this component
    } 
    else 
    { 
        Instance = this; 
        DontDestroyOnLoad(gameObject);
    } 
}
    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Money: " + Currency;
    }
}
