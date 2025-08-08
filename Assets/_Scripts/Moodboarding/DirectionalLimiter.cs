using System.Collections.Generic;
using UnityEngine;

public class DirectionalLimiter : MonoBehaviour
{
    [SerializeField] private RectTransform thisBoard;

    private BoardsManager boardsManager;

    [SerializeField] private List<RectTransform> otherBoards;

    [SerializeField] private float minAngleSeparation = 10f;

    private Transform cameraTransform;

    private Vector3 lastValidPosition;
    private bool isManipulating = false;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    public void SetUp( RectTransform _thisBoard, BoardsManager _boardsManager)
    {
        thisBoard = _thisBoard;
        boardsManager = _boardsManager;
    }

    public void OnBeginManipulation()
    {
        isManipulating = true;
        lastValidPosition = thisBoard.position;

        otherBoards = boardsManager.GetAllMoodboardsTransform();
        otherBoards.Remove(thisBoard); // Remove this board from the list
    }

    public void OnEndManipulation()
    {
        isManipulating = false;
    }

    private void Update()
    {
        if (!isManipulating || cameraTransform == null) return;

        Vector3 proposedPosition = thisBoard.position;
        Vector3 movementDelta = proposedPosition - lastValidPosition;
        float deltaX = movementDelta.x;

        if (IsBlockedByAngle(proposedPosition, deltaX))
        {
            thisBoard.position = lastValidPosition;
            Debug.Log("Blocked movement due to angular overlap.");
        }
        else
        {
            lastValidPosition = proposedPosition;
        }
    }

    private bool IsBlockedByAngle(Vector3 proposedPosition, float deltaX)
    {
        Vector3 camToThis = proposedPosition - cameraTransform.position;

        foreach (RectTransform other in otherBoards)
        {
            if (other == null || other == thisBoard) continue;

            Vector3 camToOther = other.position - cameraTransform.position;

            float angle = Vector3.Angle(camToThis, camToOther);

            bool overlapY = Mathf.Abs(proposedPosition.y - other.position.y) <
                thisBoard.rect.height * 0.5f * thisBoard.lossyScale.y;

            if (angle < minAngleSeparation && overlapY)
            {
                // Block left
                if (deltaX < 0 && Vector3.Dot(camToThis, Vector3.Cross(Vector3.up, camToOther)) > 0)
                    return true;

                // Block right
                if (deltaX > 0 && Vector3.Dot(camToThis, Vector3.Cross(Vector3.up, camToOther)) < 0)
                    return true;
            }
        }

        return false;
    }
}
