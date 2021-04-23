using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A super class for all menus
public class Menu : MonoBehaviour {
    
    public void QuitGame() {
        Debug.Log("Quitting game");
        Application.Quit();
    }
}
