using UnityEngine;

public class V_ProjectHandMenu : MonoBehaviour
{
    [SerializeField] GameObject newBoardButton;

    private string inputType; 
    
    void Start()
    {
    inputType = VM_AppData.Instance.GetInputMode();

    if (inputType == InputHelper.InputMode.Gestures.ToString())
        {
            newBoardButton.SetActive(false);
        }
    }
}
