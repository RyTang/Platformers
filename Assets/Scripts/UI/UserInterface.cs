using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
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
        GameManager.SetTimeScale(0);
        Time.timeScale = 0;
    }

    public void CloseGameOverMenu(){
        blockerScreen.SetActive(false);
        gameOverMenu.SetActive(false);
        GameManager.SetTimeScale(1);
    }

    public void ButtonPause(){
        blockerScreen.SetActive(true);
        pauseMenu.SetActive(true);
        GameManager.SetTimeScale(0);
    }

    public void ButtonResume(){
        blockerScreen.SetActive(false);
        pauseMenu.SetActive(false);
        GameManager.SetTimeScale(1);
    }

    public void ButtonRestart()
    {
        GameManager.Instance.ReloadGame();
    }

    public void ButtonQuit(){
        Debug.LogError("Have Quit the Game");
        Application.Quit();
    }
}
