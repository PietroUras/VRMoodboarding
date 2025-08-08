using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VM_Prompting
{
    public List<string> styleList { get; private set; } = new List<string> { "Photography", "Painting", "Line Art Drawing", "Digital Drawing", "Render 3D" };
    public List<string> formatList { get; private set; } = new List<string> { "Square", "Portrait", "Landscape" };
    public List<string> viewList { get; private set; } = new List<string> { "In Front", "Side", "Behind", "Over", "Under" };
    public List<string> colorsList { get; private set; } = new List<string> { "Normal Colors", "Black And White", "Low Saturation", "High Saturation" };
    public List<string> lightList { get; private set; } = new List<string> { "Day", "Golden Hour", "Night", "Studio", "Neon" };
    public List<string> moodList { get; private set; } = new List<string> { "Relaxing", "Dreamy", "Epic", "Playful", "Nostalgic" };


    public bool isProjectBriefEnabled = false;

    private string selectedMood;
    private string selectedStyle;
    private string selectedFormat;
    private string selectedView;
    private string selectedColor;
    private string selectedLight;

    public void InitialSetUp()
    {
        selectedMood = moodList[0];
        selectedStyle = styleList[0];
        selectedFormat = formatList[0];
        selectedView = viewList[0];
        selectedColor = colorsList[0];
        selectedLight = lightList[0];
    }

    public void OnProjectBriefClick()
    {
        isProjectBriefEnabled = !isProjectBriefEnabled;
    }

    public void OnRandomParametersClick()
    {
        selectedStyle = styleList[Random.Range(0, styleList.Count)];
        selectedFormat = formatList[Random.Range(0, formatList.Count)];
        selectedView = viewList[Random.Range(0, viewList.Count)];
        selectedColor = colorsList[Random.Range(0, colorsList.Count)];
        selectedLight = lightList[Random.Range(0, lightList.Count)];
        selectedMood = moodList[Random.Range(0, moodList.Count)];
    }

    #region Setters

    public void SetSelectedMood(string mood)
    {
        selectedMood = mood;
    }

    public void SetSelectedStyle(string style)
    {
        selectedStyle = style;
    }

    public void SetSelectedFormat(string format)
    {
        selectedFormat = format;
    }
    public void SetSelectedView(string view)
    {
        selectedView = view;
    }
    public void SetSelectedColor(string color)
    {
        selectedColor = color;
    }
    public void SetSelectedLight(string light)
    {
        selectedLight = light;
    }
    #endregion

    #region Getters
    public string GetSelectedMood()
    {
        return selectedMood;
    }

    public string GetSelectedStyle()
    {
        return selectedStyle;
    }

    public string GetSelectedFormat()
    {
        return selectedFormat;
    }

    public string GetSelectedView()
    {
        return selectedView;
    }

    public string GetSelectedColor()
    {
        return selectedColor;
    }

    public string GetSelectedLight()
    {
        return selectedLight;
    }
    #endregion
}
