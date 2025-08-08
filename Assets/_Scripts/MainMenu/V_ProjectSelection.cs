using MixedReality.Toolkit.UX;
using System.IO;
using UnityEngine;
using TMPro;

public class V_ProjectSelection : MonoBehaviour
{
    [SerializeField] private PressableButton backButton;
    [SerializeField] private GameObject projectSmallPrefab;
    [SerializeField] private Transform projectsContainer;

    private MainMenuUiManager uiManager;
    public static VM_AppData Instance { get; private set; }

    private void Awake()
    {
        backButton.OnClicked.AddListener(OnBackClicked);
        uiManager = Object.FindAnyObjectByType<MainMenuUiManager>();

        this.transform.position = uiManager.GetMainMenuTransform().position;
        this.transform.rotation = uiManager.GetMainMenuTransform().rotation;
    }

    private void Start()
    {
        ShowProjects();
    }

    public void ShowProjects()
    {
        if (projectSmallPrefab == null || projectsContainer == null)
        {
            Debug.LogError("Prefab or container not assigned!");
            return;
        }

        if (VM_AppData.Instance.CurrentData == null || VM_AppData.Instance.CurrentData.Projects == null)
        {
            Debug.LogError("No projects available.");
            return;
        }

        foreach (Transform child in projectsContainer)
        {
            Destroy(child.gameObject);
        }

        if (VM_AppData.Instance.CurrentData.Projects.Count == 0)
        {
            GameObject noProjectsMessage = Instantiate(projectSmallPrefab, projectsContainer);
            noProjectsMessage.name = "NoProjectsMessage";
            noProjectsMessage.GetComponent<V_ProjectButton>().title.text = "No projects available";
            noProjectsMessage.GetComponent<V_ProjectButton>().brief.text = "Create a new project to get started.";
            return;
        }

        foreach (var project in VM_AppData.Instance.CurrentData.Projects)
        {

            GameObject newProject = Instantiate(projectSmallPrefab, projectsContainer);
            newProject.name = project.Id;

            newProject.GetComponent<V_ProjectButton>().title.text = project.Name;
            newProject.GetComponent<V_ProjectButton>().brief.text = project.Brief.Length > 25
               ? project.Brief.Substring(0, 25) + "..."
               : project.Brief;
            newProject.GetComponent<PressableButton>().OnClicked.AddListener(() => uiManager.ShowProjectSelected(project.Id));
        }
    }


    private void OnBackClicked()
    {
        uiManager.CloseProjectSelected();
        uiManager.ShowMainMenu();
    }
}
