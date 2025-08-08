using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.XR.Hands.Samples.Gestures.DebugTools;

public static class GestureEventManager
{
    #region ThumbsUp

    public static event Action OnThumbsUp;

    public static void RaiseThumbsUp()
    {
        OnThumbsUp?.Invoke();
    }

    public static void RegisterThumbsUpEvent(UnityEvent unityEvent)
    {
        unityEvent.AddListener(RaiseThumbsUp);
        unityEvent.AddListener(RecordFingerShapes);
    }

    public static void UnregisterThumbsUpEvent(UnityEvent unityEvent)
    {
        unityEvent.RemoveListener(RaiseThumbsUp);
        unityEvent.RemoveListener(RecordFingerShapes);
    }

    #endregion

    #region StartMic

    public static event Action OnStartMic;

    public static void RaiseStartMic()
    {
        OnStartMic?.Invoke();
    }

    public static void RegisterStartMicEvent(UnityEvent unityEvent)
    {
        unityEvent.AddListener(RaiseStartMic);
        unityEvent.AddListener(RecordFingerShapes);
    }

    public static void UnregisterStartMicEvent(UnityEvent unityEvent)
    {
        unityEvent.RemoveListener(RaiseStartMic);
        unityEvent.RemoveListener(RecordFingerShapes);
    }

    #endregion

    #region L Shape
    public static event Action OnLShapePalm;
    public static event Action OnLShapeBack;
    public static event Action OnLShapePalmEnd;
    public static event Action OnLShapeBackEnd;

    public static void RaiseLshapePalm() => OnLShapePalm?.Invoke();
    public static void RaiseLshapeBack() => OnLShapeBack?.Invoke();
    public static void RaiseLshapePalmEnd() => OnLShapePalmEnd?.Invoke();
    public static void RaiseLshapeBackEnd() => OnLShapeBackEnd?.Invoke();

    public static void RegisterLshapePalm(UnityEvent unityEvent, UnityEvent unityEventEnd)
    {
        unityEvent.AddListener(RaiseLshapePalm);
        unityEvent.AddListener(RecordFingerShapes);

        unityEventEnd.AddListener(RaiseLshapePalmEnd);
        unityEventEnd.AddListener(RecordFingerShapes);
    }

    public static void RegisterLshapeBack(UnityEvent unityEvent, UnityEvent unityEventEnd)
    {
        unityEvent.AddListener(RaiseLshapeBack);
        unityEvent.AddListener(RecordFingerShapes);

        unityEventEnd.AddListener(RaiseLshapeBackEnd);
        unityEventEnd.AddListener(RecordFingerShapes);
    }

    public static void UnregisterLshapePalm(UnityEvent unityEvent, UnityEvent unityEventEnd)
    {
        unityEvent.RemoveListener(RaiseLshapePalm);
        unityEvent.RemoveListener(RecordFingerShapes);

        unityEventEnd.RemoveListener(RaiseLshapePalmEnd);
        unityEventEnd.RemoveListener(RecordFingerShapes);
    }

    public static void UnregisterLshapeBack(UnityEvent unityEvent, UnityEvent unityEventEnd)
    {
        unityEvent.RemoveListener(RaiseLshapeBack);
        unityEvent.RemoveListener(RecordFingerShapes);

        unityEventEnd.RemoveListener(RaiseLshapeBackEnd);
        unityEventEnd.RemoveListener(RecordFingerShapes);
    }
    #endregion

    #region Swipe
    public static event Action OnSwipeRightHand;
    public static event Action OnSwipeLeftHand;
    public static event Action OnSwipeRightHandEnd;
    public static event Action OnSwipeLeftHandEnd;

    public static void RaiseSwipeRightHand() => OnSwipeRightHand?.Invoke();
    public static void RaiseSwipeLeftHand() => OnSwipeLeftHand?.Invoke();
    public static void RaiseSwipeRightHandEnd() => OnSwipeRightHandEnd?.Invoke();
    public static void RaiseSwipeLeftHandEnd() => OnSwipeLeftHandEnd?.Invoke();

    public static void RegisterSwipeRightHand(UnityEvent unityEvent, UnityEvent unityEventEnd)
    {
        unityEvent.AddListener(RaiseSwipeRightHand);
        unityEvent.AddListener(RecordFingerShapes);

        unityEventEnd.AddListener(RaiseSwipeRightHandEnd);
        unityEventEnd.AddListener(RecordFingerShapes);
    }

    public static void RegisterSwipeLeftHand(UnityEvent unityEvent, UnityEvent unityEventEnd)
    {
        unityEvent.AddListener(RaiseSwipeLeftHand);
        unityEvent.AddListener(RecordFingerShapes);

        unityEventEnd.AddListener(RaiseSwipeLeftHandEnd);
        unityEventEnd.AddListener(RecordFingerShapes);
    }

    public static void UnregisterSwipeRightHand(UnityEvent unityEvent, UnityEvent unityEventEnd)
    {
        unityEvent.RemoveListener(RaiseSwipeRightHand);
        unityEvent.RemoveListener(RecordFingerShapes);

        unityEventEnd.RemoveListener(RaiseSwipeRightHandEnd);
        unityEventEnd.RemoveListener(RecordFingerShapes);
    }

    public static void UnregisterSwipeLeftHand(UnityEvent unityEvent, UnityEvent unityEventEnd)
    {
        unityEvent.RemoveListener(RaiseSwipeLeftHand);
        unityEvent.RemoveListener(RecordFingerShapes);

        unityEventEnd.RemoveListener(RaiseSwipeLeftHandEnd);
        unityEventEnd.RemoveListener(RecordFingerShapes);
    }
    #endregion

    #region Recorder

    public static void RecordFingerShapes()
    {
        if (XRHandFingerShapeRecorder.Instance != null)
        {
            XRHandFingerShapeRecorder.Instance.RecordCurrentFingerShapes();
        }
        else
        {
            Debug.LogWarning("XRHandFingerShapeRecorder instance not found.");
        }
    }

    #endregion
}
