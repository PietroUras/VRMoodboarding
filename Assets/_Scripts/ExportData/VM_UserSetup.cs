using System.IO;
using UnityEngine;

public class VM_UserSetup
{
    private int currentUserNumber = 0;

    public int GetCurrentUserNumber()
    {
        string usersPath;

        usersPath = Path.Combine(Application.dataPath, "_Users");

        currentUserNumber = Directory.GetDirectories(usersPath).Length;
        return currentUserNumber;
    }


    public void StartSession(int userNumber, string inputMode, bool isNewUser)
    {
        Debug.Log($"Starting session for User U{userNumber} with input mode: {inputMode}");
        GameObject.FindAnyObjectByType<StartupSceneLoader>().LoadMenuAndRoom();
        VM_AppData.Instance.SetCurrentUser(userNumber,isNewUser);
        VM_AppData.Instance.SetInputMode(inputMode);
        VM_AppData.Instance.CreateCSVFiles();
        SceneLoader.Instance.UnloadScene(SceneLoader.SceneName.TestSetup);
    }
}
