using InputHelper;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Hands.Samples.Gestures.DebugTools;
using UnityEngine.XR.Hands.Samples.GestureSample;

public class XRHandGestureHandler : MonoBehaviour
{
    [Header("Thumbs")]
    [SerializeField] private StaticHandGesture rightHandThumbsUp;
    [SerializeField] private StaticHandGesture leftHandThumbsUp;

    [Header("Mic")]
    [SerializeField] private StaticHandGesture leftHandStartMic;
    [SerializeField] private StaticHandGesture rightHandStartMic;

    [Header("Frame")]
    [SerializeField] private StaticHandGesture rightHandLShapeFacePalm;
    [SerializeField] private StaticHandGesture rightHandLShapeFaceBack;
    [SerializeField] private StaticHandGesture leftHandLShapeFacePalm;
    [SerializeField] private StaticHandGesture leftHandLShapeFaceBack;

    [Header("Swipe")]
    [SerializeField] private StaticHandGesture rightHandOpenPalm;
    [SerializeField] private StaticHandGesture leftHandOpenPalm;

    private void Start()
    {
        #region Thumb  
        if (rightHandThumbsUp != null)
        {
            GestureEventManager.RegisterThumbsUpEvent(rightHandThumbsUp.gesturePerformed);
        }

        if (leftHandThumbsUp != null)
        {
            GestureEventManager.RegisterThumbsUpEvent(leftHandThumbsUp.gesturePerformed);
        }
        #endregion

        #region Mic  
        if (rightHandStartMic != null)
        {
            GestureEventManager.RegisterStartMicEvent(rightHandStartMic.gesturePerformed);
        }

        if (leftHandStartMic != null)
            GestureEventManager.RegisterStartMicEvent(leftHandStartMic.gesturePerformed);
        #endregion

        #region Frame Gesture  
        if (rightHandLShapeFacePalm != null)
        {
            GestureEventManager.RegisterLshapePalm(rightHandLShapeFacePalm.gesturePerformed, rightHandLShapeFacePalm.gestureEnded);
        }

        if (rightHandLShapeFaceBack != null)
        {
            GestureEventManager.RegisterLshapeBack(rightHandLShapeFaceBack.gesturePerformed, rightHandLShapeFaceBack.gestureEnded);
        }

        if (leftHandLShapeFacePalm != null)
        {
            GestureEventManager.RegisterLshapePalm(leftHandLShapeFacePalm.gesturePerformed, leftHandLShapeFacePalm.gestureEnded);
        }

        if (leftHandLShapeFaceBack != null)
        {
            GestureEventManager.RegisterLshapeBack(leftHandLShapeFaceBack.gesturePerformed, leftHandLShapeFaceBack.gestureEnded);
        }
        #endregion

        #region Swipe  
        if (rightHandOpenPalm != null)
        {
            GestureEventManager.RegisterSwipeRightHand(rightHandOpenPalm.gesturePerformed, rightHandOpenPalm.gestureEnded);
        }

        if (leftHandOpenPalm != null)
        {
            GestureEventManager.RegisterSwipeLeftHand(leftHandOpenPalm.gesturePerformed, leftHandOpenPalm.gestureEnded);
        }
        #endregion
    }

    private void OnDestroy()
    {
        #region Thumb
        if (rightHandThumbsUp != null)
            GestureEventManager.UnregisterThumbsUpEvent(rightHandThumbsUp.gesturePerformed);

        if (leftHandThumbsUp != null)
            GestureEventManager.UnregisterThumbsUpEvent(leftHandThumbsUp.gesturePerformed);
        #endregion

        #region Mic
        if (rightHandStartMic != null)
            GestureEventManager.UnregisterStartMicEvent(rightHandStartMic.gesturePerformed);

        if (leftHandStartMic != null)
            GestureEventManager.UnregisterStartMicEvent(leftHandStartMic.gesturePerformed);
        #endregion

        #region Frame Gesture

        if (rightHandLShapeFacePalm != null)
            GestureEventManager.RegisterLshapePalm(rightHandLShapeFacePalm.gesturePerformed, rightHandLShapeFacePalm.gestureEnded);

        if (rightHandLShapeFaceBack != null)
            GestureEventManager.RegisterLshapeBack(rightHandLShapeFaceBack.gesturePerformed, rightHandLShapeFaceBack.gestureEnded);

        if (leftHandLShapeFacePalm != null)
            GestureEventManager.RegisterLshapePalm(leftHandLShapeFacePalm.gesturePerformed, leftHandLShapeFacePalm.gestureEnded);

        if (leftHandLShapeFaceBack != null)
            GestureEventManager.RegisterLshapeBack(leftHandLShapeFaceBack.gesturePerformed, leftHandLShapeFaceBack.gestureEnded);

        #endregion

        #region Swipe

        if (rightHandOpenPalm != null)
            GestureEventManager.UnregisterSwipeRightHand(rightHandOpenPalm.gesturePerformed, rightHandOpenPalm.gestureEnded);

        if (leftHandOpenPalm != null)
            GestureEventManager.UnregisterSwipeLeftHand(leftHandOpenPalm.gesturePerformed, leftHandOpenPalm.gestureEnded);

        #endregion

    }
}
