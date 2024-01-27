using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scorer : MonoBehaviour
{
    public static Scorer Instance { get; private set; }
    public TextMeshProUGUI scoreText;

    private int CurrentScore = 0;

    void Awake()
    {
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

    public void AddScore()
    {
        CurrentScore += 1;
        scoreText.text = "Money: $" + CurrentScore;
    }

    public int GetScore()
    {
        return CurrentScore;
    }
}