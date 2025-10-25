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
        // TODO: Figure out how to fix this shit, it's really stinkeh
        playerData.ResetPlayerStats();
        SceneManager.LoadScene((int)SceneIndexes.TESTING_SCENE);
        SetTimeScale(1);
    }

    /// <summary>
    /// Loads a scene by its index. While resetting necessary parameters like time scale.
    /// </summary>
    /// <param name="sceneIndex">Index To Load</param>
    public static void LoadScene(SceneIndexes sceneIndex)
    {
        SceneManager.LoadScene((int)sceneIndex);
        SetTimeScale(1);
    }

    /// <summary>
    /// Sets the time scale and adjusts fixed delta time accordingly, so that animations and physics calculations plays smoothly.
    /// </summary>
    /// <param name="scale">Time Scale to Set</param>
    public static void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
        Time.fixedDeltaTime = 0.02f * scale;
    }
}