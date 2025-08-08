using InputHelper;
using MixedReality.Toolkit.UX;
using UnityEngine;

public class DeleteProjectDialog : MonoBehaviour
{
    [SerializeField] private PressableButton noButton;
    [SerializeField] private PressableButton yesButton;
    [SerializeField] private AudioSource openDialogSound;
    [SerializeField] private MainMenuUiManager uiManager;

    private string _currentProjectId;

    private string inputType;

    void Start()
    { 
        noButton.OnClicked.AddListener(HandleNoButton);
        yesButton.OnClicked.AddListener(HandleYesButton);

        InteractionLogger.Instance.LogInteraction(
             actionPerformed: "null",
             usedInputMode: "null",
             gestureName: "null",
             sourceScene: "Main Menu",
             sourceComponent: this.name,
             custom_field: "open delete project dialog"
         );
    }
    private void OnEnable()
    {

        inputType = VM_AppData.Instance.GetInputMode();

        if (inputType ==InputMode.Hybrid.ToString() || inputType == InputMode.Gestures.ToString())
        {
            GestureEventManager.OnThumbsUp += HandleThumbGesture;
            GetComponentInParent<SwipeDetector>().enabled = true;
            //swipe detector enabled by default
        }
        if (inputType == InputMode.Traditional.ToString())
        {
            GetComponentInParent<SwipeDetector>().enabled = false;
        }

        if (inputType == InputMode.Gestures.ToString())
        {
            noButton.GetComponent<Collider>().enabled = false;
            yesButton.GetComponent<Collider>().enabled = false;
        }
    }

    private void OnDisable()
    {
        GestureEventManager.OnThumbsUp -= HandleThumbGesture;
        GetComponentInParent<SwipeDetector>().enabled = false;
    }

    private void HandleNoButton()
    {
        OnNoClicked(TriggerSource.Button, GestureName.Null);
    }

    private void HandleYesButton()
    {
        OnYesClicked(TriggerSource.Button, GestureName.Null);
    }

    private void HandleThumbGesture()
    {
        OnYesClicked(TriggerSource.Gesture, GestureName.ThumbsUp);
    }


    public void SetUpAndShow(string projectId)
    {
        _currentProjectId = projectId;
        openDialogSound.Play();
        transform.GetChild(0).gameObject.SetActive(true);
        PositioningHelper.PositionInFrontOfUser(transform.gameObject, 1.5f);
        if (inputType == InputMode.Hybrid.ToString() || inputType == InputMode.Gestures.ToString())
        {
            GetComponentInParent<SwipeDetector>().enabled = true;
        }
    }

    public void OnNoClickedGesture()
    {
        // assigned to the swipe by inspector
        OnNoClicked(TriggerSource.Gesture, GestureName.Swipe);
    }

    public void OnYesClickedGesture()
    {
        // assigned to the swipe by inspector
        OnYesClicked(TriggerSource.Gesture, GestureName.Swipe);
    }

    public void OnNoClicked(TriggerSource _trigger, GestureName _gestureName)
    {
        InteractionLogger.Instance.LogInteraction(
        actionPerformed: ActionPerformed.No.ToString(),
        usedInputMode: _trigger.ToString(), 
        gestureName: _gestureName.ToString(), 
        sourceScene: "Main Menu",
        sourceComponent: this.name
        );

        InteractionLogger.Instance.LogInteraction(
             actionPerformed: "null",
             usedInputMode: "null",
             gestureName: "null",
             sourceScene: "Main Menu",
             sourceComponent: this.name,
             custom_field: "close delete project dialog"
         );

        transform.gameObject.SetActive(false);
    }

    public void OnYesClicked(TriggerSource _trigger, GestureName _gestureName)
    {
        InteractionLogger.Instance.LogInteraction(
        actionPerformed: ActionPerformed.Yes.ToString(),
        usedInputMode: _trigger.ToString(), 
        gestureName: _gestureName.ToString(), 
        sourceScene: "Main Menu",
        sourceComponent: this.name
        );

        InteractionLogger.Instance.LogInteraction(
             actionPerformed: "null",
             usedInputMode: "null",
             gestureName: "null",
             sourceScene: "Main Menu",
             sourceComponent: this.name,
             custom_field: "close delete project dialog"
         );


        VM_AppData.Instance.DeleteProject(_currentProjectId);
        uiManager.ReloadProjectSelection();
        uiManager.CloseProjectSelected();
        transform.gameObject.SetActive(false);

    }
}
