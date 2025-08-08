using System;
using TMPro;
using UnityEngine;

public class V_ImageDetails : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI prompt;
    [SerializeField] private TextMeshProUGUI promptDetails;

    ImageData currentImage;

    public void SetUpImageDetails(ImageData imageData, Transform imageTransform)
    {
        currentImage = imageData;

        Vector3 newPosition = imageTransform.position + Vector3.left * 2f;
        transform.position = newPosition;

        prompt.text = currentImage.UserPrompt;

        promptDetails.text =
             $"Format: {currentImage.Format},\n" +
             $"Style: {currentImage.Style},\n" +
             $"View: {currentImage.View},\n" +
             $"Colors: {currentImage.Colors},\n" +
             $"Light: {currentImage.Light},\n" +
             $"Mood: {currentImage.Mood}";
    }

}
