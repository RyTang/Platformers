using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameEvent RestartEvent;
    [SerializeField] private PlayerData playerData;

    void Start()
    {
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (this != instance){
            Destroy(gameObject);
        }
    }

    public void ReloadGame(){
        Debug.Log("Restarting Game");
        playerData.ResetPlayerStats();
    }
}