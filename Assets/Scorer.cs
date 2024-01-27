using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Scorer : MonoBehaviour
{
    public static Scorer Instance { get; private set; }
    public TextMeshProUGUI scoreText;

    private int CurrentMoney = 20;

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
        scoreText.text = "Money: $" + CurrentMoney;
    }

    public void AddMoney()
    {
        CurrentMoney += 1;
        scoreText.text = "Money: $" + CurrentMoney;
    }

    public int GetMoney()
    {
        return CurrentMoney;
    }

    public bool TrySpendMoney(int amount)
    {
        if (CurrentMoney >= amount)
        {
            CurrentMoney -= amount;
            scoreText.text = "Money: $" + CurrentMoney;
            return true;
        }
        else
        {
            return false;
        }
    }

    internal bool CanAfford(int price)
    {
        return CurrentMoney >= price;
    }
}