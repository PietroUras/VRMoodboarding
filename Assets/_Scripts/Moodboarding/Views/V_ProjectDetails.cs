using TMPro;
using UnityEngine;

public class V_ProjectDetails : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI brief;

    ProjectData currentProject;
    void Start()
    {
        currentProject = VM_AppData.Instance.GetSelectedProject();
        title.text = currentProject.Name;
        brief.text = currentProject.Brief;
    }
}
