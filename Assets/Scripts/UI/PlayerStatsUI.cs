using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Player Stats")]
    public PlayerData playerData;

    [Header("UI Interface")]
    public TextMeshProUGUI playerHealthText;

    public void Start(){
        UpdatePlayerHealth();
    }


    public void UpdatePlayerHealth(){
        playerHealthText.text = $"Health: {playerData.health}";
    }

    
}
