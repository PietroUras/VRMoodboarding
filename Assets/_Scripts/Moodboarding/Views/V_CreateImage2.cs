using TMPro;
using UnityEngine;
using static SceneLoader;
using System.Collections;
using MixedReality.Toolkit.UX;
using InputHelper;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class V_CreateImage2 : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI subTitle;

    [Header("Voice Input")]
    [SerializeField] private GameObject voiceInputContainer;
    [SerializeField] private PressableButton microphoneButton;
    [SerializeField] private TextMeshProUGUI vocalInput;

    [Header("Navigation")]
    [SerializeField] private PressableButton forwardButton;
    [SerializeField] private TextMeshProUGUI forwardButtonText;
    [SerializeField] private PressableButton backButton;
    [SerializeField] private TextMeshProUGUI backButtonText;

    [Header("Prompt Settings")]
    [SerializeField] private GameObject promptingGeneralButtons;
    [SerializeField] private GameObject promptingSpecificButtons;
    [SerializeField] private V_Prompting promptingComponent;
    [SerializeField] private V_PromptingDetails promptingDetailsComponent;

    [Header("Image Selection")]
    [SerializeField] private GameObject imageContainer;
    [SerializeField] private V_Image[] images;

    [Header("Other Components")]
    [SerializeField] private BoardsManager boardsManager;
    [SerializeField] private WorkingAreaUiManager uiManager;

    [SerializeField] private GameObject backGenerationDialog;

    private VM_CreateImage createImageViewModel;
    private VM_Prompting promptingViewModel;

    private enum AppPhase { VoiceInput, PromptSettings, ImageSelection }
    private AppPhase currentPhase;

    private string userPrompt = string.Empty;
    private string oldPrompt = string.Empty;
    private bool isListening = false;

    private ImageData selectedImage;
    private GameObject selectedMoodBoard;
    private string selectedMoodBoardId;

    private string inputType;

    private void Awake()
    {
        microphoneButton.OnClicked.AddListener(HandleMicButton);
        forwardButton.OnClicked.AddListener(HandleNextButton);
        backButton.OnClicked.AddListener(HandleBackButton);

        uiManager = Object.FindAnyObjectByType<WorkingAreaUiManager>();
        createImageViewModel = new VM_CreateImage(promptingComponent);
        currentPhase = AppPhase.VoiceInput;
        vocalInput.text = "Image Subject";
    }

    private void Start()
    {
        SetTitleAndSubtitle("Image creation", "Step 1: Describe the subject");
        backButtonText.text = "Close";
        forwardButtonText.text = "Generate";
        forwardButton.enabled = false;

        if (inputType == InputHelper.InputMode.Gestures.ToString())
        {
            microphoneButton.GetComponent<Collider>().enabled = false;
            forwardButton.GetComponent<Collider>().enabled = false;
            backButton.GetComponent<Collider>().enabled = false;
        }
    }

    private void OnEnable()
    {
        InteractionLogger.Instance.LogInteraction(
             actionPerformed: "null",
             usedInputMode: "null",
             gestureName: "null",
             sourceScene: "Moodboarding",
             sourceComponent: this.name,
             custom_field: "open create image"
         );

        selectedMoodBoardId = VM_AppData.Instance.SelectedMoodboardId;
        foreach (var moodboard in boardsManager.Moodboards)
        {
            if (moodboard.TryGetComponent(out V_Moodboard moodboardComponent))
            {
                if (moodboardComponent.GetMoodboardId() == selectedMoodBoardId)
                {
                    selectedMoodBoard = moodboard;
                    break;
                }
            }
        }
        PositioningHelper.PositionParentInFrontOfUser(transform, 2f, 1.5f);

        //INPUT AREA

        inputType = VM_AppData.Instance.GetInputMode();

        if (inputType == InputHelper.InputMode.Hybrid.ToString() || inputType == InputHelper.InputMode.Gestures.ToString())
        {
            //swipe enabled by default
            GestureEventManager.OnThumbsUp += HandleThumbGesture;
            GestureEventManager.OnStartMic += HandleMicGesture;
        }
        if(inputType == InputHelper.InputMode.Traditional.ToString())
        {
           GetComponent<SwipeDetector>().enabled = false;
        }

        if (inputType == InputHelper.InputMode.Gestures.ToString())
        {
            microphoneButton.GetComponent<Collider>().enabled = false;
            forwardButton.GetComponent<Collider>().enabled = false;
            backButton.GetComponent<Collider>().enabled = false;
        }


    }

    private void OnDisable()
    {
        InteractionLogger.Instance.LogInteraction(
             actionPerformed: "null",
             usedInputMode: "null",
             gestureName: "null",
             sourceScene: "Moodboarding",
             sourceComponent: this.name,
             custom_field: "close create image"
         );

        GestureEventManager.OnThumbsUp -= HandleThumbGesture;
        GestureEventManager.OnStartMic -= HandleMicGesture;

        ResetComponent();
    }

    public void OnBackClicked(TriggerSource _trigger, GestureName _gesture)
    {
        if (backButton.enabled)
        {
            switch (currentPhase)
            {
                case AppPhase.VoiceInput:
                    InteractionLogger.Instance.LogInteraction(
                       actionPerformed: ActionPerformed.Back.ToString(),
                       usedInputMode: _trigger.ToString(),
                       gestureName: _gesture.ToString(),
                       sourceScene: "Moodboarding",
                       sourceComponent: this.name,
                       custom_field: "closing create image"
                       );
                    ResetComponent();
                    gameObject.SetActive(false);
                    break;

                case AppPhase.PromptSettings:
                    InteractionLogger.Instance.LogInteraction(
                       actionPerformed: ActionPerformed.Back.ToString(),
                       usedInputMode: _trigger.ToString(),
                       gestureName: _gesture.ToString(),
                       sourceScene: "Moodboarding",
                       sourceComponent: this.name,
                       custom_field: "back to voice input"
                       );
                    TransitionToPhase(AppPhase.VoiceInput);
                    break;

                case AppPhase.ImageSelection:
                    InteractionLogger.Instance.LogInteraction(
                       actionPerformed: ActionPerformed.Back.ToString(),
                       usedInputMode: _trigger.ToString(),
                       gestureName: _gesture.ToString(),
                       sourceScene: "Moodboarding",
                       sourceComponent: this.name,
                       custom_field: "back to prompt settings"
                       );
                    backGenerationDialog.GetComponent<BackImageCreation>().SetUp(this);
                    
                    break;
            }
        }
    }

    public void OnForwardClicked(TriggerSource _trigger, GestureName _gesture)
    {
        if (forwardButton.enabled)
        {
            switch (currentPhase)
            {
                case AppPhase.VoiceInput:
                    InteractionLogger.Instance.LogInteraction(
                    actionPerformed: ActionPerformed.Next.ToString(),
                    usedInputMode: _trigger.ToString(),
                    gestureName: _gesture.ToString(),
                    sourceScene: "Moodboarding",
                    sourceComponent: this.name,
                    custom_field: userPrompt
                    );
                    TransitionToPhase(AppPhase.PromptSettings);
                    break;

                case AppPhase.PromptSettings:
                    InteractionLogger.Instance.LogInteraction(
                       actionPerformed: ActionPerformed.Next.ToString(),
                       usedInputMode: _trigger.ToString(),
                       gestureName: _gesture.ToString(),
                       sourceScene: "Moodboarding",
                       sourceComponent: this.name,
                       custom_field: "closing prompt settings"
                       );
                    TransitionToPhase(AppPhase.ImageSelection);
                    break;

                case AppPhase.ImageSelection:
                    if (selectedImage == null)
                    {
                        Debug.LogWarning("No image selected.");
                        return;
                    }
                    InteractionLogger.Instance.LogInteraction(
                       actionPerformed: ActionPerformed.Next.ToString(),
                       usedInputMode: _trigger.ToString(),
                       gestureName: _gesture.ToString(),
                       sourceScene: "Moodboarding",
                       sourceComponent: this.name,
                       custom_field: "instantiating image"
                       );
                    VM_AppData.Instance.AddImage(selectedImage, selectedMoodBoardId);
                    selectedMoodBoard.GetComponent<V_Moodboard>().LoadNewImage();
                    ResetComponent();
                    gameObject.SetActive(false);
                    break;
            }
        }
    }

    private void TransitionToPhase(AppPhase nextPhase)
    {
        currentPhase = nextPhase;
        StopAllCoroutines(); // Stop any ongoing animations

        switch (nextPhase)
        {
            case AppPhase.VoiceInput:
                ResetComponent();
                break;

            case AppPhase.PromptSettings:
                promptingGeneralButtons.SetActive(true);
                promptingSpecificButtons.SetActive(true);
                voiceInputContainer.SetActive(false);
                imageContainer.SetActive(false);
                forwardButtonText.text = "Generate";
                backButtonText.text = "Back";
                EnableButton(forwardButton);

                StartCoroutine(AnimateTMPTextChange(subTitle, "Step 2: Customize image generation parameters"));
                break;

            case AppPhase.ImageSelection:
                _ = GenerateImagesAsync();
                promptingGeneralButtons.SetActive(false);
                promptingSpecificButtons.SetActive(false);
                promptingDetailsComponent.gameObject.SetActive(false);
                voiceInputContainer.SetActive(false);
                imageContainer.SetActive(true);

                forwardButtonText.text = "Instantiate";
                backButtonText.text = "Back";
                StartCoroutine(AnimateTMPTextChange(subTitle, "Step 3: Select the image to instantiate"));
                break;
        }
    }

    private async System.Threading.Tasks.Task GenerateImagesAsync()
    {
        promptingViewModel = promptingComponent.GetViewModel();
        forwardButton.enabled = false;

        for (int i = 0; i < 3 && i < images.Length; i++)
        {
           
            var data = await createImageViewModel.CreateImage(
                userPrompt,
                promptingViewModel.GetSelectedFormat(),
                promptingViewModel.GetSelectedStyle(),
                promptingViewModel.GetSelectedView(),
                promptingViewModel.GetSelectedColor(),
                promptingViewModel.GetSelectedLight(),
                promptingViewModel.GetSelectedMood());

            if (images[i].transform.parent.childCount > 1)
                images[i].transform.parent.GetChild(1).gameObject.SetActive(false); // hide loading  

            images[i].Initialize(data);
        }

        // Images are interactable after all of them are loaded correctly
        for (int i = 0; i < 3 && i < images.Length; i++)
        {
            images[i].gameObject.GetComponentInParent<PressableButton>().enabled = true;
        }
    }

    private void HandleMicGesture()
    {
        OnMicrophoneClicked(TriggerSource.Gesture, GestureName.SpeechRec);
    }

    private void HandleMicButton()
    {
        OnMicrophoneClicked(TriggerSource.Button, GestureName.Null);
    }

    private void HandleNextButton()
    {
        OnForwardClicked(TriggerSource.Button,GestureName.Null);
    }

    private void HandleBackButton()
    {
        OnBackClicked(TriggerSource.Button, GestureName.Null);
    }

    private void HandleThumbGesture()
    {
        OnForwardClicked(TriggerSource.Gesture, GestureName.ThumbsUp);
    }

    public void HandleSwipeYes()
    {
        //called by inspector from SwipeDetector
        OnForwardClicked(TriggerSource.Gesture, GestureName.Swipe);
    }

    public void HandleSwipeNo()
    {
        //called by inspector from SwipeDetector
        OnBackClicked(TriggerSource.Gesture, GestureName.Swipe);
    }


    private void OnMicrophoneClicked(TriggerSource _trigger, GestureName _gesture)
    {
        if (!microphoneButton.enabled || isListening || vocalInput.text == "Listening...")
            return;

        InteractionLogger.Instance.LogInteraction(
           actionPerformed: InputHelper.ActionPerformed.StartMic.ToString(),
           usedInputMode: _trigger.ToString(),
           gestureName: _gesture.ToString(),
           sourceScene: "Moodboarding",
           sourceComponent: this.name,
           custom_field: "start recognition"
       );

        isListening = true;
        vocalInput.text = "Listening...";
        StartListeningForSpeech(_trigger,_gesture);
    }

    private async void StartListeningForSpeech(TriggerSource _trigger, GestureName _gesture)
    {
        string result = await SpeechToTextManager.Instance.TryGetRecognitionResultAsync();

        if (!string.IsNullOrEmpty(result))
        {
            ProcessSpeechResult(result, _trigger, _gesture);
        }
        else
        {
            vocalInput.text = "Press the button again and try to speak louder!";
            isListening = false;
        }
    }

    private void ProcessSpeechResult(string result, TriggerSource _trigger, GestureName _gesture)
    {
        if (!string.IsNullOrEmpty(result))
        {
            InteractionLogger.Instance.LogInteraction(
           actionPerformed: InputHelper.ActionPerformed.StartMic.ToString(),
            usedInputMode: _trigger.ToString(),
           gestureName: _gesture.ToString(),
           sourceScene: "Moodboarding",
           sourceComponent: this.name,
           custom_field: result
       );

            vocalInput.text = result;

            if (userPrompt != result)
            {
                oldPrompt = userPrompt;
                userPrompt = result;

                EnableButton(forwardButton);
                StartCoroutine(AnimateButtonPulse(forwardButton.transform));
            }
        }
        else
        {
            vocalInput.text = "No input detected, press the button again and try to speak louder!";
        }

        isListening = false;
    }

    public void SetImageSelected(GameObject clickedImage)
    {
        selectedImage = clickedImage.GetComponent<V_Image>().ImageData;
        EnableButton(forwardButton);

        foreach (var image in images)
        {
            image.transform.localScale = image.ImageData == selectedImage ? Vector3.one * 1.2f : Vector3.one;
        }
    }

    private void SetTitleAndSubtitle(string titleText, string subtitleText)
    {
        title.text = titleText;
        subTitle.text = subtitleText;
    }

    private void ResetComponent()
    {
        userPrompt = string.Empty;
        oldPrompt = string.Empty;
        vocalInput.text = "Image Subject";
        forwardButton.enabled = false;
        selectedImage = null;

        foreach (var image in images)
        {
            image.transform.localScale = Vector3.one;
            image.transform.parent.GetChild(1).gameObject.SetActive(true); // show loading  
            image.Reset();
        }

        SetTitleAndSubtitle("Image creation", "Step 1: Describe the subject");
        backButtonText.text = "Close";
        forwardButtonText.text = "Generate";

        voiceInputContainer.SetActive(true);
        promptingGeneralButtons.SetActive(false);
        promptingSpecificButtons.SetActive(false);
        imageContainer.SetActive(false);
        promptingDetailsComponent.gameObject.SetActive(false);

        currentPhase = AppPhase.VoiceInput;

        if (inputType == InputHelper.InputMode.Gestures.ToString())
        {
            microphoneButton.GetComponent<Collider>().enabled = false;
            forwardButton.GetComponent<Collider>().enabled = false;
            backButton.GetComponent<Collider>().enabled = false;
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
    public void GoToPromptSettingsPhase()
    {
        foreach (var image in images)
        {
            image.transform.localScale = Vector3.one;
            image.transform.parent.GetChild(1).gameObject.SetActive(true); // show loading  
            image.Reset();
        }

        TransitionToPhase(AppPhase.PromptSettings);
    }

    public void DeactivateButtonCollidersOnGestureOnly()
    {
        if (inputType == InputHelper.InputMode.Gestures.ToString())
        {
            microphoneButton.GetComponent<Collider>().enabled = false;
            forwardButton.GetComponent<Collider>().enabled = false;
            backButton.GetComponent<Collider>().enabled = false;
        }
    }

    #region Animations

    private IEnumerator AnimateTMPTextChange(TextMeshProUGUI textElement, string newText)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Color originalColor = textElement.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            textElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        textElement.text = newText;
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

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            buttonTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            buttonTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
            yield return null;
        }

        buttonTransform.localScale = originalScale;
    }

    #endregion
}
