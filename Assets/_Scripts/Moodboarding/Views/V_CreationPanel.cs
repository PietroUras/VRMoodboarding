using MixedReality.Toolkit.UX;
using UnityEngine;

public class V_CreationPanel : MonoBehaviour
{
    [SerializeField] private PressableButton microphoneButton;
    [SerializeField] private PressableButton forwardButton;
    [SerializeField] private PressableButton backButton;
    private string inputType;
    private void OnEnable()
    {
        inputType = VM_AppData.Instance.GetInputMode();

        if (inputType == InputHelper.InputMode.Gestures.ToString())
        {
            microphoneButton.GetComponent<Collider>().enabled = false;
            forwardButton.GetComponent<Collider>().enabled = false;
            backButton.GetComponent<Collider>().enabled = false;
        }
    }
}
