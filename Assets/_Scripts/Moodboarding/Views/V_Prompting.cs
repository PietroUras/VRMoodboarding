using MixedReality.Toolkit.UX;
using TMPro;
using UnityEngine;

public class V_Prompting : MonoBehaviour
{
    [SerializeField] private PressableButton projectBriefButton;
    [SerializeField] private PressableButton randomParametersButton;

    [SerializeField] private PressableButton moodButton;
    [SerializeField] private PressableButton styleButton;
    [SerializeField] private PressableButton formatButton;
    [SerializeField] private PressableButton viewButton;
    [SerializeField] private PressableButton colorButton;
    [SerializeField] private PressableButton lightButton;

    [SerializeField] private TextMeshProUGUI moodLabel;
    [SerializeField] private TextMeshProUGUI styleLabel;
    [SerializeField] private TextMeshProUGUI formatLabel;
    [SerializeField] private TextMeshProUGUI viewLabel;
    [SerializeField] private TextMeshProUGUI colorLabel;
    [SerializeField] private TextMeshProUGUI lightLabel;

    [SerializeField] private V_PromptingDetails promptingDetails;

    private VM_Prompting viewModel;

    private void Awake()
    {
        projectBriefButton.OnClicked.AddListener(OnProjectBriefClick);
        randomParametersButton.OnClicked.AddListener(OnRandomParametersClick);
        moodButton.OnClicked.AddListener(OnMoodSelection);
        styleButton.OnClicked.AddListener(OnStyleSelection);
        formatButton.OnClicked.AddListener(OnFormatSelection);
        viewButton.OnClicked.AddListener(OnViewSelection);
        colorButton.OnClicked.AddListener(OnColorSelection);
        lightButton.OnClicked.AddListener(OnLightSelection);

        viewModel = new VM_Prompting();
        viewModel.InitialSetUp();

        promptingDetails.Initialize(viewModel);

        projectBriefButton.GetComponent<PressableButton>().ForceSetToggled(true);
        UpdateLabels();

    }

    private void OnEnable()
    {
        projectBriefButton.GetComponent<PressableButton>().ForceSetToggled(true);
    }

    private void OnProjectBriefClick()
    {
        viewModel.OnProjectBriefClick();
    }
    private void OnRandomParametersClick()
    {
        viewModel.OnRandomParametersClick();
        UpdateLabels();
        promptingDetails.gameObject.SetActive(false);
    }

    private void ShowPromptingDetails(string option)
    {
        if (!promptingDetails.gameObject.activeSelf)
        {
            promptingDetails.gameObject.SetActive(true);
        }
    }

    private void OnMoodSelection()
    {
        ShowPromptingDetails(Options.Mood.ToString());

        promptingDetails.SetCanva(Options.Mood.ToString());
    }
    private void OnStyleSelection()
    {
        ShowPromptingDetails(Options.Style.ToString());

        promptingDetails.SetCanva(Options.Style.ToString());
    }

    private void OnFormatSelection()
    {
        ShowPromptingDetails(Options.Format.ToString());

        promptingDetails.SetCanva(Options.Format.ToString());
    }
    private void OnViewSelection()
    {
        ShowPromptingDetails(Options.View.ToString());

        promptingDetails.SetCanva(Options.View.ToString());
    }

    private void OnColorSelection()
    {
        ShowPromptingDetails(Options.Color.ToString());

        promptingDetails.SetCanva(Options.Color.ToString());
    }   

    private void OnLightSelection()
    {
        ShowPromptingDetails(Options.Light.ToString());

        promptingDetails.SetCanva(Options.Light.ToString());
    }

    public void UpdateLabels() 
    {
        moodLabel.text = viewModel.GetSelectedMood();
        styleLabel.text = viewModel.GetSelectedStyle();
        formatLabel.text = viewModel.GetSelectedFormat();
        viewLabel.text = viewModel.GetSelectedView();
        colorLabel.text = viewModel.GetSelectedColor();
        lightLabel.text = viewModel.GetSelectedLight();
    }

    private enum Options
    {
        Style,
        Mood,
        Format,
        View,
        Color,
        Light
    }

    public VM_Prompting GetViewModel()
    {
        return viewModel;
    }
}
