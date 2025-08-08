using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using MixedReality.Toolkit.SpatialManipulation;

public class HandPullDetectorXR : MonoBehaviour
{
    [Header("Pull Detection Settings")]
    [SerializeField] private Vector3 _checkBoxHalfExtents = new Vector3(0.4f, 0.4f, 0.8f);

    [Header("Layer Masks")]
    [SerializeField] private LayerMask _boardLayerMask;
    [SerializeField] private LayerMask _deleteLayerMask;

    [Header("Sounds")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip detachImage;
    [SerializeField] private AudioClip dropImageVoid;
    [SerializeField] private AudioClip dropImageOtherBoard;

    private XRHandSubsystem _handSubsystem;
    private XRNode _lastHandMoved = XRNode.RightHand;

    private Vector3 _startPalmPos;
    private bool _isTracking = false;

    private Vector3 _startPosition;
    private Quaternion _startRotation;

    private string _currentMoodboardId;
    private string _currentImageId;
    private Transform _imagePrefab;

    private GameObject deleteArea;
    private GameObject deleteDialog;

    [SerializeField] private float pullDistanceZTreshold = 0.13f;

    private void Start()
    {
        _currentImageId = GetComponentInParent<V_Image>().GetImageId();

        deleteArea = FindAnyObjectByType<V_DeleteImageArea>().transform.GetChild(0).gameObject;
        deleteDialog = FindAnyObjectByType<V_DeleteImageArea>().transform.GetChild(1).gameObject;

        var subsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);

        if (subsystems.Count > 0)
            _handSubsystem = subsystems[0];
    }

    private void OnEnable()
    {
        _currentMoodboardId = GetComponentInParent<V_Moodboard>().GetMoodboardId();
        Debug.Log("Current Moodboard ID: " + _currentMoodboardId);

        if (_handSubsystem == null || !_handSubsystem.running) return;

        InitializeHandTracking();
    }

    private void Update()
    {
        if (_handSubsystem == null || !_handSubsystem.running) return;

        UpdateHandTracking();

        HandleDebugInput();
    }

