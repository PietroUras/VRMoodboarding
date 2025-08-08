using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Hands.Samples.Gestures.DebugTools;

public class VM_AppData : MonoBehaviour
{
    public static VM_AppData Instance { get; private set; }
    public AppData CurrentData { get; private set; }

    private string loadingDataPath;
    private string savedDataPath;
    string contextualGestureFilePath;
    string gestureManagerFilePath;


    private string currentUser;
    private string inputMode;
    private string pythonPath;
    private string scriptPath;

    public string SelectedProjectId { get; private set; }

    public string SelectedMoodboardId { get; private set; }

    public string SelectedImageId { get; private set; }

    public event Action OnMoodboardUpdated;

    private ProjectData tempProject;

    private BoardsManager boardsManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            
            SelectedMoodboardId = string.Empty; // Initialize to empty string
            InitializePythonPaths();
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
    
    private void InitializePythonPaths()
    {
        string baseFolderPath = Path.Combine(Application.dataPath, "_Users");
        string filePath = Path.Combine(baseFolderPath, "python_path.txt");

        if (File.Exists(filePath))
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length >= 2)
                {
                    pythonPath = lines[0].Trim();
                    scriptPath = lines[1].Trim();
                    Debug.Log($"Python Path: {pythonPath}, Script Path: {scriptPath}");
                }
                else
                {
                    Debug.LogWarning("python_path.txt does not contain enough lines.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read python_path.txt: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("python_path.txt not found in _Users folder.");
        }
    }

    #region Data Management

