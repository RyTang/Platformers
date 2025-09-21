using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    [Tooltip("The scene to load when this LoadScene is called.")]
    public SceneIndexes sceneIndexToLoad;

    /// <summary>
    /// Loads the specified level when called.
    /// </summary>
    public void LoadScene()
    {
        Debug.Log("Loading scene: " + sceneIndexToLoad);
        GameManager.Instance.LoadScene(sceneIndexToLoad);
    }

    // TODO: Create an auto UI Button Creator for levels in the future. Use Classes for the Levels
}
