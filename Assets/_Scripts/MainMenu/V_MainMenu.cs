using MixedReality.Toolkit.UX;
using System.Collections;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI.Extensions;

public class V_MainMenu : MonoBehaviour
{
    [SerializeField] private PressableButton newProject;
    [SerializeField] private TextMeshProUGUI newProjectLabel;
    [SerializeField] private PressableButton loadProject;
    [SerializeField] private PressableButton tutorial;
    [SerializeField] private PressableButton quitButton;

    private MainMenuUiManager uiManager;

    private void Awake()
    {
        newProject.OnClicked.AddListener(OnNewProjectClicked);
        loadProject.OnClicked.AddListener(OnLoadProjectClicked);
        quitButton.OnClicked.AddListener(OnQuitButtonClicked);
        tutorial.OnClicked.AddListener(OnTutorialButtonClicked);

        uiManager = Object.FindAnyObjectByType<MainMenuUiManager>();

        PositioningHelper.PositionInFrontOfUser(gameObject, 1f,1.5f);
    }

    private void Start()
    {
        EnableNewProjectButton();
    }

    private void OnEnable()
    {
        EnableNewProjectButton();
    }

    private void EnableNewProjectButton()
    {
        if (VM_AppData.Instance.CurrentData.Projects.Count >= 8)
        {
            newProject.enabled = false;
            newProjectLabel.enabled = true;
        }
        else
        {
            newProject.enabled = true;
            newProjectLabel.enabled = false;
        }
    }

    private void OnNewProjectClicked()
    {
        uiManager.ShowProjectCreation();
    }

    private void OnLoadProjectClicked()
    {
        uiManager.ShowProjectSelection();
    }

    private void OnTutorialButtonClicked()
    {
        SceneLoader.Instance.OpenTutorialScene();
    }

    private void OnQuitButtonClicked()
    {
        StartCoroutine(QuitAfterDelay());
    }

    private IEnumerator QuitAfterDelay()
    {
        yield return null; // aspetta un frame per sicurezza

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }


}
