using UnityEngine;
using static SceneLoader;

public class MainMenuUiManager : MonoBehaviour
{
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject ProjectCreation;
    [SerializeField] private GameObject ProjectSelection;
    [SerializeField] private GameObject ProjectSelected;

    private string projectId = string.Empty;

    public void ShowMainMenu()
    {
        ProjectCreation.SetActive(false);
        ProjectSelection.SetActive(false);
        MainMenu.SetActive(true);
    }

    public Transform GetMainMenuTransform()
    {
        return MainMenu.transform;
    }

    public void ShowProjectCreation()
    {
        MainMenu.SetActive(false);
        ProjectCreation.SetActive(true);
        ProjectCreation.GetComponent<V_ProjectCreation>().DeactivateButtonColliders();
    }

    public void ShowProjectSelection()
    {
        MainMenu.SetActive(false);
        ProjectSelection.SetActive(true);
    }

    public void ReloadProjectSelection()
    {
        ProjectSelection.GetComponent<V_ProjectSelection>().ShowProjects();
    }

    public void ShowProjectSelected(string _projectId)
    {
        // if i click on a new or different project, it should open the project
        if (projectId != _projectId)
        {
            ProjectSelected.SetActive(true);
            projectId = _projectId;
            VM_AppData.Instance.SetSelectedProject(_projectId);
            ProjectSelected.GetComponent<V_ProjectSelected>().SetProjectData(VM_AppData.Instance.GetSelectedProject());
        }
        
    }

    public void CloseProjectSelected()
    {
        ProjectSelected.SetActive(false);
        projectId = string.Empty;
        VM_AppData.Instance.SetSelectedProject(string.Empty);
    }

    public void OpenWorkScene()
    {
        SceneLoader.Instance.OpenMoodboardingScene();
    }

}
