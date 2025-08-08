using InputHelper;
using MixedReality.Toolkit.UX;
using System.Collections;
using UnityEngine;

public class BackImageCreation : MonoBehaviour
{
    [SerializeField] private PressableButton noButton;
    [SerializeField] private PressableButton yesButton;
    [SerializeField] private AudioSource openDialogSound;

    private string inputType;
    private V_CreateImage2 createImageComponent;

    void Start()
    {
        noButton.OnClicked.AddListener(HandleNoButton);
        yesButton.OnClicked.AddListener(HandleYesButton);

        InteractionLogger.Instance.LogInteraction(
             actionPerformed: "null",
             usedInputMode: "null",
             gestureName: "null",
             sourceScene: "Moodboarding",
             sourceComponent: this.name,
             custom_field: "open back during image creation dialog"
         );
    }
    private void OnEnable()
    {
        inputType = VM_AppData.Instance.GetInputMode();



        if (inputType == InputHelper.InputMode.Hybrid.ToString() || inputType == InputHelper.InputMode.Gestures.ToString())
        {
            GestureEventManager.OnThumbsUp += HandleThumbGesture;
            StartCoroutine(EnableSwipeDetectorAfterDelay(1.1f));
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

        openDialogSound.Play();
    }

    private void OnDisable()
    {

        GestureEventManager.OnThumbsUp -= HandleThumbGesture;
        if (transform.GetComponentInParent<SwipeDetector>() != null)
        {
            transform.GetComponentInParent<SwipeDetector>().enabled = false;
        }
        this.GetComponentInParent<SwipeDetector>().enabled = false;
    }

    public void SetUp( V_CreateImage2 createImage)
    {
        createImageComponent = createImage;
        this.gameObject.SetActive(true);
        PositioningHelper.PositionInFrontOfUser(this.gameObject, 1.5f, 1.5f);
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
             custom_field: "close back during image creation dialog"
         );

        this.gameObject.SetActive(false);

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
             custom_field: "close back during image creation dialog"
         );

        createImageComponent.GoToPromptSettingsPhase();
       this.gameObject.SetActive(false);
    }

    private IEnumerator EnableSwipeDetectorAfterDelay(float delay)
    {
        var swipeDetector = this.GetComponentInParent<SwipeDetector>();
        if (swipeDetector != null)
        {
            swipeDetector.enabled = false;
            yield return new WaitForSeconds(delay);
            swipeDetector.enabled = true;
        }
    }
}
