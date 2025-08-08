using MixedReality.Toolkit.UX;
using System.Collections;
using TMPro;
using UnityEngine;
using InputHelper;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class V_ProjectCreation : MonoBehaviour
{
    [SerializeField] private PressableButton backButton;
    [SerializeField] private PressableButton nextButton;
    [SerializeField] private PressableButton microphoneButton;
    [SerializeField] private TextMeshProUGUI vocalInput;
    [SerializeField] private TextMeshProUGUI subtitle;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip getTitle;
    [SerializeField] private AudioClip getBrief;

    private MainMenuUiManager uiManager;

    private string projectTitle = string.Empty;
    private string projectBrief = string.Empty;

    private bool isTitleSet = false;
    private bool isListening = false;

    private string inputType;

    private string trigger;
    private void Start()
    {
        backButton.OnClicked.AddListener(HandleBackButton);
        nextButton.OnClicked.AddListener(HandleNextButton);
        microphoneButton.OnClicked.AddListener(HandleMicButton);

        nextButton.enabled = false;

        if (inputType == InputHelper.InputMode.Gestures.ToString())
        {
            nextButton.GetComponent<Collider>().enabled = false;
            backButton.GetComponent<Collider>().enabled = false;
            microphoneButton.GetComponent<Collider>().enabled = false;
        }


        EnableButton(backButton);
        EnableButton(microphoneButton);
        vocalInput.text = "Project title";
        subtitle.text = "Step 1: Name Your Project";

        uiManager = Object.FindAnyObjectByType<MainMenuUiManager>();

        this.transform.position = uiManager.GetMainMenuTransform().position;
        this.transform.rotation = uiManager.GetMainMenuTransform().rotation;
    }
    
    private void OnEnable()
    {
        InteractionLogger.Instance.LogInteraction(
             actionPerformed: "null",
             usedInputMode: "null",
             gestureName: "null",
             sourceScene: "Main Menu",
             sourceComponent: this.name,
             custom_field: "open create project"
         );

        inputType = VM_AppData.Instance.GetInputMode();

        if (inputType == InputHelper.InputMode.Hybrid.ToString() || inputType == InputHelper.InputMode.Gestures.ToString())
        {
            GestureEventManager.OnStartMic += HandleMicGesture;
            GestureEventManager.OnThumbsUp += HandleThumbGesture;
            // swipe detector enabled by default
        }

        if (inputType == InputHelper.InputMode.Traditional.ToString())
        {
            GetComponent<SwipeDetector>().enabled = false;
        }

        if (inputType == InputHelper.InputMode.Gestures.ToString())
        {
            nextButton.GetComponent<Collider>().enabled = false;
            backButton.GetComponent<Collider>().enabled = false;
            microphoneButton.GetComponent<Collider>().enabled = false;
        }

        audioSource.PlayOneShot(getTitle);

    }

    private void OnDisable()
    {
        InteractionLogger.Instance.LogInteraction(
             actionPerformed: "null",
             usedInputMode: "null",
             gestureName: "null",
             sourceScene: "Main Menu",
             sourceComponent: this.name,
             custom_field: "close create project"
         );
        GestureEventManager.OnStartMic -= HandleMicGesture;
        GestureEventManager.OnThumbsUp -= HandleThumbGesture;
    }

    private void HandleMicButton()
    {
        OnMicrophoneClicked(TriggerSource.Button, GestureName.Null);
    }

    private void HandleNextButton()
    {
        OnNextClicked(TriggerSource.Button);
    }

    private void HandleBackButton()
    {
        OnBackClicked(TriggerSource.Button);
    }

    private void HandleMicGesture()
    {
        OnMicrophoneClicked(TriggerSource.Gesture, GestureName.SpeechRec);
    }

    private void HandleThumbGesture()
    {
        OnNextClicked(TriggerSource.Gesture, GestureName.ThumbsUp);
    }

    private void OnMicrophoneClicked(TriggerSource _trigger, GestureName _gesture)
    {
        if(microphoneButton.enabled == true)
        {  
            trigger = _trigger.ToString();

            InteractionLogger.Instance.LogInteraction(
            actionPerformed: InputHelper.ActionPerformed.StartMic.ToString(),
            usedInputMode: trigger, 
            gestureName: _gesture.ToString(), 
            sourceScene: "Main Menu",
            sourceComponent: this.name,
            custom_field: "start recognition"
        );

            // Prevent multiple presses before processing previous input
            if (isListening || string.IsNullOrEmpty(vocalInput.text)) 
                return;

            isListening = true;
            StartCoroutine(AnimateTMPTextChange(vocalInput, "Listening..."));

            // Start async speech recognition
            StartListeningForSpeech(_trigger,_gesture);
        }
        
    }

    private async void StartListeningForSpeech(TriggerSource _trigger, GestureName _gesture)
    {
        // Wait for a speech result asynchronously
        var result = await SpeechToTextManager.Instance.TryGetRecognitionResultAsync();

        if (!string.IsNullOrEmpty(result))
        {
            ProcessSpeechResult(result, _trigger, _gesture);
        }
        else
        {
            vocalInput.text = "No input detected, press the button again and try to speak louder!";
            isListening = false;
        }
    }
    private void ProcessSpeechResult(string result, TriggerSource _trigger, GestureName _gesture)
    {
        InteractionLogger.Instance.LogInteraction(
        actionPerformed: InputHelper.ActionPerformed.StartMic.ToString(),
        usedInputMode: _trigger.ToString(), 
        gestureName: _gesture.ToString(),
        sourceScene: "Main Menu",
        sourceComponent: this.name,
        custom_field: result
        );

        vocalInput.text = result;

        if (!isTitleSet)
        {
            if (result.Length > 20)
            {
                vocalInput.text = "Project title too long, please try again!";
                isListening = false;
                return;
            }
            projectTitle = result;
        }
        else
        {
            projectBrief = result;
        }

        if (!string.IsNullOrEmpty(result))
        {
            EnableButton(nextButton);
        }
        else
        {
            vocalInput.text = "No input detected, press the button again and try to speak louder!";
        }

        isListening = false;
    }

    public void OnNextClickedSwipe()
    {
        if (nextButton.enabled == true)
        {
            OnNextClicked(TriggerSource.Gesture, GestureName.Swipe);
        }
    }

    public void OnBackClickedSwipe()
    {
        if (backButton.enabled == true)
        {
            OnBackClicked(TriggerSource.Gesture);
        }
    }

    public void OnNextClicked(TriggerSource _trigger, GestureName gesture = GestureName.Null)
    {
        if (nextButton.enabled==true) {

            trigger = _trigger.ToString();

            InteractionLogger.Instance.LogInteraction(
            actionPerformed: ActionPerformed.Next.ToString(),
            usedInputMode: trigger,
            gestureName: gesture.ToString(),
            sourceScene: "Main Menu",
            sourceComponent: this.name
            );

            if (!isTitleSet)
            {
                StartCoroutine(AnimateTMPTextChange(subtitle, "Step 2: Describe Your Project Briefly"));
                StartCoroutine(AnimateTMPTextChange(vocalInput, "Project brief"));
                audioSource.PlayOneShot(getBrief);
                isTitleSet = true;
                StartCoroutine(AnimateButtonPulse(microphoneButton.transform));
                nextButton.enabled = false;
            }
            else
            {
                Debug.Log("Project saved");

                VM_AppData.Instance.AddProject(projectTitle, projectBrief);

                ResetComponent();

                uiManager.OpenWorkScene();
            }
        }  
    }

    public void OnBackClicked(TriggerSource _trigger)
    {
        trigger = _trigger.ToString();

        InteractionLogger.Instance.LogInteraction(
            actionPerformed: ActionPerformed.Back.ToString(),
            usedInputMode: trigger,
            gestureName: InputHelper.GestureName.Swipe.ToString(), 
            sourceScene: "Main Menu",
            sourceComponent: this.name
            );


        if (isTitleSet)
        {
            // Go back to Step 1
            isTitleSet = false;
            projectBrief = string.Empty;
            subtitle.text = "Step 1: Name Your Project";
            audioSource.PlayOneShot(getTitle);
            vocalInput.text = projectTitle;
            EnableButton(nextButton);
            StartCoroutine(AnimateButtonPulse(microphoneButton.transform));
        }
        else
        {
            // We're in Step 1, so exit to main menu
            ResetComponent();
            uiManager.ShowMainMenu();
        }
    }

    private void ResetComponent()
    {
        projectTitle = string.Empty;
        projectBrief = string.Empty;
        isTitleSet = false;
        vocalInput.text = "Project title";
        nextButton.enabled = false;
        subtitle.text = "Step 1: Name Your Project";

        if (inputType == InputHelper.InputMode.Gestures.ToString())
        {
            nextButton.GetComponent<Collider>().enabled = false;
            backButton.GetComponent<Collider>().enabled = false;
            microphoneButton.GetComponent<Collider>().enabled = false;
        }

    }

    private void EnableButton(PressableButton button)
    {
        button.enabled = true;

        if (inputType == InputHelper.InputMode.Gestures.ToString())
        {
            Collider col = button.GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }

    private IEnumerator AnimateTMPTextChange(TextMeshProUGUI textElement, string newText)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Color originalColor = textElement.color;

        // Fade out
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            textElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        textElement.text = newText;

        // Fade in
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            textElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        textElement.color = originalColor;
    }

    private IEnumerator AnimateButtonPulse(Transform buttonTransform)
    {
        Vector3 originalScale = buttonTransform.localScale;
        Vector3 targetScale = originalScale * 1.1f;
        float duration = 0.2f;
        float elapsed = 0f;

        // Scale up
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            buttonTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            yield return null;
        }

        // Scale back down
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            buttonTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
            yield return null;
        }

        buttonTransform.localScale = originalScale;
    }

    public void DeactivateButtonColliders()
    {
        if (inputType == InputHelper.InputMode.Gestures.ToString())
        {
            nextButton.GetComponent<Collider>().enabled = false;
            backButton.GetComponent<Collider>().enabled = false;
            microphoneButton.GetComponent<Collider>().enabled = false;
        }
    }



}
