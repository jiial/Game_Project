using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : Menu {

    public static bool isOver;

    public GameObject gameOverMenuUI;
    public GameObject gameWonMenuUI;

    private void Update() {
        
    }
    public void EnterMenu(bool won) {
        isOver = true;
        Time.timeScale = 0f;
        if (won) {
            gameWonMenuUI.SetActive(true);
        } else {
            gameOverMenuUI.SetActive(true);
        }
    }
    public void PlayAgain() {
        Time.timeScale = 1f;
        isOver = false;
        gameOverMenuUI.SetActive(false);
        gameWonMenuUI.SetActive(false);
        SceneManager.LoadScene(1);
    }

    public void LoadMenu() {
        isOver = false;
        Time.timeScale = 1f;
        gameOverMenuUI.SetActive(false);
        gameWonMenuUI.SetActive(false);
        SceneManager.LoadScene("Menu");
    }
}
