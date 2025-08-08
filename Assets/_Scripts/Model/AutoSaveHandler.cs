using UnityEngine;
using System.Collections;

public class AutoSaveHandler : MonoBehaviour
{
    private Coroutine saveRoutine;

    void Start()
    {
        saveRoutine = StartCoroutine(AutoSaveCoroutine());
    }

    IEnumerator AutoSaveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);
            Save();
        }
    }

    void OnDestroy()
    {
        Save();
        if (saveRoutine != null)
        {
            StopCoroutine(saveRoutine);
        }
    }

    private void Save()
    {
        if (VM_AppData.Instance != null)
        {
            Debug.Log("Auto-saving data...");
            VM_AppData.Instance.SaveData();
        }
    }
}