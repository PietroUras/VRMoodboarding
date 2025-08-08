using InputHelper;
using MixedReality.Toolkit.UX;
using UnityEngine;

public class DeleteImageDialog : MonoBehaviour
{
    [SerializeField] private PressableButton noButton;
    [SerializeField] private PressableButton yesButton;

    [SerializeField] private AudioSource openDialogSound;
    [SerializeField] private AudioSource deleteImage;

    private GameObject _currentImageObject;
    private string _currentImageId;
    private string _currentMoodboardId;

    private string inputType;

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
            transform.GetComponentInParent<SwipeDetector>().enabled = false;

        this.GetComponent<DeleteImageDialog>().enabled = false; // to check if works
    }

    void Start()
    {
        noButton.OnClicked.AddListener(HandleNoButton);
        yesButton.OnClicked.AddListener(HandleYesButton);
        openDialogSound.Play();

        InteractionLogger.Instance.LogInteraction(
            actionPerformed: "null",
            usedInputMode: "null",
            gestureName: "null",
            sourceScene: "Moodboarding",
            sourceComponent: this.name,
            custom_field: "open delete image dialog"
        );
    }


    public void SetUp(GameObject currentImageObject, string imageID, string moodboardId)
    {
        _currentImageObject = currentImageObject;
        _currentImageId = imageID;
        _currentMoodboardId = moodboardId;
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
             custom_field: "close delete image dialog"
         );

        _currentImageObject.GetComponentInChildren<HandPullDetectorXR>().ImageBackToLastPosition();
        gameObject.SetActive(false);
    }

    public void OnYesClicked(TriggerSource _trigger, GestureName _gestureName)
    {
        InteractionLogger.Instance.LogInteraction(
         actionPerformed: ActionPerformed.Yes.ToString(),
         usedInputMode: _trigger.ToString(),
         gestureName: _gestureName.ToString(), 
         sourceScene: "Moodboarding",
         sourceComponent: this.name
         );

        InteractionLogger.Instance.LogInteraction(
             actionPerformed: "null",
             usedInputMode: "null",
             gestureName: "null",
             sourceScene: "Moodboarding",
             sourceComponent: this.name,
             custom_field: "close delete image dialog"
         );

        Destroy(_currentImageObject);
        VM_AppData.Instance.DeleteImage(_currentImageId, _currentMoodboardId);
        gameObject.SetActive(false);
    }
}
