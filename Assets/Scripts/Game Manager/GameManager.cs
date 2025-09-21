using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameEvent RestartEvent;
    [SerializeField] private PlayerData playerData;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (this != Instance)
        {
            Destroy(gameObject);
        }
    }

    public void ReloadGame()
    {
        playerData.ResetPlayerStats();
        SceneManager.LoadScene((int) SceneIndexes.TESTING_SCENE);
        Time.timeScale = 1;
    }

    public void LoadScene(SceneIndexes sceneIndex){
        SceneManager.LoadScene((int) sceneIndex);
        Time.timeScale = 1;
    }
}