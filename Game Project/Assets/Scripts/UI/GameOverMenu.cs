using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : Menu {

    public bool isOver;

    public GameObject gameOverMenuUI;

    private static readonly string gameWonText = "Demo completed!";
    private static readonly string gaméLostText = "You are dead!";

    public void EnterMenu(bool won) {
        isOver = true;
        Time.timeScale = 0f;
        if (won) {
            gameOverMenuUI.GetComponentInChildren<TextMeshProUGUI>().SetText(gameWonText);
        } else {
            gameOverMenuUI.GetComponentInChildren<TextMeshProUGUI>().SetText(gaméLostText);
        }
        gameOverMenuUI.SetActive(true);
    }
    public void PlayAgain() {
        gameOverMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isOver = false;
        SceneManager.LoadScene(1);
    }

    public void LoadMenu() {
        isOver = false;
        Time.timeScale = 1f;
        gameOverMenuUI.SetActive(false);
        SceneManager.LoadScene("Menu");
    }
}
