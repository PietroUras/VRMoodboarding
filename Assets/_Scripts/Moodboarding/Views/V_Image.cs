using MixedReality.Toolkit.UX;
using UnityEngine;
using UnityEngine.UI;


public class V_Image : MonoBehaviour
{
    private ImageData imageData;

    [SerializeField] private Image imageFront;
    [SerializeField] private Image imageBack;
    [SerializeField] private RectTransform rectTransform;

    private WorkingAreaUiManager uiManager;

    private bool isHoldingStatic = false;
    private bool hasMoved = false;
    private float timer = 1.2f;
    private Vector3 lastPosition;
    private float movementThreshold = 0.5f;
    private bool isShowingDetails = false;

    public void Initialize(ImageData data)
    {
        this.imageData = data;

        if (imageData != null)
        {
            LoadImage(imageData.Src);

            switch (imageData.Format)
            {
                case "Square":
                    //keep dimensions as default (500,500) 
                    break;
                case "Landscape":
                    SetImageDimensions(500, 375); 
                    break;
                case "Portrait":
                    SetImageDimensions(375, 500);
                    break;
                default:
                    Debug.LogError("Invalid format: " + imageData.Format);
                    break;
            }
        }

        uiManager = Object.FindFirstObjectByType<WorkingAreaUiManager>();
    }

    public void SetImageDimensions(int x, int y)
    {
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(x, y);
        }

        foreach (RectTransform child in rectTransform)
        {
            child.sizeDelta = new Vector2(x, y);
        }

        if (rectTransform.parent.parent.GetComponent<PressableButton>() != null)
        {
            Debug.Log("Changed parent transform");
            transform.parent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
        }
    }

    public void LoadImage(string imagePath)
    {
        Texture2D texture = CreateTexture(imagePath);
        if (texture != null && imageFront != null)
        {
            imageFront.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            if (imageBack != null)
            {
                imageBack.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
                );
            }
        }
    }

    private Texture2D CreateTexture(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            Debug.LogError("Invalid image path");
            return null;
        }

        byte[] fileData;
        try
        {
            fileData = System.IO.File.ReadAllBytes(imagePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to read image file: " + e.Message);
            return null;
        }

        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData))
        {
            return texture;
        }
        else
        {
            Debug.LogError("Failed to load texture");
            return null;
        }
    }

    public void UpdateImagePosition(UnityEngine.XR.Interaction.Toolkit.SelectExitEventArgs args)
    {
        // This method is called from the prefab and it's assigned by inspector
        // object manipulator > manipulation ended

        if (args.interactableObject.transform.gameObject != null)
        {
            GameObject manipulatedObject = args.interactableObject.transform.gameObject;

            manipulatedObject.GetComponentInParent<V_Image>().ImageData.Position = manipulatedObject.GetComponent<RectTransform>().anchoredPosition;
            manipulatedObject.GetComponentInParent<V_Image>().ImageData.Scale = manipulatedObject.GetComponent<RectTransform>().localScale;
        }

        // Set the image as the last sibling in the hierarchy to appear on top of other images that overlap
        transform.SetAsLastSibling();

    }

    public void SetImageSelected()
    {
        // This method is called from the prefab and it's assigned by inspector
        // object manipulator > manipulation started
        if (imageData != null)
        {
            Debug.Log("Image selected: " + imageData.Id);
            VM_AppData.Instance.SetSelectedImageId(imageData.Id);
        }
    }

    public void ShowImageDetails()
    {
        // This method is called from the prefab and it's assigned by inspector
        // object manipulator > manipulation started

        if (imageData != null)
        {
            isHoldingStatic = true;
            hasMoved = false;
            timer = 1.2f;
            lastPosition = rectTransform.position;
        }
    }
    private void Update()
    {
        if (!isHoldingStatic) return;

        float movement = Vector3.Distance(rectTransform.position, lastPosition);

        // If the image moves beyond threshold after holding started
        if (movement > movementThreshold && uiManager!=null)
        {
            // Cancel the interaction
            uiManager.HideImageDetails();
            hasMoved = true;
            lastPosition = rectTransform.position;
            isShowingDetails = false;
        }

        // If it has already moved, reset timer with higher value
        if (hasMoved)
        {
            timer = 2f;
            hasMoved = false;

        }
        else // Countdown before showing details
        {
            timer -= Time.deltaTime;
            if (timer <= 0f && !isShowingDetails && uiManager!=null)
            {
                uiManager.ShowImageDetails(imageData,this.transform);
                isShowingDetails = true;
            }
        }
    }

public void HideImageDetails()
    {
        // This method is called from the prefab and it's assigned by inspector
        // object manipulator > manipulation ended
        if (uiManager != null)
        {
            uiManager.HideImageDetails();
        }
        isHoldingStatic = false;
        hasMoved = false;
        timer = 1.2f;
        isShowingDetails = false;
    }

    public RectTransform RectTransform
    {
        get => rectTransform;
    }

    public ImageData ImageData
    {
        get => imageData;
    }
    public void Reset()
    {
        imageData = null;
        imageFront.sprite = null;
        if(imageBack != null)
            imageBack.sprite = null;

        if (transform.parent.parent.GetComponent<PressableButton>() != null)
        {
            transform.parent.parent.GetComponent<PressableButton>().enabled = false;
        }

    }

    public void FirstMoodboardTouch()
    {
        //Assigned by inspector
        gameObject.GetComponentInParent<V_Moodboard>().ChangeOldImagesOpacity(1f,1f,imageData.Id);
    }

    public string GetImageId()
    {
        return imageData.Id;
    }

}

