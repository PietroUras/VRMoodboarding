using InputHelper;
using MixedReality.Toolkit.UX;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechTutorial : MonoBehaviour
{
    [SerializeField] private PressableButton nextButton;
    [SerializeField] private PressableButton microphoneButton;
    [SerializeField] private TextMeshProUGUI vocalInput;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI subtitle;
    [SerializeField] private GameObject micArea;
    [SerializeField] private GameObject frameTutorial;
    [SerializeField] private GameObject image;
    [SerializeField] private bool isDebug = false;

    [SerializeField] AudioClip matchThePose;
    [SerializeField] AudioClip matchThePoseAgain;
    [SerializeField] AudioClip speak;
    [SerializeField] AudioClip pressNext;

    [SerializeField] AudioSource audioSource;


    private string projectTitle = string.Empty;
    private bool isListening = false;

    private int count = 0;

    private void Awake()
    {
        if (isDebug)
        {
            nextButton.OnClicked.AddListener(OnNextClicked);
            microphoneButton.OnClicked.AddListener(OnMicrophoneClicked);
        }
        micArea.SetActive(true);
        title.enabled = true;
        nextButton.gameObject.SetActive(false);
        nextButton.enabled = false;
        vocalInput.text = "Vocal input";
        subtitle.text = "Match the hand pose shown in the figure";
        microphoneButton.GetComponent<Collider>().enabled = false;

        PositioningHelper.PositionInFrontOfUser(this.gameObject, 1.5f, 1.5f);
    }

    private void OnEnable()
    {
        GestureEventManager.OnThumbsUp += OnNextClicked;
        GestureEventManager.OnStartMic += OnMicrophoneClicked;
    }

    private void OnDisable()
    {
        GestureEventManager.OnThumbsUp -= OnNextClicked;
        GestureEventManager.OnStartMic -= OnMicrophoneClicked;
    }

    private void Start()
    {
        StartCoroutine(PlayAudioAfterDelay(matchThePose, 1f));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneLoader.Instance.OpenMainMenuScene();
        }
    }

    private void OnMicrophoneClicked()
    {
        if (microphoneButton.enabled == true)
        {
            InteractionLogger.Instance.LogInteraction(
            actionPerformed: ActionPerformed.StartMic.ToString(),
            usedInputMode: TriggerSource.Gesture.ToString(),
            gestureName: GestureName.SpeechRec.ToString(),
            sourceScene: "Tutorial",
            sourceComponent: this.name,
            custom_field: "start recognition"
            );


            // Prevent multiple presses before processing previous input
            if (isListening || string.IsNullOrEmpty(vocalInput.text))
                return;

            isListening = true;
            subtitle.text = "Start speaking";
            StartCoroutine(PlayAudioAfterDelay(speak, 0.3f));
            vocalInput.text = "Listening...";

            // Start async speech recognition
            StartListeningForSpeech();
        }

    }

    private void ProcessSpeechResult(string result)
    {
        vocalInput.text = result;

        InteractionLogger.Instance.LogInteraction(
            actionPerformed: ActionPerformed.StartMic.ToString(),
            usedInputMode: TriggerSource.Gesture.ToString(),
            gestureName: GestureName.SpeechRec.ToString(),
            sourceScene: "Tutorial",
            sourceComponent: this.name,
            custom_field: result
            );

        if (string.IsNullOrEmpty(result))
        {
            vocalInput.text = "No input detected";
            isListening = false;
            return;
        }

        if (count>0)
        {
            title.enabled = false;
            micArea.SetActive(false);
            nextButton.gameObject.SetActive(true);
            nextButton.enabled = true;
            StartCoroutine(AnimateButtonPulse(nextButton.transform));
            image.SetActive(false);
            subtitle.text = "Press next button";
            StartCoroutine(PlayAudioAfterDelay(pressNext, 0.3f));
            isListening = false;
            return;
        }

        count++;

        if(result.Length > 20)
        {
            projectTitle = result.Substring(0, 20);
        }
        else
        {
            projectTitle = result;
        }

        subtitle.text = "Match the hand pose shown in the figure again";
        StartCoroutine(PlayAudioAfterDelay(matchThePoseAgain, 0.3f));
        isListening = false;
    }
    private void OnNextClicked()
    {
        if (nextButton.enabled == true)
        {
            InteractionLogger.Instance.LogInteraction(
            actionPerformed: ActionPerformed.Next.ToString(),
            usedInputMode: TriggerSource.Button.ToString(),
            gestureName: GestureName.Null.ToString(),
            sourceScene: "Tutorial",
            sourceComponent: this.name,
            custom_field: "moving to frame gesture tutorial"
            );
            frameTutorial.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    private async void StartListeningForSpeech()
    {
        // Wait for a speech result asynchronously
        var result = await SpeechToTextManager.Instance.TryGetRecognitionResultAsync();

        if (!string.IsNullOrEmpty(result))
        {
            ProcessSpeechResult(result);
        }
        else
        {
            vocalInput.text = "No input detected";
            subtitle.text = "Match the hand pose shown in the figure again";
            isListening = false;
        }
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

    private IEnumerator PlayAudioAfterDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.clip = clip;
        audioSource.Play();
    }



}
