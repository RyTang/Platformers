using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UserInterface : MonoBehaviour
{   
    [Header("Blocker Screen")]
    public GameObject blockerScreen;

    [Header("Menus")]
    public GameObject pauseMenu;
    public GameObject gameOverMenu;

    public void OpenGameOverMenu(){
        blockerScreen.SetActive(true);
        gameOverMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void CloseGameOverMenu(){
        blockerScreen.SetActive(false);
        gameOverMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void ButtonPause(){
        blockerScreen.SetActive(true);
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void ButtonResume(){
        blockerScreen.SetActive(false);
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void ButtonRestart(){
        // TODO: Need to implement this restart feature
        GameManager.instance.ReloadGame();

    }

    public void ButtonQuit(){
        Debug.LogError("Have Quit the Game");
        Application.Quit();
    }
}
