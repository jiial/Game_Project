using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : Menu {

    public void PlayGame() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
}
