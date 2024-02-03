using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    void Awake()
    {
        // freeze the game but still allow UI interaction
        Time.timeScale = 0;
        GetComponent<Image>().enabled = true;
    }

    public void StartGame()
    {
        // unfreeze the game
        Time.timeScale = 1;
        // destroy the start screen
        Destroy(gameObject);
    }
}
