using InputHelper;
using MixedReality.Toolkit.UX;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FrameGestureTutorial : MonoBehaviour
{
    [SerializeField] private BoardsManager boardsManager;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private PressableButton nextButton;
    [SerializeField] private GameObject image;
    [SerializeField] private GameObject swipeTutorial;

    [SerializeField] AudioClip createBoard;
    [SerializeField] AudioClip createImage;
    [SerializeField] AudioClip pressNext;

    [SerializeField] AudioSource audioSource;


    private ImageData imageTutorial;
    private int boardCounter = 0;


    private AudioHelper audioHelper;


    #region L-shape Gesture Declaration

    private bool LshapeBack = false;
    private bool LshapePalm = false;

    private bool gestureAlreadyTriggered = false;

    private void ChangeLShapeBack()
    {
        LshapeBack = true;

        InteractionLogger.Instance.LogInteraction(
            actionPerformed: "ChangeLShapeBack",
            usedInputMode: TriggerSource.Gesture.ToString(),
            gestureName: GestureName.Frame.ToString(),
            sourceScene: "Tutorial",
            sourceComponent: this.name,
            custom_field: "LShapeBack detected"
        );
    }
    private void ChangeLShapePalm()
    {
        LshapePalm = true;

        InteractionLogger.Instance.LogInteraction(
        actionPerformed: "ChangeLShapeBack",
        usedInputMode: TriggerSource.Gesture.ToString(),
        gestureName: GestureName.Frame.ToString(),
        sourceScene: "Tutorial",
        sourceComponent: this.name,
        custom_field: "LShapePalm detected"
        );
    }
    private void ResetLShapeBack()
    {
        LshapeBack = false;

        InteractionLogger.Instance.LogInteraction(
        actionPerformed: "ResetLShapeBack",
        usedInputMode: TriggerSource.Gesture.ToString(),
        gestureName: GestureName.Frame.ToString(),
        sourceScene: "Tutorial",
        sourceComponent: this.name,
        custom_field: "LShapeBack undetected"
        );
    }
    private void ResetLShapePalm()
    {
        LshapePalm = false;

        InteractionLogger.Instance.LogInteraction(
        actionPerformed: "ResetLShapePalm",
        usedInputMode: TriggerSource.Gesture.ToString(),
        gestureName: GestureName.Frame.ToString(),
        sourceScene: "Tutorial",
        sourceComponent: this.name,
        custom_field: "LShapePalm undetected"       
        );
    }

    #endregion

    private void OnEnable()
    {
        GestureEventManager.OnLShapePalm += ChangeLShapePalm;
        GestureEventManager.OnLShapeBack += ChangeLShapeBack;
        GestureEventManager.OnLShapePalmEnd += ResetLShapePalm;
        GestureEventManager.OnLShapeBackEnd += ResetLShapeBack;

    }

    private void OnDisable()
    {
        GestureEventManager.OnLShapePalm -= ChangeLShapePalm;
        GestureEventManager.OnLShapeBack -= ChangeLShapeBack;
        GestureEventManager.OnLShapePalmEnd -= ResetLShapePalm;
        GestureEventManager.OnLShapeBackEnd -= ResetLShapeBack;
    }

    void Start()
    {
        PositioningHelper.PositionInFrontOfUser(this.gameObject, 1.5f, 0.3f);
        tutorialText.text = "Match the hand pose shown in the figure while looking at an empty area to create a moodboard";
        StartCoroutine(PlayAudioAfterDelay(createBoard, 0.5f));
        nextButton.OnClicked.AddListener(OnNextClicked);
        nextButton.enabled = false;
        nextButton.gameObject.SetActive(false);

        imageTutorial = new ImageData
        {
            Id = System.Guid.NewGuid().ToString("N").Substring(0, 8),
            Src = "E:\\Generative-AI-Powered-Moodboarding\\RandomImages\\random_image (3).png",
            UserPrompt = "Tutorial Random",
            Format = "Square",
            Style = "Photography",
            View = "In Front",
            Colors = "Normal Colors",
            Light = "Day",
            Mood = "Relaxing",
            Position = new Vector3(0f, 0f, 0.0f),
            Rotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
            Scale = new Vector3(1f, 1f, 1f)
        };

        audioHelper = FindAnyObjectByType<AudioHelper>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Creating image or moodboard");
            CreateImageOrMoodboard();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneLoader.Instance.OpenMainMenuScene();
        }

        // Frame gesture detection
        if (LshapeBack && LshapePalm)
        {
            if (!gestureAlreadyTriggered) // only trigger once per gesture hold
            {
                Debug.Log("Frame gesture detected (single trigger)");
                CreateImageOrMoodboard();
                gestureAlreadyTriggered = true;
            }
        }
        else
        {
            // Reset trigger when user stops the gesture
            gestureAlreadyTriggered = false;
        }
    }

    public void CreateImageOrMoodboard()
    {
        if (VM_AppData.Instance.SelectedMoodboardId == string.Empty && boardCounter==0)
        {
            InteractionLogger.Instance.LogInteraction(
            actionPerformed: ActionPerformed.CreateBoard.ToString(),
            usedInputMode: TriggerSource.Gesture.ToString(), //
            gestureName: GestureName.Frame.ToString(), //
            sourceScene: "Tutorial",
            sourceComponent: this.name,
            custom_field: "first board creation"
            );

            boardsManager.CreateMoodboard();
            boardCounter++;
            tutorialText.text = "Repeat the hand pose while looking at a moodboard to create an image";
            StartCoroutine(PlayAudioAfterDelay(createImage, 0.3f));
        }
        else if(VM_AppData.Instance.SelectedMoodboardId != string.Empty)
        {
            InteractionLogger.Instance.LogInteraction(
            actionPerformed: ActionPerformed.CreateImage.ToString(),
            usedInputMode: TriggerSource.Gesture.ToString(), //
            gestureName: GestureName.Frame.ToString(), //
            sourceScene: "Tutorial",
            sourceComponent: this.name,
            custom_field: "First image creation"
            );

            boardsManager.GetComponentInChildren<V_Moodboard>().InstantiateUIImage(imageTutorial);

            imageTutorial.Position = new Vector3(Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f), 0.0f);
            imageTutorial.Id = System.Guid.NewGuid().ToString("N").Substring(0, 8);
            //component update
            image.SetActive(false);
            image.GetComponentInParent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 400);
            tutorialText.text = "Press next button";
            StartCoroutine(PlayAudioAfterDelay(pressNext, 0.3f));
            nextButton.gameObject.SetActive(true);
            nextButton.enabled = true;
        }

        audioHelper.PlayFrameGestureSound();
    }

    void OnNextClicked()
    {
        Destroy(boardsManager.gameObject);

        InteractionLogger.Instance.LogInteraction(
            actionPerformed: ActionPerformed.Next.ToString(),
            usedInputMode: TriggerSource.Button.ToString(), //
            gestureName: GestureName.Null.ToString(), //
            sourceScene: "Tutorial",
            sourceComponent: this.name,
            custom_field: "moving to swipe gesture tutorial"
            );

        swipeTutorial.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private IEnumerator PlayAudioAfterDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.clip = clip;
        audioSource.Play();
    }
}
