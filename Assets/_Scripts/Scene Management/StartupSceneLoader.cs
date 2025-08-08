using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupSceneLoader : MonoBehaviour
{
    private void Start()
    {
       LoadIfNotLoaded("TestSetUp");
    }

    public void LoadMenuAndRoom()
    {
        LoadIfNotLoaded("MainMenu");
        LoadIfNotLoaded("Room");
    }

    void LoadIfNotLoaded(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}
