using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : Menu {

    public static bool isOver;

    public GameObject gameOverMenuUI;

    private void Update() {
        
    }
    public void EnterMenu() {
        isOver = true;
        Time.timeScale = 0f;
        gameOverMenuUI.SetActive(true);
    }
    public void PlayAgain() {
        Time.timeScale = 1f;
        isOver = false;
        gameOverMenuUI.SetActive(false);
        SceneManager.LoadScene(1);
    }

    public void LoadMenu() {
        isOver = false;
        Time.timeScale = 1f;
        gameOverMenuUI.SetActive(false);
        SceneManager.LoadScene("Menu");
    }
}
