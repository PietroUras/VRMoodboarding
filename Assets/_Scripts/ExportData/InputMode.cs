
using MixedReality.Toolkit.UX;
using UnityEngine;

namespace InputHelper
{
    enum InputMode
    {
        Traditional,
        Gestures,
        Hybrid
    }
    public enum GestureName
    {
        Swipe,
        ThumbsUp,
        Frame,
        SpeechRec,
        Null
    }
    public enum ActionPerformed
    {
        StartMic,
        CreateImage,
        CreateBoard,
        Next,
        Back,
        Yes,
        No,
        Open //quick open project from project selection on main menu
    }
    public enum TriggerSource
    {
        Button,
        Gesture
    }

}

