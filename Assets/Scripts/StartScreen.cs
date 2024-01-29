using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    void Awake()
    {
        // freeze the game but still allow UI interaction
        Time.timeScale = 0;
    }

    public void StartGame()
    {
        // unfreeze the game
        Time.timeScale = 1;
        // destroy the start screen
        Destroy(gameObject);
    }
}