    public void LoadData()
    {
        if (File.Exists(loadingDataPath))
        {
            try
            {
                string json = File.ReadAllText(loadingDataPath);
                CurrentData = JsonUtility.FromJson<AppData>(json);
                if (CurrentData == null)
                {
                    Debug.LogWarning("Loaded data was null, creating new AppData.");
                    CurrentData = new AppData();
                    SaveData(); 
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load data: " + e.Message);
                CurrentData = new AppData();
                SaveData();
            }
        }
        else
        {
            Debug.LogWarning("Data file not found, creating new AppData.");
            CurrentData = new AppData();
            SaveData();
        }
    }


    public void CreateUserFolder()
    {
        Debug.Log("Creating user folder for: " + currentUser);

        string baseFolderPath;
        baseFolderPath = Path.Combine(Application.dataPath, "_Users");

        string userFolderPath = Path.Combine(baseFolderPath, currentUser);
        Directory.CreateDirectory(userFolderPath);

        savedDataPath = Path.Combine(userFolderPath, $"Projects_Data_{currentUser}.json");
    }


    public void SaveData()
    {
        try
        {
            if (currentUser == null)
            {
                throw new System.Exception("currentUser is null.");
            }

            string json = JsonUtility.ToJson(CurrentData, true);
            Debug.Log("Saving data to: " + savedDataPath);
            File.WriteAllText(savedDataPath, json);
            Debug.Log("Data saved successfully." + savedDataPath);
        }
        catch (System.Exception e)
        {
            Debug.Log($"Save operation failed: {e.Message}");
        }
    }

    #endregion

    #region Project
    public void AddProject(string projectName, string projectBrief)
    {
        if (CurrentData.Projects.Count < 10)
        {
            CurrentData.AddProject(projectName, projectBrief);

            Debug.Log($"Project '{projectName}' added successfully. Total projects: {CurrentData.Projects.Count}");

            SaveData();

            SelectedProjectId = CurrentData.Projects.Last().Id;

        }
        else
        {
            Debug.LogWarning("Cannot add more projects, limit of 10 reached.");
        }
    }

    public void DeleteProject(string projectId)
    {
        CurrentData.DeleteProject(projectId);
        SaveData();
    }

    public void SetSelectedProject(string projectId)
    {
        SelectedProjectId = projectId;
    }

    public ProjectData GetSelectedProject()
    {
        if(boardsManager !=null && boardsManager.isTutorial)
        {
            tempProject = CurrentData.GetTempProject();
            //doing this to not ovverride anything during the tutorial
            Debug.Log("SelectedProjectId was null because i didn't start from menu");
            return tempProject;
        }

        ProjectData project = CurrentData.Projects.FirstOrDefault(p => p.Id == SelectedProjectId);
        if (project != null)
        {
            return project;
        }
        else
        {
            project = CurrentData.Projects.FirstOrDefault();
            return project;
        }
    }
    #endregion

    #region Moodboard & Image

    public MoodboardData AddMoodboard(string moodboardName)
    {
        GetSelectedProject().AddMoodboard(moodboardName);
        return GetSelectedProject().Moodboards.Last();
    }

    public void DeleteMoodboard(string moodboardId)
    {
        GetSelectedProject().DeleteMoodboard(moodboardId);
    }

    public void SetSelectedMoodboardId(string moodboardId)
    {
        SelectedMoodboardId = moodboardId;
    }

    public void AddImage( ImageData img, string _selectedMoodboardId = null)
    {
        if (SelectedProjectId == null)
        {
            Debug.Log("SelectedProjectId was null because i didn't start from menu");
            SelectedProjectId = CurrentData.Projects.FirstOrDefault().Id;
        }

        if (_selectedMoodboardId == null)
        {
            CurrentData.Projects.Find(p => p.Id == SelectedProjectId).Moodboards.Find(m => m.Id == SelectedMoodboardId).AddImage(img);
        }
        else
        {
            CurrentData.Projects.Find(p => p.Id == SelectedProjectId).Moodboards.Find(m => m.Id == _selectedMoodboardId).AddImage(img);
        }

    }

    public void DeleteImage(string imageId, string moodboardId)
    {
        Debug.Log("image id: " + imageId + " moodboardId: " + moodboardId);

        var project = CurrentData.Projects.Find(p => p.Id == SelectedProjectId);
        if (project == null)
        {
            project= CurrentData.Projects.FirstOrDefault();
        }

        var moodboard = project.Moodboards.Find(m => m.Id == moodboardId);
        if (moodboard == null)
        {
            Debug.LogError($"Moodboard with ID {moodboardId} not found in project {SelectedProjectId}.");
            return;
        }

        moodboard.DeleteImage(imageId);
    }

    public void SetSelectedImageId(string imageId)
    {
        SelectedImageId = imageId;
    }

    public ImageData GetSelectedImage()
    {

        ImageData image = CurrentData.Projects.FirstOrDefault(p => p.Id == SelectedProjectId).Moodboards.FirstOrDefault(m => m.Id == SelectedMoodboardId).Images.FirstOrDefault(i => i.Id == SelectedImageId);
        if (image != null)
        {
            return image;
        }
        else
        {
            Debug.Log("SelectedImageId was null");
            return null;
        }
    }

    public void MoveImageToAnotherBoard(string imageId, string startMoodboardId, string endMoodboardId)
    {
        MoodboardData startMoodboard = GetSelectedProject().Moodboards.FirstOrDefault(m => m.Id == startMoodboardId);
        if (startMoodboard == null)
        {
            Debug.LogError($"Start moodboard with ID {startMoodboardId} not found.");
            return;
        }


        // Find the image in the start moodboard  
        ImageData imageToMove = startMoodboard.Images.FirstOrDefault(i => i.Id == imageId);
        if (imageToMove == null)
        {
            Debug.Log($"Image with ID {imageId} not found in start moodboard: {startMoodboardId}.");
        }

        // Find the end moodboard  
        MoodboardData endMoodboard = GetSelectedProject().Moodboards.FirstOrDefault(m => m.Id == endMoodboardId);
        if (endMoodboard == null)
        {
            Debug.LogError($"End moodboard with ID {endMoodboardId} not found.");
            return;
        }

        // Remove the image from the start moodboard and add it to the end moodboard  
        startMoodboard.Images.Remove(imageToMove);
        endMoodboard.AddImage(imageToMove);

        Debug.Log($"Image with ID {imageId} moved from moodboard {startMoodboardId} to {endMoodboardId}.");
    }

    private void OnImageUpdated(object sender, PropertyChangedEventArgs e)
    {
        OnMoodboardUpdated?.Invoke(); // Trigger UI update when any property changes
    }


    #endregion

    public void SetBoardManager (BoardsManager boardsManager)
    {
        this.boardsManager = boardsManager;
    }

    public string GetCurrentUser()
    {
        return currentUser;
    }

    public void SetCurrentUser(int userNumber, bool _isNewUser)
    {
        currentUser = $"U{userNumber}";

        if (_isNewUser)
        {
            loadingDataPath = Path.Combine(Application.dataPath, "_Users/StartingPoint", "projects_data.json");
            CreateUserFolder();
        }
        else
        {
        loadingDataPath = Path.Combine(Application.dataPath, "_Users", currentUser, $"Projects_Data_{currentUser}.json");
        savedDataPath = loadingDataPath;
        }

        LoadData();
    }


    public void SetInputMode(string inputMode)
    {
        this.inputMode = inputMode;
        Debug.Log($"Input mode set to: {inputMode}");
    }

    public void CreateCSVFiles()
    {
        try
        {
            string reportFolderPath = Path.Combine(Application.dataPath, "_Users", currentUser,"Reports");

            // Create the Reports folder if it doesn't exist  
            if (!Directory.Exists(reportFolderPath))
            {
                Directory.CreateDirectory(reportFolderPath);
            }

            // Count existing CSV files in the Reports folder  
            int number = Directory.GetFiles(reportFolderPath, "*.csv").Length/2;

            // Generate file names for the new CSV files  
            string contextualGestureFileName = $"report_contextualGestures_{currentUser}_{inputMode}_test_{(number + 1)}.csv";
            string gestureManagerFileName = $"report_gestureManager_{currentUser}_{inputMode}_test_{(number + 1)}.csv";

            contextualGestureFilePath = Path.Combine(reportFolderPath, contextualGestureFileName);
            gestureManagerFilePath = Path.Combine(reportFolderPath, gestureManagerFileName);

            // Create and write to the contextual CSV file  
            using (StreamWriter writer = new(contextualGestureFilePath))
            {

            }

            // Create and write to the absolute CSV file  
            using (StreamWriter writer = new StreamWriter(gestureManagerFilePath))
            { 
            }

            Debug.Log($"CSV files created successfully: {contextualGestureFileName}, {gestureManagerFileName}");
            InteractionLogger.Instance.SetUp(contextualGestureFilePath, gestureManagerFilePath, currentUser, inputMode);
            GameObject.FindAnyObjectByType<XRHandFingerShapeRecorder>().SetUp(gestureManagerFilePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create CSV files: {e.Message}");
        }
    }

    public string GetInputMode()
    {
        return inputMode;
    }

    public string GetPythonPath()
    {
        return pythonPath;
    }

    public string GetScriptPath()
    {
        return scriptPath;
    }

    public string GetContextualGestureFilePath()
    {
        return contextualGestureFilePath;
    }
    public string GetGestureManagerFilePath()
    {
        return gestureManagerFilePath;
    }

    public string GetUserId()
    {
        return currentUser;
    }
}

