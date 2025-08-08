using InputHelper;
using MixedReality.Toolkit;
using MixedReality.Toolkit.UX;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using static SceneLoader;

public class V_ProjectSelected : MonoBehaviour
{
    [SerializeField] private PressableButton OpenButton;
    [SerializeField] private PressableButton DeleteButton;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI brief;
    [SerializeField] private GameObject deleteDialog;

    [SerializeField] GameObject deleteProjectDialog;

    private MainMenuUiManager uiManager;
    private string id;

    private string inputType;

    private void Start()
    {
        OpenButton.OnClicked.AddListener(() => OnOpenClicked(TriggerSource.Button,GestureName.Null));
        DeleteButton.OnClicked.AddListener(OnDeleteClicked);
        uiManager = Object.FindAnyObjectByType<MainMenuUiManager>();
    }

    private void OnEnable()
    {
        inputType = VM_AppData.Instance.GetInputMode();

        if (inputType == InputHelper.InputMode.Hybrid.ToString() || inputType == InputHelper.InputMode.Gestures.ToString())
        {
            GestureEventManager.OnThumbsUp += HandleThumbsUp;
        }
    }

    private void OnDisable()
    {
        GestureEventManager.OnThumbsUp -= HandleThumbsUp;
    }

    private void HandleThumbsUp()
    {
        OnOpenClicked(TriggerSource.Gesture, GestureName.ThumbsUp);
    }

    public void SetProjectData(ProjectData project)
    {
        title.text = project.Name;
        brief.text = project.Brief;
        id = project.Id;
    }

    private void OnOpenClicked(TriggerSource _trigger, GestureName _gestureName)
    {
        if(!deleteDialog.activeSelf)
        {
            uiManager.OpenWorkScene();

            InteractionLogger.Instance.LogInteraction(
            actionPerformed: ActionPerformed.Open.ToString(),
            usedInputMode: _trigger.ToString(), //
            gestureName: _gestureName.ToString(),
            sourceScene: "Main Menu",
            sourceComponent: this.name
            );

            VM_AppData.Instance.SetSelectedProject(id);
        }
    }

    private void OnDeleteClicked()
    {
        deleteProjectDialog.transform.GetChild(0).gameObject.SetActive(true);
        deleteProjectDialog.GetComponentInChildren<DeleteProjectDialog>().SetUpAndShow(id);
    }

}
