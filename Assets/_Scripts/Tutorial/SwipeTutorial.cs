using InputHelper;
using MixedReality.Toolkit.UX;
using System.Collections;
using TMPro;
using UnityEngine;

public class SwipeTutorial : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private PressableButton backButton;
    [SerializeField] private PressableButton nextButton;

    [SerializeField] private AudioClip swipeRight;
    [SerializeField] private AudioClip swipeLeft;
    [SerializeField] private AudioClip swipeRightAgain;
    [SerializeField] private AudioClip thumbsUp;
    [SerializeField] private AudioClip thumbsUpAgain;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject swipeImage;
    [SerializeField] private GameObject thumbsUpImage;

    private int nextCounter;
    public float interval = 0.05f;

    private void Awake()
    {
        if (nextButton != null) nextButton.OnClicked.AddListener(OnNextClicked);
        if (backButton != null) backButton.OnClicked.AddListener(OnBackClicked);

        PositioningHelper.PositionInFrontOfUser(this.gameObject, 1.5f, 0.3f);
        nextCounter = 0;
    }

    private void OnEnable()
    {
        nextButton.enabled = true;
        backButton.enabled = false;
        tutorialText.text = "Swipe right to confirm";
        thumbsUpImage.SetActive(false);
        swipeImage.SetActive(true);
        nextCounter = 0;

        backButton.GetComponent<Collider>().enabled = false;
        nextButton.GetComponent<Collider>().enabled = false;
    }

    private void Start()
    {
        StartCoroutine(PlayAudioAfterDelay(swipeRight, 0.5f));
    }

    public void OnNextClicked()
    {
        
    switch (nextCounter)
            {
                case 0:
                    
                tutorialText.text = "Swipe left to go back";

                StartCoroutine(PlayAudioAfterDelay(swipeLeft, 0.3f));
                nextButton.enabled = false;
                backButton.enabled = true;
                backButton.GetComponent<Collider>().enabled = false;
                nextCounter++;

                InteractionLogger.Instance.LogInteraction(
                actionPerformed: ActionPerformed.Next.ToString(),
                usedInputMode: TriggerSource.Gesture.ToString(), 
                gestureName: GestureName.Swipe.ToString(), 
                sourceScene: "Tutorial",
                sourceComponent: this.name
                );

                break;

                case 1:
                    tutorialText.text = "You can also do a thumbs-up gesture to confirm";
                    GestureEventManager.OnThumbsUp += OnNextClicked;
                    swipeImage.SetActive(false);
                    thumbsUpImage.SetActive(true);
                    StartCoroutine(PlayAudioAfterDelay(thumbsUp, 0.3f));
                    nextCounter++;

                    InteractionLogger.Instance.LogInteraction(
                    actionPerformed: ActionPerformed.Next.ToString(),
                    usedInputMode: TriggerSource.Gesture.ToString(), 
                    gestureName: GestureName.Swipe.ToString(), 
                    sourceScene: "Tutorial",
                    sourceComponent: this.name

                    );
                break;

                case 2:
                    tutorialText.text = "Repeat the thumbs-up gesture to close the tutorial";
                    StartCoroutine(PlayAudioAfterDelay(thumbsUpAgain, 0.3f));
                    nextCounter++;

                    InteractionLogger.Instance.LogInteraction(
                    actionPerformed: ActionPerformed.Next.ToString(),
                    usedInputMode: TriggerSource.Gesture.ToString(), 
                    gestureName: GestureName.ThumbsUp.ToString(), 
                    sourceScene: "Tutorial",
                    sourceComponent: this.name

                    );

                break;

                case 3:
                    GestureEventManager.OnThumbsUp -= OnNextClicked;

                    InteractionLogger.Instance.LogInteraction(
                    actionPerformed: ActionPerformed.Next.ToString(),
                    usedInputMode: TriggerSource.Gesture.ToString(), 
                    gestureName: GestureName.ThumbsUp.ToString(),
                    sourceScene: "Tutorial",
                    sourceComponent: this.name
                    );

                   SceneLoader.Instance.OpenMainMenuScene();

                break;


                default:

                break;
            }


    }

    public void OnBackClicked()
    {
        tutorialText.text = "Swipe right again";
        StartCoroutine(PlayAudioAfterDelay(swipeRightAgain, 0.3f));
        backButton.enabled = false;
        nextButton.enabled = true;
        nextButton.GetComponent<Collider>().enabled = false;

        InteractionLogger.Instance.LogInteraction(
                    actionPerformed: ActionPerformed.Back.ToString(),
                    usedInputMode: TriggerSource.Gesture.ToString(), 
                    gestureName: GestureName.Swipe.ToString(), 
                    sourceScene: "Tutorial",
                    sourceComponent: this.name
                    );
    }

    private IEnumerator PlayAudioAfterDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.clip = clip;
        audioSource.Play();
    }
}
