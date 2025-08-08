using InputHelper;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;
using static SceneLoader;

public class WorkingAreaUiManager : MonoBehaviour
{
    [SerializeField] private GameObject ProjectDetailsCanvas;
    [SerializeField] private GameObject ImageArchiveCanvas;
    [SerializeField] private GameObject CreateImageCanva;
    [SerializeField] private GameObject ProjectHandMenu;
    [SerializeField] private GameObject ImageDetails;

    [SerializeField] private BoardsManager boardsManager;

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
            sourceScene: "Moodboarding",
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
        sourceScene: "Moodboarding",
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
        sourceScene: "Moodboarding",
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
        sourceScene: "Moodboarding",
        sourceComponent: this.name,
        custom_field: "LShapePalm undetected"
        );
    }

    #endregion

    private string selectedMoodboardId;

    private string inputType;

    private void OnEnable()
    {
        inputType = VM_AppData.Instance.GetInputMode();

        if (inputType == InputHelper.InputMode.Hybrid.ToString() || inputType == InputHelper.InputMode.Gestures.ToString())
        {
            GestureEventManager.OnLShapePalm += ChangeLShapePalm;
            GestureEventManager.OnLShapeBack += ChangeLShapeBack;
            GestureEventManager.OnLShapePalmEnd += ResetLShapePalm;
            GestureEventManager.OnLShapeBackEnd += ResetLShapeBack;
        }

        audioHelper = FindAnyObjectByType<AudioHelper>();

    }

    private void OnDisable()
    {
        GestureEventManager.OnLShapePalm -= ChangeLShapePalm;
        GestureEventManager.OnLShapeBack -= ChangeLShapeBack;
        GestureEventManager.OnLShapePalmEnd -= ResetLShapePalm;
        GestureEventManager.OnLShapeBackEnd -= ResetLShapeBack;
    }

    private void Update()
    {
        #region Debug Keyboard controls

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Creating image or moodboard");
            CreateImageOrMoodboard(TriggerSource.Button,GestureName.Null);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Saving data");
            SaveSession();

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseProject();
        }
        #endregion

        // Frame gesture detection
        if (LshapeBack && LshapePalm)
        {
            if (!gestureAlreadyTriggered) // only trigger once per gesture hold
            {
                Debug.Log("Frame gesture detected (single trigger)");
                CreateImageOrMoodboard(TriggerSource.Gesture, GestureName.Frame);

                if (audioHelper != null)
                {
                    audioHelper.PlayFrameGestureSound();
                }

                gestureAlreadyTriggered = true;
            }
        }
        else
        {
            // Reset trigger when user stops the gesture
            gestureAlreadyTriggered = false;
        }
    }

    public void CreateImageOrMoodboard (TriggerSource _source, GestureName _gesture)
    {
        if (VM_AppData.Instance.SelectedMoodboardId == string.Empty)
        {
            boardsManager.CreateMoodboard();

            InteractionLogger.Instance.LogInteraction(
            actionPerformed: ActionPerformed.CreateBoard.ToString(),
            usedInputMode: _source.ToString(), 
            gestureName: _gesture.ToString(),
            sourceScene: "Moodboarding",
            sourceComponent: this.name,
            custom_field: "board creation"
            );
        }
        else
        {
            InteractionLogger.Instance.LogInteraction(
            actionPerformed: ActionPerformed.CreateImage.ToString(),
            usedInputMode: _source.ToString(),
            gestureName: _gesture.ToString(),
            sourceScene: "Moodboarding",
            sourceComponent: this.name,
            custom_field: "image creation opened"
            );

            ShowCreateImage();
        }
    }

    public void CreateMoodboardByButton()
    {
        if (VM_AppData.Instance.SelectedMoodboardId == string.Empty)
        {
            InteractionLogger.Instance.LogInteraction(
            actionPerformed: ActionPerformed.CreateImage.ToString(),
            usedInputMode: TriggerSource.Button.ToString(),
            gestureName: GestureName.Null.ToString(),
            sourceScene: "Moodboarding",
            sourceComponent: this.name,
            custom_field: "board creation"
            );

            boardsManager.CreateMoodboard();
        }
        else
        {
            Debug.Log("Show alert, look somewhere else");
        }
    }


    public void ShowProjectDetails()
    {
        ProjectDetailsCanvas.SetActive(!ProjectDetailsCanvas.activeSelf);
    }

    public void ShowImageArchive()
    {
        ImageArchiveCanvas.SetActive(!ImageArchiveCanvas.activeSelf);
    }

    public void ShowCreateImage()
    {
        CreateImageCanva.SetActive(true);
        CreateImageCanva.GetComponent<V_CreateImage2>().DeactivateButtonCollidersOnGestureOnly();
    }

    public void HideCreateImage()
    {
        CreateImageCanva.SetActive(false);
    }

    public void SaveSession()
    {
        VM_AppData.Instance.SaveData();
    }

    public void ShowImageDetails(ImageData imageData, Transform imagePosition)
    {
        ImageDetails.SetActive(true);
        ImageDetails.GetComponent<V_ImageDetails>().SetUpImageDetails(imageData,imagePosition);
    }

    public bool IsImageDetailsActive()
    {
        return ImageDetails.activeSelf;
    }

    public void HideImageDetails()
    {
        ImageDetails.SetActive(false);
    }

    public void CloseProject()
    {
        SceneLoader.Instance.OpenMainMenuScene();
        VM_AppData.Instance.SetSelectedProject(string.Empty);
    }
}
