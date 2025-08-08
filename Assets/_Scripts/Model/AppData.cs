using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Serializable]
public class AppData : INotifyPropertyChanged
{
    [SerializeField]
    private List<ProjectData> projects = new List<ProjectData>();
    public event PropertyChangedEventHandler PropertyChanged;

    private ProjectData tempProject;

    public List<ProjectData> Projects
    {
        get => projects;
        set
        {
            if (projects != value)
            {
                projects = value;
                OnPropertyChanged(nameof(Projects));
            }
        }
    }

    public ProjectData GetTempProject()
    {
        if(tempProject == null)
        {
            tempProject = new ProjectData
            {
                Name = "Temp Project",
                Brief = "This is a temporary project.",
                Id = System.Guid.NewGuid().ToString("N").Substring(0, 8),
                Moodboards = new List<MoodboardData>()
            };
        }
        return tempProject;
    }

    public void AddProject(string projectName,string projectBrief)
    {

        if (projects.Count <8)
        {
            var newProject = new ProjectData
            {
                Name = projectName,
                Brief = projectBrief,
                Id = System.Guid.NewGuid().ToString("N").Substring(0, 8),
                Moodboards = new List<MoodboardData>()
            };

            // Create the default moodboard
            newProject.AddMoodboard("Moodboard");

            projects.Add(newProject);
            OnPropertyChanged(nameof(Projects));
        }
        else
        {
            Debug.LogWarning("Max project limit reached (8).");
        }
    }

    public void DeleteProject(string projectId)
    {
        projects.RemoveAll(p => p.Id == projectId);
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}

[Serializable]
public class ProjectData : INotifyPropertyChanged
{
    [SerializeField]
    private string id;
    [SerializeField]
    private string name;
    [SerializeField]
    private string brief;
    [SerializeField]
    private List<MoodboardData> moodboards = new List<MoodboardData>();

    public event PropertyChangedEventHandler PropertyChanged;

    public string Name
    {
        get => name;
        set
        {
            if (name != value)
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public string Id
    {
        get => id;
        set
        {
            if (id != value)
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
    }

    public string Brief
    {
        get => brief;
        set
        {
            if (brief != value)
            {
                brief = value;
                OnPropertyChanged(nameof(Brief));
            }
        }
    }

    public List<MoodboardData> Moodboards
    {
        get => moodboards;
        set
        {
            if (moodboards != value)
            {
                moodboards = value;
                OnPropertyChanged(nameof(Moodboards));
            }
        }
    }

    public void AddMoodboard(string boardName)
    {
        moodboards.Add(new MoodboardData { Name = boardName, Images = new List<ImageData>(), Id= System.Guid.NewGuid().ToString("N").Substring(0, 8) });
        OnPropertyChanged(nameof(Moodboards));
    }

    public void DeleteMoodboard(string moodboardId)
    {
        Moodboards.RemoveAll(m => m.Id == moodboardId);
        OnPropertyChanged(nameof(Moodboards));
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

[Serializable]
public class MoodboardData : INotifyPropertyChanged
{

    [SerializeField]
    private string id;

    [SerializeField]
    private string name;

    [SerializeField]
    private Vector3 position = new Vector3(0.0f, 0.0f, 0.0f); //default position to spawn in front of the user

    [SerializeField]
    private Quaternion rotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

    [SerializeField]
    private List<ImageData> images = new List<ImageData>();

    public event PropertyChangedEventHandler PropertyChanged;

    public string Name
    {
        get => name;
        set
        {
            if (name != value)
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public string Id
    {
        get => id;
        set
        {
            if (id != value)
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
    }

    public Vector3 Position
    {
        get => position;
        set
        {
            if (position != value)
            {
                position = value;
                OnPropertyChanged(nameof(Position));
            }
        }
    }

    public Quaternion Rotation
    {
        get => rotation;
        set
        {
            if (rotation != value)
            {
                rotation = value;
                OnPropertyChanged(nameof(Rotation));
            }
        }
    }

    public List<ImageData> Images
    {
        get => images;
        set
        {
            if (images != value)
            {
                images = value;
                OnPropertyChanged(nameof(Images));
            }
        }
    }

    public void AddImage (ImageData img)
    {
        if (images.Count < 10)
        {
            img.Id = System.Guid.NewGuid().ToString("N").Substring(0, 8);
            images.Add(img);
            OnPropertyChanged(nameof(Images));
        }
        else
        {
            Debug.LogWarning("Max image limit per moodboard reached (10).");
        }
    }

    public void DeleteImage(string _imgId)
    {
        images.RemoveAll(img => img.Id == _imgId);
        OnPropertyChanged(nameof(Images));
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

[Serializable]
public class ImageData : INotifyPropertyChanged
{
    [SerializeField]
    private string id;

    [SerializeField]
    private string src;

    [SerializeField]
    private string userPrompt;

    [SerializeField]
    private string format;

    [SerializeField]
    private string style;

    [SerializeField]
    private string view;

    [SerializeField]
    private string colors;

    [SerializeField]
    private string light;

    [SerializeField]
    private string mood;

    [SerializeField]
    private Vector3 position = new Vector3(100.0f, 100.0f, 0.0f);

    [SerializeField]
    private Quaternion rotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

    [SerializeField]
    private Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);

    public event PropertyChangedEventHandler PropertyChanged;

    public string Id
    {
        get => id;
        set
        {
            if (id != value)
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
    }

    public string Src
    {
        get => src;
        set
        {
            if (src != value)
            {
                src = value;
                OnPropertyChanged(nameof(Src));
            }
        }
    }

    public string UserPrompt
    {
        get => userPrompt;
        set
        {
            if (userPrompt != value)
            {
                userPrompt = value;
                OnPropertyChanged(nameof(UserPrompt));
            }
        }
    }

    public string Format
    {
        get => format;
        set
        {
            if (format != value)
            {
                format = value;
                OnPropertyChanged(nameof(Format));
            }
        }
    }

    public string Style
    {
        get => style;
        set
        {
            if (style != value)
            {
                style = value;
                OnPropertyChanged(nameof(Style));
            }
        }
    }

    public string View
    {
        get => view;
        set
        {
            if (view != value)
            {
                view = value;
                OnPropertyChanged(nameof(View));
            }
        }
    }

    public string Colors
    {
        get => colors;
        set
        {
            if (colors != value)
            {
                colors = value;
                OnPropertyChanged(nameof(Colors));
            }
        }
    }

    public string Light
    {
        get => light;
        set
        {
            if (light != value)
            {
                light = value;
                OnPropertyChanged(nameof(Light));
            }
        }
    }

    public string Mood
    {
        get => mood;
        set
        {
            if (mood != value)
            {
                mood = value;
                OnPropertyChanged(nameof(Mood));
            }
        }
    }

    public Vector3 Position
    {
        get => position;
        set
        {
            if (position != value)
            {
                position = value;
                OnPropertyChanged(nameof(Position));
            }
        }
    }

    public Quaternion Rotation
    {
        get => rotation;
        set
        {
            if (rotation != value)
            {
                rotation = value;
                OnPropertyChanged(nameof(Rotation));
            }
        }
    }

    public Vector3 Scale
    {
        get => scale;
        set
        {
            if (scale != value)
            {
                scale = value;
                OnPropertyChanged(nameof(Scale));
            }
        }
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
