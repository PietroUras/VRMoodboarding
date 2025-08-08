using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvaBoundLimiter : MonoBehaviour
{
    private RectTransform boardCanvas;
    public float zOffset = 5f;

    void Start()
    {
        if (TryGetComponent(out RectTransform imageRect))
        {
            boardCanvas = imageRect.parent.parent.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("CanvaBoundLimiter must be attached to a RectTransform.");
        }
    }

    void Update()
    {
        SetLimits();
    }

    void SetLimits()
    {
        Vector3 localPosition = transform.localPosition;
        localPosition.x = Mathf.Clamp(localPosition.x, boardCanvas.rect.xMin, boardCanvas.rect.xMax);
        localPosition.y = Mathf.Clamp(localPosition.y, boardCanvas.rect.yMin, boardCanvas.rect.yMax);
        localPosition.z = boardCanvas.position.z - zOffset;

        transform.localPosition = localPosition;
    }

    public void SetBoardCanvas(RectTransform boardCanvas)
    {
        this.boardCanvas = boardCanvas;
    }
}