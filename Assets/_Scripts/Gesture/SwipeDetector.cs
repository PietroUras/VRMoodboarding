using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;
using System.Collections.Generic;

public class SwipeDetector : MonoBehaviour
{
    private float swipeSpeedThreshold = 0.02f;
    private float minSwipeDistance = 0.02f;

    public UnityEvent onSwipeRight;
    public UnityEvent onSwipeLeft;

    private XRHandSubsystem handSubsystem;

    private bool isLeftTracking = false;
    private bool isRightTracking = false;

    private Vector3 lastLeftPalmPos;
    private Vector3 lastRightPalmPos;
    private float lastLeftTime;
    private float lastRightTime;

    private AudioHelper audioHelper;

    private void Awake()
    {
        var subsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);
        if (subsystems.Count > 0)
            handSubsystem = subsystems[0];

        GestureEventManager.OnSwipeLeftHand += StartLeftHandSwipeTracking;
        GestureEventManager.OnSwipeRightHand += StartRightHandSwipeTracking;
        GestureEventManager.OnSwipeLeftHandEnd += StopLeftHandSwipeTracking;
        GestureEventManager.OnSwipeRightHandEnd += StopRightHandSwipeTracking;

        audioHelper = FindAnyObjectByType<AudioHelper>();
    }

    private void Update()
    {
        if (handSubsystem == null || !handSubsystem.running) return;

        if (isLeftTracking)
            CheckSwipe(handSubsystem.leftHand, ref lastLeftPalmPos, ref lastLeftTime);

        if (isRightTracking)
            CheckSwipe(handSubsystem.rightHand, ref lastRightPalmPos, ref lastRightTime);
    }

    private void CheckSwipe(XRHand hand, ref Vector3 lastPalmPos, ref float lastTime)
    {
        if (!TryGetPalmPose(hand, out Pose pose)) return;

        Vector3 currentPalmPos = pose.position;
        float deltaTime = Time.time - lastTime;

        if (deltaTime > 0.001f)
        {
            Vector3 velocity = (currentPalmPos - lastPalmPos) / deltaTime;
            float speedX = velocity.x;
            float distanceX = currentPalmPos.x - lastPalmPos.x;

            if (Mathf.Abs(distanceX) >= minSwipeDistance && Mathf.Abs(speedX) >= swipeSpeedThreshold)
            {
                if (speedX < 0)
                {
                    onSwipeRight?.Invoke();
                    if(audioHelper != null)
                        audioHelper.PlaySwipeSound();
                }
                else if (speedX > 0)
                {
                    onSwipeLeft?.Invoke();
                    if (audioHelper != null)
                        audioHelper.PlaySwipeSound();
                }


                if (hand == handSubsystem.leftHand)
                    isLeftTracking = false;
                else if (hand == handSubsystem.rightHand)
                    isRightTracking = false;
            }
        }

        lastPalmPos = currentPalmPos;
        lastTime = Time.time;
    }

    private bool TryGetPalmPose(XRHand hand, out Pose pose)
    {
        pose = default;

        // Ensure the hand's joint array is valid  
        if (!hand.isTracked)
            return false;

        var joint = hand.GetJoint(XRHandJointID.Palm);
        if (!joint.TryGetPose(out pose))
            return false;

        return true;
    }

    private void StartLeftHandSwipeTracking()
    {
        if (TryGetPalmPose(handSubsystem.leftHand, out Pose pose))
        {
            lastLeftPalmPos = pose.position;
            lastLeftTime = Time.time;
            isLeftTracking = true;
        }
    }

    private void StartRightHandSwipeTracking()
    {
        if (TryGetPalmPose(handSubsystem.rightHand, out Pose pose))
        {
            lastRightPalmPos = pose.position;
            lastRightTime = Time.time;
            isRightTracking = true;
        }
    }

    private void StopLeftHandSwipeTracking() => isLeftTracking = false;
    private void StopRightHandSwipeTracking() => isRightTracking = false;

    private void OnDestroy()
    {
        GestureEventManager.OnSwipeLeftHand -= StartLeftHandSwipeTracking;
        GestureEventManager.OnSwipeRightHand -= StartRightHandSwipeTracking;
        GestureEventManager.OnSwipeLeftHandEnd -= StopLeftHandSwipeTracking;
        GestureEventManager.OnSwipeRightHandEnd -= StopRightHandSwipeTracking;
    }
}
