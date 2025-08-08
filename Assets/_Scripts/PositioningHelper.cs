using UnityEngine;

public static class PositioningHelper
{
    private static Camera _mainCamera;

    public static void PositionInFrontOfUser(GameObject obj, float distance = 0f, float? fixedY = null)
    {
        if (obj == null) return;

        if (_mainCamera == null)
            _mainCamera = Camera.main;

        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        Transform camTransform = _mainCamera.transform;

        // Calculate position in gaze direction
        Vector3 forward = camTransform.forward.normalized;
        Vector3 targetPosition = camTransform.position + forward * distance;

        // Override Y if specified
        if (fixedY.HasValue)
            targetPosition.y = fixedY.Value;

        // Canvas should face the user
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - camTransform.position);

        // Apply to RectTransform or normal Transform
        RectTransform rectTransform = obj.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            rectTransform.position = targetPosition;
            rectTransform.rotation = targetRotation;
        }
        else
        {
            obj.transform.SetPositionAndRotation(targetPosition, targetRotation);
        }
    }

    public static void PositionParentInFrontOfUser(Transform parentTransform, float distance = 0f, float? fixedY = null)
    {
        if (parentTransform == null) return;

        if (_mainCamera == null)
            _mainCamera = Camera.main;

        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        Transform camTransform = _mainCamera.transform;

        // Calculate position in gaze direction
        Vector3 forward = camTransform.forward.normalized;
        Vector3 targetPosition = camTransform.position + forward * distance;

        // Override Y if specified
        if (fixedY.HasValue)
            targetPosition.y = fixedY.Value;

        // Make the parent face the user
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - camTransform.position);

        // Apply to parent (which will move the children automatically)
        parentTransform.SetPositionAndRotation(targetPosition, targetRotation);
    }

}
