using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    public CameraScreenshot screenshotManager;

    [SerializeField] public Material SkyWhite;
    [SerializeField] public Material SkyGray;

    public enum SceneName
    {
        TestSetup,
        MainMenu,
        Moodboarding,
        Room,
        Tutorial
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadSceneAdditive(SceneName sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName.ToString()).isLoaded)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    }

    public void UnloadScene(SceneName sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName.ToString()).isLoaded)
        {
            StartCoroutine(UnloadSceneAsync(sceneName));
        }
    }

    private IEnumerator LoadSceneAsync(SceneName sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if(sceneName == SceneName.Moodboarding)
        {
            screenshotManager = FindAnyObjectByType<CameraScreenshot>();
        }
    }

    private IEnumerator UnloadSceneAsync(SceneName sceneName)
    {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName.ToString());
        while (!asyncUnload.isDone)
        {
            yield return null;
        }
    }

    public void OpenMoodboardingScene()
    {
        UnloadScene(SceneName.MainMenu);
        UnloadScene(SceneName.Tutorial);
        LoadSceneAdditive(SceneName.Moodboarding);
        RenderSettings.skybox = SkyWhite;
    }

    public void OpenMainMenuScene()
    {
        if (SceneManager.GetSceneByName(SceneName.Moodboarding.ToString()).isLoaded && screenshotManager!=null)
        {
            Debug.Log("Capturing screenshot before unloading Moodboarding scene.");
            screenshotManager.CaptureScreenshotsForMoodboards();
        }
        
        UnloadScene(SceneName.Moodboarding);
        UnloadScene(SceneName.Tutorial);
        LoadSceneAdditive(SceneName.MainMenu);
        RenderSettings.skybox = SkyGray;
    }

    public void OpenTutorialScene()
    {
        UnloadScene(SceneName.MainMenu);
        LoadSceneAdditive(SceneName.Tutorial);
        RenderSettings.skybox = SkyWhite;
    }
}
