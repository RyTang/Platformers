using UnityEngine;

public class LevelSelectorButton : MonoBehaviour
{
    [SerializeField] private SceneIndexes sceneIndex;

    public void OnLevelSelectButtonPressed()
    {
        GameManager.LoadScene(sceneIndex);
    }
}