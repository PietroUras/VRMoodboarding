using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardsManager : MonoBehaviour
{
    [SerializeField] private GameObject MoodboardPrefab;

    [SerializeField] private V_CreateImage2 createImageComponent;
    [SerializeField] private AudioSource createBoardSound;
    [SerializeField] private AudioSource deleteBoardSound;

    private List<GameObject> moodboards = new List<GameObject>();

    private bool isNewMoodboard = false;

    private string selectedMoodboardId;

    [SerializeField] public bool isTutorial = false;


    void Start()
    {
        VM_AppData.Instance.SetBoardManager(this);
        if (!isTutorial)
        {
            Debug.Log("Loading moodboards from the project data.");
            List<MoodboardData> moodboards = VM_AppData.Instance.GetSelectedProject().Moodboards;

            if (moodboards.Count == 1)
            {
                isNewMoodboard = true; // set to true to position the moodboard in front of the user
                InstantiateMoodboard(moodboards[0]);
            }

            else // load all moodboards
            {
                for (int i = 0; i < moodboards.Count; i++)
                {
                    InstantiateMoodboard(moodboards[i]);
                }
            }
        }

    }

    public void DeleteMoodboard(string selectedMoodboardId)
    {
        if (selectedMoodboardId == string.Empty || selectedMoodboardId == null)
        {
            Debug.LogWarning("Selected moodboard ID is empty or null.");
            return;
        }

        GameObject moodboardToRemove = moodboards.Find(x => x.GetComponent<V_Moodboard>().GetMoodboardId() == selectedMoodboardId);
        if (moodboardToRemove != null)
        {
            moodboards.Remove(moodboardToRemove);
            Destroy(moodboardToRemove);
        }

        VM_AppData.Instance.DeleteMoodboard(selectedMoodboardId);
        deleteBoardSound.Play();
    }

    public void InstantiateMoodboard(MoodboardData moodboardData)
    {
        GameObject newMoodboard = Instantiate(MoodboardPrefab, transform);

        newMoodboard.GetComponent<V_Moodboard>().SetMoodboardData(moodboardData);

        newMoodboard.GetComponent<V_Moodboard>().CreateImageComponent = createImageComponent;

        if(! isTutorial)
        moodboards.Add(newMoodboard);

        var vmoodboard = newMoodboard.GetComponent<V_Moodboard>();
        Transform canvasTransform = vmoodboard.MoodboardTransform;

        if (!isNewMoodboard)
        {
            canvasTransform.position = moodboardData.Position;
            canvasTransform.rotation = moodboardData.Rotation;
        }
        else
        {
            PositioningHelper.PositionInFrontOfUser(canvasTransform.gameObject, 3f,1.5f);
            moodboardData.Position = canvasTransform.position;
            moodboardData.Rotation = canvasTransform.rotation;
            isNewMoodboard = false;
            createBoardSound.Play();
        }
    }

    public void CreateMoodboard()
    {
        if (IsMoodboardLimitReached() && ! isTutorial)
        {
            Debug.Log("Max moodboard limit reached (5).");
            return;
        }

        isNewMoodboard = true;

        MoodboardData newMoodboard = VM_AppData.Instance.AddMoodboard("New Moodboard");

        InstantiateMoodboard(newMoodboard);


    }

    public bool IsMoodboardLimitReached()
    {
        return VM_AppData.Instance.GetSelectedProject().Moodboards.Count > 4;
    }

    public List<GameObject> Moodboards
    {
        get => moodboards;
    }

    public List<RectTransform> GetAllMoodboardsTransform()
    {
        List<RectTransform> moodboardCenters = new List<RectTransform>();
        foreach (GameObject moodboard in moodboards)
        {
            moodboardCenters.Add(moodboard.GetComponent<V_Moodboard>().MoodboardTransform);
        }
        return moodboardCenters;

    }
}
