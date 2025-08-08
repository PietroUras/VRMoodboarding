using MixedReality.Toolkit.UX;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class V_PromptingDetails : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;

    [SerializeField] private PressableButton closeButton;

    [SerializeField] private PressableButton [] buttons;
    [SerializeField] private TextMeshProUGUI[] labels;
    [SerializeField] private V_Prompting V_Prompting;

    private VM_Prompting viewModelPrompting;

    private string content;

    private List<string> currentList = new List<string>();
    private string selectedElement = string.Empty;

    private void Awake()
    {
        closeButton.OnClicked.AddListener(OnCloseButtonClicked);
    }

    public void Initialize(VM_Prompting viewModel)
    {
        viewModelPrompting = viewModel;
    }

    public void SetCanva(string _title)
    {
        title.text = _title;
        content = _title;

        switch (content)
        {
            case "Style":
                currentList = viewModelPrompting.styleList;
                selectedElement = viewModelPrompting.GetSelectedStyle();
                break;
            case "Format":
                currentList = viewModelPrompting.formatList;
                selectedElement = viewModelPrompting.GetSelectedFormat();
                break;
            case "View":
                currentList = viewModelPrompting.viewList;
                selectedElement = viewModelPrompting.GetSelectedView();
                break;
            case "Color":
                currentList = viewModelPrompting.colorsList;
                selectedElement = viewModelPrompting.GetSelectedColor();
                break;
            case "Light":
                currentList = viewModelPrompting.lightList;
                selectedElement = viewModelPrompting.GetSelectedLight();
                break;
            case "Mood":
                currentList = viewModelPrompting.moodList;
                selectedElement = viewModelPrompting.GetSelectedMood();
                break;
        }

        LoadCanvaContent();


    }

    public void LoadCanvaContent()
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
        }

        for (int i =0; i < currentList.Count; i++)
        {
            buttons[i].gameObject.SetActive(true);
            labels[i].text = currentList[i];
        }

        UpdateViewSelectedElement();
    }

    public void UpdateViewSelectedElement()
    {
        for (int i = 0; i < currentList.Count; i++)
        {
            if (labels[i].text == selectedElement)
            {
                buttons[i].ForceSetToggled(true);
            }

            else
            {
                buttons[i].ForceSetToggled(false);
            }

        }
    }

    public void SetSelectedElementFromUI(GameObject buttonObject)
{
    if (buttonObject == null)
    {
        Debug.LogError("No button object passed");
        return;
    }

    var label = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
    if (label == null)
    {
        Debug.LogError("No TextMeshProUGUI component found in button");
        return;
    }

    selectedElement = label.text;

    UpdateViewSelectedElement();

    switch (content)
    {
        case "Style":
            viewModelPrompting.SetSelectedStyle(selectedElement);
            break;
        case "Format":
            viewModelPrompting.SetSelectedFormat(selectedElement);
            break;
        case "View":
            viewModelPrompting.SetSelectedView(selectedElement);
            break;
        case "Color":
            viewModelPrompting.SetSelectedColor(selectedElement);
            break;
        case "Light":
            viewModelPrompting.SetSelectedLight(selectedElement);
            break;
        case "Mood":
            viewModelPrompting.SetSelectedMood(selectedElement);
            break;
    }

    V_Prompting.UpdateLabels();
}
    public string GetContent()
    {
        return content;
    }

    private void OnCloseButtonClicked()
    {
        gameObject.SetActive(false);
    }

}
