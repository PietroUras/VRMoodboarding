using InputHelper;
using MixedReality.Toolkit.UX;
using UnityEngine;

public class DeleteBoardDialog : MonoBehaviour
{
    [SerializeField] private PressableButton noButton;
    [SerializeField] private PressableButton yesButton;
    [SerializeField] private AudioSource openDialogSound;

    private BoardsManager boardsManager;
    private string _currentMoodboardId;

    private string inputType;

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

    public void OnNoClickedGesture()
    {
        //manually linked to swipe
        OnNoClicked(TriggerSource.Gesture, GestureName.Swipe);
    }

    public void OnYesClickedGesture()
    {
        //manually linked to swipe
        OnYesClicked(TriggerSource.Gesture, GestureName.Swipe);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        noButton.OnClicked.AddListener(HandleNoButton);
        yesButton.OnClicked.AddListener(HandleYesButton);
        boardsManager = Object.FindAnyObjectByType<BoardsManager>();
    }
    private void OnEnable()
    {
       

        inputType = VM_AppData.Instance.GetInputMode();

        if (inputType == InputHelper.InputMode.Hybrid.ToString() || inputType == InputHelper.InputMode.Gestures.ToString())
        {
            GestureEventManager.OnThumbsUp += HandleThumbGesture;
            transform.GetComponentInParent<SwipeDetector>().enabled = true;
        }
        if (inputType == InputHelper.InputMode.Traditional.ToString())
        {
            //swipe detector disabled by default
        }

        if (inputType == InputHelper.InputMode.Gestures.ToString())
        {
            noButton.GetComponent<Collider>().enabled = false;
            yesButton.GetComponent<Collider>().enabled = false;
        }


    }

    private void OnDisable()
    {

        GestureEventManager.OnThumbsUp -= HandleThumbGesture;
        if (transform.GetComponentInParent<SwipeDetector>() != null)
        {
            transform.GetComponentInParent<SwipeDetector>().enabled = false;
        }
        this.enabled = false; // disable the dialog component to stop listening for events

    }

    public void SetUp(string moodboardId)
    {
        _currentMoodboardId = moodboardId;
        openDialogSound.Play();
        this.enabled = true; // enable the dialog component to listen for events
        InteractionLogger.Instance.LogInteraction(
            actionPerformed: "null",
            usedInputMode: "null",
            gestureName: "null",
            sourceScene: "Moodboarding",
            sourceComponent: this.name,
            custom_field: "open delete board dialog"
        );
    }

    public void OnNoClicked(TriggerSource _trigger, GestureName _gestureName)
    {
        InteractionLogger.Instance.LogInteraction(
        actionPerformed: ActionPerformed.No.ToString(),
        usedInputMode: _trigger.ToString(), //
        gestureName: _gestureName.ToString(), //
        sourceScene: "Moodboarding",
        sourceComponent: this.name
        );

        InteractionLogger.Instance.LogInteraction(
             actionPerformed: "null",
             usedInputMode: "null",
             gestureName: "null",
             sourceScene: "Moodboarding",
             sourceComponent: this.name,
             custom_field: "close delete board dialog"
         );

        transform.GetChild(0).gameObject.SetActive(false);
            
    }

    public void OnYesClicked(TriggerSource _trigger, GestureName _gestureName)
    {
        InteractionLogger.Instance.LogInteraction(
        actionPerformed: ActionPerformed.Yes.ToString(),
        usedInputMode: _trigger.ToString(), //
        gestureName: _gestureName.ToString(), //
        sourceScene: "Moodboarding",
        sourceComponent: this.name
        );

        InteractionLogger.Instance.LogInteraction(
             actionPerformed: "null",
             usedInputMode: "null",
             gestureName: "null",
             sourceScene: "Moodboarding",
             sourceComponent: this.name,
             custom_field: "close delete board dialog"
         );

        boardsManager.DeleteMoodboard(_currentMoodboardId);
            transform.GetChild(0).gameObject.SetActive(false);   
    }
}
