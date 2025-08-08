using InputHelper;
using Microsoft.MixedReality.GraphicsTools;
using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.UX;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class V_Moodboard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private GameObject imagePrefab;
    [SerializeField] private RectTransform moodboardCenter;
    [SerializeField] private RectTransform moodboardTransform;

    [SerializeField] private PressableButton titleButton;
    [SerializeField] private PressableButton addImage;


    private DirectionalLimiter directionalLimiter;

    private DeleteBoardDialog deleteBoardDialog;
    private GameObject deleteBoardCanva;

    public V_CreateImage2 CreateImageComponent { get; set; }

    private MoodboardData moodboardData;
    private BoardsManager boardsManager;
    private WorkingAreaUiManager uiManager;

    private string moodboardId;
    private bool isListening;

    private bool isNewImage = false;

    private string oldTitle = string.Empty;

    private string inputType;

    private void OnEnable()
    {
        boardsManager = Object.FindAnyObjectByType<BoardsManager>();

        uiManager = Object.FindAnyObjectByType<WorkingAreaUiManager>();
        if (uiManager == null)
        {
            Debug.LogWarning("WorkingAreaUiManager not found in the scene.");
        }

        isListening = false;

        deleteBoardDialog = Object.FindAnyObjectByType<DeleteBoardDialog>();
        if (deleteBoardDialog != null)
        {
            if (deleteBoardDialog.transform.childCount > 0)
            {
                deleteBoardCanva = deleteBoardDialog.transform.GetChild(0).gameObject;
            }
            else
            {
                Debug.LogWarning("DeleteBoardDialog exists but has no children.");
            }
        }
        else
        {
            Debug.LogWarning("DeleteBoardDialog not found in the scene.");
        }

        //Input area

        inputType = VM_AppData.Instance.GetInputMode();

        if (inputType == InputHelper.InputMode.Hybrid.ToString() || inputType == InputHelper.InputMode.Gestures.ToString())
        {
            GestureEventManager.OnStartMic += HandleMicGesture;
        }

        if(inputType == InputHelper.InputMode.Gestures.ToString())
        {
            addImage.gameObject.SetActive(false); 
            titleButton.gameObject.GetComponent<Collider>().enabled = false;
        }
    }

    public void HandleMicButton()
    {
        OnMicrophoneClicked(TriggerSource.Button, GestureName.Null);
    }

    private void HandleMicGesture()
    {
        OnMicrophoneClicked(TriggerSource.Gesture, GestureName.SpeechRec);
    }


    private void OnDisable()
    {
        GestureEventManager.OnStartMic -= HandleMicGesture;
    }

    public void SetMoodboardData(MoodboardData _moodboardData)
    {
        moodboardData = _moodboardData;
        moodboardId = _moodboardData.Id;
        title.text = moodboardData.Name;
        LoadAllImages();
        DirectionalLimiterSetUp();
    }

    private void DirectionalLimiterSetUp()
    {
        directionalLimiter = GetComponentInChildren<DirectionalLimiter>();

        directionalLimiter.SetUp(moodboardTransform, boardsManager);
    }

    private void LoadAllImages()
    {
        foreach (ImageData imageData in moodboardData.Images)
        {
            InstantiateUIImage(imageData);
        }
    }

    public void LoadNewImage()
    {
        // Check if there are any images in the list before accessing the last one
        if (moodboardData.Images.Any())
        {
            var lastImage = moodboardData.Images.Last();
            if (lastImage != null)
            {
                isNewImage = true;
                InstantiateUIImage(lastImage);

                ChangeOldImagesOpacity(117/255f,0.5f, lastImage.Id);
            }
        }
        else
        {
            Debug.LogWarning("No images available to load.");
        }
    }

    public void ChangeOldImagesOpacity(float color, float opacity,string lastImageId)
    {
        foreach (Transform child in moodboardCenter)
        {
            if(child == null)
            { 
                return;
            }
            else
            {
                V_Image vImageComponent = child.GetComponent<V_Image>();
                if (vImageComponent != null && vImageComponent.ImageData.Id != lastImageId)
                {
                    UnityEngine.UI.Image image = child.GetComponentInChildren<UnityEngine.UI.Image>();
                    if (image != null)
                    {
                        image.color = new Color(color, color, color, opacity);
                    }
                    else
                    {
                        Debug.Log("Image component not found in child.");
                    }
                }
            }
            
        }
    }

    public void DeleteSelectedImage()
    {
        if (VM_AppData.Instance.SelectedMoodboardId == moodboardData.Id)
        {
            var selectedImage = moodboardData.Images.Find(i => i.Id == VM_AppData.Instance.SelectedImageId);

            //delete image from this moodboard data and from app data
            if (selectedImage != null)
            {
                moodboardData.Images.Remove(selectedImage);
            }
            else
            {
                Debug.LogWarning("No image selected to delete.");
            }

            // destroy the game object

            foreach (Transform child in moodboardCenter)
            {
                V_Image vImageComponent = child.GetComponent<V_Image>();
                if (vImageComponent != null && vImageComponent.ImageData.Id == selectedImage.Id)
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
        }

    }

    public void InstantiateUIImage(ImageData imageData)
    {
        if (IsImageLimitReached())
        {
            Debug.LogWarning("Limit of 10 images reached");
            return;
        }

        GameObject newImage = Instantiate(imagePrefab, moodboardCenter);
        RectTransform rectTransform = newImage.GetComponent<V_Image>().RectTransform;

        if (!isNewImage) // set the position, rotation and scale from appdata
        {
            rectTransform.anchoredPosition = imageData.Position;
            rectTransform.localRotation = imageData.Rotation;
            rectTransform.localScale = imageData.Scale;
        }
        else //if creating a new image assign an available free position
        {
            rectTransform.anchoredPosition = GetAvailablePosition();
            imageData.Position = rectTransform.anchoredPosition;
            isNewImage = false;
        }

        V_Image vImageComponent = newImage.GetComponent<V_Image>();
        if (vImageComponent != null)
        {
            vImageComponent.Initialize(imageData);
        }
        else
        {
            Debug.LogError("V_Image component not found on prefab.");
        }
    }
    public bool IsImageLimitReached()
    {
        return moodboardData.Images.Count() > 10;
    }

    public Vector2 GetAvailablePosition()
    {
        const int maxRows = 3;
        const int maxCols = 4;
        const float imageSize = 500f;
        const float padding = 100f;

        float slotWidth = imageSize + padding;
        float slotHeight = imageSize + padding;

        float startX = -((maxCols - 1) * slotWidth) / 2;
        float startY = ((maxRows - 1) * slotHeight) / 2;

        // HashSet to track grid slots already used
        var occupiedSlots = new HashSet<Vector2Int>();

        // Go through instantiated images
        foreach (var img in moodboardData.Images)
        {
            // Map actual position back to grid slot
            int col = Mathf.RoundToInt((img.Position.x - startX) / slotWidth);
            int row = Mathf.RoundToInt((startY - img.Position.y) / slotHeight);

            occupiedSlots.Add(new Vector2Int(col, row));
        }

        // Check available slots
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                var gridSlot = new Vector2Int(col, row);
                if (!occupiedSlots.Contains(gridSlot))
                {
                    float x = startX + col * slotWidth;
                    float y = startY - row * slotHeight;
                    return new Vector2(x, y);
                }
            }
        }

        Debug.LogWarning("No available grid slots for new image.");
        return Vector2.zero;
    }



    public void UpdateMoodboardPosition(UnityEngine.XR.Interaction.Toolkit.SelectExitEventArgs args)
    {
        // This method is called from the prefab and it's assigned by inspector
        // object manipulator > manipulation ended

        var moodboard = args.interactableObject.transform.GetComponentInParent<V_Moodboard>();
        moodboard.moodboardData.Position = moodboard.moodboardTransform.position;
        moodboard.moodboardData.Rotation = moodboard.moodboardTransform.rotation;
    }

    public void SelectMoodboard()
    {
        // This method is called from the prefab and it's assigned by inspector
        // canva > object manipulator >mrtk events > is gaze hovered
        VM_AppData.Instance.SetSelectedMoodboardId(moodboardId);
        Debug.Log("Selected moodboard: " + moodboardData.Name);
    }
    public void UnselectMoodboard()
    {
        // This method is called from the prefab and it's assigned by inspector
        // canva > object manipulator >mrtk events > is gaze hovered
        VM_AppData.Instance.SetSelectedMoodboardId(string.Empty);
        Debug.Log("Unselected moodboard: " + moodboardData.Name);
    }

    public string GetMoodboardId()
    {
        return moodboardId;
    }
    public void DeleteSelectedMoodboard()
    {
        //assigned from inspector and called by button
        if(deleteBoardDialog!= null)
        {
            deleteBoardDialog.GetComponentInParent<DeleteBoardDialog>().enabled = true; // to check if works
            deleteBoardDialog.SetUp(moodboardId);
            deleteBoardCanva.SetActive(true);
            PositioningHelper.PositionInFrontOfUser(deleteBoardCanva, 2f, 1.5f);
        }

    }
    public void ShowCreateImage()
    {
        //assigned from inspector and called by button
        if (uiManager != null)
        {
            InteractionLogger.Instance.LogInteraction(
            actionPerformed: ActionPerformed.CreateImage.ToString(),
            usedInputMode: TriggerSource.Button.ToString(),
            gestureName: GestureName.Null.ToString(),
            sourceScene: "Moodboarding",
            sourceComponent: this.name,
            custom_field: "image creation opened"
            );

            uiManager.ShowCreateImage();
        }
    }


    public RectTransform MoodboardTransform
    {
        get => moodboardTransform;
    }

    public RectTransform MoodboardCenter
    {
        get => moodboardCenter;
    }

    public void OnMicrophoneClicked(TriggerSource _trigger, GestureName _gesture)
    {
        if (CreateImageComponent != null && !isListening && CreateImageComponent.isActiveAndEnabled == false)
        {
            InteractionLogger.Instance.LogInteraction(
            actionPerformed: InputHelper.ActionPerformed.StartMic.ToString(),
            usedInputMode: _trigger.ToString(), 
            gestureName: _gesture.ToString(),
            sourceScene: "Moodboarding",
            sourceComponent: this.name,
            custom_field: "start recognition moodboard rename");


            isListening = true;
            oldTitle = title.text;
            title.text = "Listening...";
            StartListeningForSpeech(_trigger,_gesture);
        }

    }
    private async void StartListeningForSpeech(TriggerSource _trigger, GestureName _gesture)
    {
        // Wait for a speech result asynchronously
        var result = await SpeechToTextManager.Instance.TryGetRecognitionResultAsync();

        InteractionLogger.Instance.LogInteraction(
            actionPerformed: InputHelper.ActionPerformed.StartMic.ToString(),
            usedInputMode: _trigger.ToString(),
            gestureName: _gesture.ToString(), 
            sourceScene: "Moodboarding",
            sourceComponent: this.name,
            custom_field: result);

        if (!string.IsNullOrEmpty(result))
        {
            if (result.Length > 15)
            {
                result = result.Substring(0, 15);
                Debug.LogWarning("Title too long, truncated to 15 characters.");
            }

            title.text = result;
            moodboardData.Name = result;
        }
        else
        {
            title.text = oldTitle;
            Debug.LogWarning("No speech result found.");
        }
        isListening = false;
    }

    public void PreventOverlap()
    {
        //called from the inspector on board handle in manipulation started
        directionalLimiter?.OnBeginManipulation();
    }

    public void StopPreventOverlap()
    {
        //called from the inspector on board handle in manipulation ended
        directionalLimiter?.OnEndManipulation();
    }

}