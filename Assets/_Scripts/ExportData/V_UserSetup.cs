using UnityEngine.UI;
using TMPro;
using UnityEngine;
using InputHelper;

public class V_UserSetup : MonoBehaviour
{
    [SerializeField] private Button createUser;
    [SerializeField] private TMP_Dropdown userDropdown;

    [SerializeField] private Button inputTraditional;
    [SerializeField] private Button inputGestures;
    [SerializeField] private Button inputHybrid;
    [SerializeField] private Button start;

    [SerializeField] private TMPro.TextMeshProUGUI statusText;

    private VM_UserSetup userSetup;

    private int currentUserNumber = 0;
    private bool newUserCreated = false;
    private string selectedInputMode = string.Empty;

    private void Start()
    {
        userSetup = new VM_UserSetup();

        //default setup
        currentUserNumber = userSetup.GetCurrentUserNumber() - 1;

        UpdateStatusText();

        createUser.onClick.AddListener(OnCreateUser);
        inputTraditional.onClick.AddListener(OnInputTraditional);
        inputGestures.onClick.AddListener(OnInputGestures);
        inputHybrid.onClick.AddListener(OnInputHybrid);
        start.onClick.AddListener(OnStart);

        PopulateUserDropdown(userSetup.GetCurrentUserNumber() - 1);

        userDropdown.onValueChanged.AddListener(delegate
        {
            OnSelectUserFromDropdown();
            UpdateStatusText();
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            OnCreateUser();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnInputTraditional();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnInputGestures();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            OnInputHybrid();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            OnStart();
        }
    }

    private void PopulateUserDropdown(int count)
    {
        userDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string> { "None" }; // Default null option  
        for (int i = 1; i <= count; i++)
        {
            options.Add($"U{i}");
        }
        userDropdown.AddOptions(options);
        userDropdown.value = count;
    }

    private void OnCreateUser()
    {
        userDropdown.value = 0; // Reset dropdown to "None"
        currentUserNumber = userSetup.GetCurrentUserNumber();
        SetButtonAsSelected(createUser);

        newUserCreated = true;
        UpdateStatusText();

    }

    private void OnSelectUserFromDropdown()
    {
        currentUserNumber = userDropdown.value;
        newUserCreated = false;

        UpdateStatusText();
        SetButtonAsNotSelected(createUser);
    }

    private void OnInputTraditional()
    {
        selectedInputMode = InputMode.Traditional.ToString();
        SetInputButtonAsSelected(inputTraditional);
        UpdateStatusText();
    }

    private void OnInputGestures()
    {
        selectedInputMode = InputMode.Gestures.ToString();
        SetInputButtonAsSelected(inputGestures);
        UpdateStatusText();
    }

    private void OnInputHybrid()
    {
        selectedInputMode = InputMode.Hybrid.ToString();
        SetInputButtonAsSelected(inputHybrid);
        UpdateStatusText();
    }

    private void UpdateStatusText()
    {
        statusText.text = (newUserCreated ? "New Tester" : string.Empty) + $" Current User: U{currentUserNumber}, Input Mode: {selectedInputMode}";
    }

    private void OnStart()
    {
        if (currentUserNumber != 0 && !string.IsNullOrEmpty(selectedInputMode))
        {
            userSetup.StartSession(currentUserNumber, selectedInputMode,newUserCreated);
        }
        else
        {
            statusText.text = ("Please select a user and input mode before starting the session.");
        }
    }

    private void SetButtonAsSelected(Button button)
    {
        var cb = button.colors;
        cb.normalColor = Color.blue;
        cb.highlightedColor = Color.blue;
        cb.selectedColor = Color.blue;
        button.colors = cb;
    }

    private void SetButtonAsNotSelected(Button button)
    {
        var cb = button.colors;
        cb.normalColor = new Color32(116, 116, 116, 255);
        cb.highlightedColor = new Color32(116, 116, 116, 255);
        cb.selectedColor = new Color32(116, 116, 116, 255);
        button.colors = cb;
    }

    private void SetInputButtonAsSelected(Button selectedButton)
    {
        // Set all to gray first
        SetButtonAsNotSelected(inputTraditional);
        SetButtonAsNotSelected(inputGestures);
        SetButtonAsNotSelected(inputHybrid);

        // Then color the selected one blue
        SetButtonAsSelected(selectedButton);
    }
}
