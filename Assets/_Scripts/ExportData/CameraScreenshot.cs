using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class CameraScreenshot : MonoBehaviour
{
    public Camera photoCamera;
    [SerializeField] BoardsManager boardsManager;
    public int width = 1920;
    public int height = 1024;
    public string folderPath = "Screenshots";
    private int screenshotCounter;

    public void CaptureScreenshotsForMoodboards()
    {
        Debug.Log("Capturing screenshots for all moodboards.");

        string basePath;
        basePath = Path.Combine(Application.dataPath, "_Users");

        string fullPath = Path.Combine(basePath,
            VM_AppData.Instance.GetCurrentUser(),
            "Project",
            VM_AppData.Instance.SelectedProjectId);

        // Ensure directory exists
        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        // Get all moodboards
        List<RectTransform> moodboards = boardsManager.GetAllMoodboardsTransform();
        screenshotCounter = 1;

        foreach (RectTransform moodboard in moodboards)
        {
            if (moodboard.childCount > 0)
            {
                Transform firstChild = moodboard.GetChild(0);

                if (moodboard.gameObject.layer == LayerMask.NameToLayer("Board"))
                {
                    // Rotate camera to look at the moodboard  
                    Vector3 directionToMoodboard = moodboard.position - photoCamera.transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(directionToMoodboard, Vector3.up);
                    photoCamera.transform.rotation = targetRotation;

                    // Capture and save screenshot  
                    CaptureAndSaveImage(moodboard.name, fullPath, screenshotCounter);
                    screenshotCounter++;
                }
            }
        }
    }


    private void CaptureAndSaveImage(string moodboardName, string fullPath,int screenshotCounter)
    {
        Debug.Log($"Capturing screenshot for moodboard: {moodboardName}");

        // Setup RenderTexture  
        RenderTexture rt = new RenderTexture(width, height, 24);
        photoCamera.targetTexture = rt;

        // Render camera  
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;
        photoCamera.Render();

        // Read pixels  
        Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        image.Apply();

        // Restore state  
        photoCamera.targetTexture = null;
        RenderTexture.active = currentRT;
        Destroy(rt);

        // Encode to PNG  
        byte[] bytes = image.EncodeToPNG();
        Destroy(image);

        // Save to disk  
        string filename = Path.Combine(fullPath, moodboardName + "_screenshot"+ screenshotCounter+".png");
        File.WriteAllBytes(filename, bytes);

        Debug.Log($"Saved screenshot to: {filename}");

        VM_AppData.Instance.SaveData();
    }
}
