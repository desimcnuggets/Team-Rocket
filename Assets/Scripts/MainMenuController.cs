using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("The name of the scene to load when 'ASSUME COMMAND' is clicked.")]
    public string gameSceneName = "Main Scene"; // Updated to likely main scene name

    public void StartGame()
    {
        // Try to load by name, or fallback to build index 1
        if (Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogWarning($"Scene '{gameSceneName}' not found. Loading Scene Build Index 1.");
            SceneManager.LoadScene(1);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game requested.");
        Application.Quit();
    }
}
