using UnityEngine;

public class DebugGesture : MonoBehaviour
{
    public void DebugPrint (string text)
    {
        Debug.Log("Detected Gesture" + text);
    }
}