    private void HandleDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            //_isPulledBack = true;
            Debug.Log("Pull Detected");
            OnPullDetected();
        }
    }

    private void InitializeHandTracking()
    {
        XRHand leftHand = _handSubsystem.leftHand;
        XRHand rightHand = _handSubsystem.rightHand;

        if (TryGetPalmPose(leftHand, out Pose leftPalmPose) && TryGetPalmPose(rightHand, out Pose rightPalmPose))
        {
            float leftDistance = Vector3.Distance(leftPalmPose.position, _startPalmPos);
            float rightDistance = Vector3.Distance(rightPalmPose.position, _startPalmPos);

            _lastHandMoved = leftDistance > rightDistance ? XRNode.LeftHand : XRNode.RightHand;
        }
        else if (TryGetPalmPose(leftHand, out _))
        {
            _lastHandMoved = XRNode.LeftHand;
        }
        else if (TryGetPalmPose(rightHand, out _))
        {
            _lastHandMoved = XRNode.RightHand;
        }

        XRHand hand = _lastHandMoved == XRNode.LeftHand ? _handSubsystem.leftHand : _handSubsystem.rightHand;
        if (TryGetPalmPose(hand, out Pose palmPose))
        {
            _startPalmPos = palmPose.position;
        }
    }

    private void UpdateHandTracking()
    {
        XRHand hand = _lastHandMoved == XRNode.LeftHand ? _handSubsystem.leftHand : _handSubsystem.rightHand;
        if (!TryGetPalmPose(hand, out Pose palmPose)) return;

        Vector3 palmPosition = palmPose.position;

        if (!_isTracking && IsHeldWithOneHand())
        {
            // Start tracking when object is selected
            _startPalmPos = palmPosition;
            _isTracking = true;
        }

        if (_isTracking && IsHeldWithOneHand())
        {
            Vector3 displacement = palmPosition - _startPalmPos;

            // Use camera's backward direction as "pull direction"
            Vector3 pullDirection = -Camera.main.transform.forward;

            float pullDistance = Vector3.Dot(displacement, pullDirection);

            if (pullDistance >= pullDistanceZTreshold)
            {
                Debug.Log("Pull triggered with one hand.");
                OnPullDetected();

                // Reset the start position to allow for continuous detection
                _startPalmPos = palmPosition;
            }
        }

        // If the hand releases the object, stop tracking
        if (!IsHeldWithOneHand())
        {
            _isTracking = false;
        }
    }



    private bool TryGetPalmPose(XRHand hand, out Pose pose)
    {
        return hand.GetJoint(XRHandJointID.Palm).TryGetPose(out pose);
    }

    private void OnPullDetected()
    {
        Debug.Log("Pull Detected");

        var canvaLimiter = GetComponent<CanvaBoundLimiter>();
        var sphereLimiter = GetComponent<SphereBoundLimiterImage>();

        if (canvaLimiter != null) canvaLimiter.enabled = false;
        if (sphereLimiter != null) sphereLimiter.enabled = true;

        _startPosition = transform.position;
        _startRotation = transform.rotation;

        deleteArea.SetActive(true);
        audioSource.PlayOneShot(detachImage);
    }

    public void ReleaseDetected()
    {
        _isTracking = false;

        Vector3 checkCenter = transform.position;

        Collider[] boardHits = Physics.OverlapBox(checkCenter, _checkBoxHalfExtents, Quaternion.identity, _boardLayerMask);
        Collider[] deleteHits = Physics.OverlapBox(checkCenter, _checkBoxHalfExtents, Quaternion.identity, _deleteLayerMask);

        if (deleteHits.Length > 0)
        {
            OpenDeleteDialog();
        }
        else if (boardHits.Length > 0)
        {
            V_Moodboard moodboard = boardHits[0].transform.GetComponentInParent<V_Moodboard>();
            if (moodboard == null)
            {
                Debug.LogError("V_Moodboard component is missing on the board hit.");
                return;
            }

            if (moodboard.GetMoodboardId() != _currentMoodboardId)
            {
                RectTransform moodboardCenter = moodboard.MoodboardCenter;
                if (moodboardCenter != null)
                {
                    VM_AppData.Instance.MoveImageToAnotherBoard(_currentImageId, _currentMoodboardId, moodboard.GetMoodboardId());
                    _currentMoodboardId = moodboard.GetMoodboardId();

                    _imagePrefab = transform.parent;
                    _imagePrefab.SetParent(moodboardCenter);
                    _imagePrefab.SetLocalPositionAndRotation(new Vector3(17f, -15f, -100f), Quaternion.Euler(0f, 0f, 0f));

                    var canvaLimiter = GetComponent<CanvaBoundLimiter>();
                    var sphereLimiter = GetComponent<SphereBoundLimiterImage>();

                    if (sphereLimiter != null) sphereLimiter.enabled = false;

                    if (canvaLimiter != null)
                    {
                        canvaLimiter.SetBoardCanvas(moodboardCenter);
                        canvaLimiter.enabled = true;
                    }

                    transform.localPosition = boardHits[0].ClosestPoint(transform.position);
                    transform.rotation = Quaternion.LookRotation(moodboardCenter.forward, Vector3.up);

                    transform.parent.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                    audioSource.PlayOneShot(dropImageOtherBoard);
                }
                else
                {
                    Debug.LogError("BoardCanvas not found on the target board.");
                }
            }
            else
            {
                Debug.Log("Released over the starting board.");

                var canvaLimiter = GetComponent<CanvaBoundLimiter>();
                var sphereLimiter = GetComponent<SphereBoundLimiterImage>();

                if (canvaLimiter != null) canvaLimiter.enabled = true;
                if (sphereLimiter != null) sphereLimiter.enabled = false;

                transform.parent.localRotation = Quaternion.Euler(0f, 0f, 0f);
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
        else
        {
            Debug.Log("Released in empty area.");
            ImageBackToLastPosition();
            audioSource.PlayOneShot(dropImageVoid);
        }

        deleteArea.SetActive(false);
    }

    public void ImageBackToLastPosition()
    {
        var canvaLimiter = GetComponent<CanvaBoundLimiter>();
        var sphereLimiter = GetComponent<SphereBoundLimiterImage>();

        if (canvaLimiter != null) canvaLimiter.enabled = true;
        if (sphereLimiter != null) sphereLimiter.enabled = false;

        transform.SetPositionAndRotation(_startPosition, _startRotation);
        transform.parent.localRotation = Quaternion.Euler(0f, 0f, 0f);
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void OpenDeleteDialog()
    {
        deleteDialog.GetComponent<DeleteImageDialog>().enabled = true;
        deleteDialog.SetActive(true);
        PositioningHelper.PositionInFrontOfUser(deleteDialog, 2f, 1.5f);
        deleteDialog.GetComponent<DeleteImageDialog>().SetUp(transform.parent.gameObject, _currentImageId, _currentMoodboardId);
    }

    private bool IsHeldWithOneHand()
    {
        var manipulator = GetComponent<ObjectManipulator>();
        return manipulator != null && manipulator.interactorsSelecting.Count == 1;
    }

}
