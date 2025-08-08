using UnityEngine;
using System.Threading.Tasks;

public class VM_CreateImage
{
    private string brief = "";
    private V_Prompting promptingComponent;

    //private Folder_ImageGenerator imageGenerator; 
    private DM_HF_ImageGenerator imageGenerator;

    public VM_CreateImage(V_Prompting _promptingComponent)
    {
        //imageGenerator = new Folder_ImageGenerator("E:\\Generative-AI-Powered-Moodboarding\\RandomImages");

        imageGenerator = new DM_HF_ImageGenerator();

        promptingComponent = _promptingComponent;

    }

    public async Task<ImageData> CreateImage(string userPrompt, string format, string style,string view, string colors, string light, string mood)
    {
        if (promptingComponent.GetViewModel().isProjectBriefEnabled)
        {
            brief = VM_AppData.Instance.GetSelectedProject().Brief;
        }
        else
        {
            brief = "";
        }
        
        var (isSuccess, error, imagePath) = await imageGenerator.GenerateImageAsync(userPrompt, format, style, view, colors, light,mood, brief);

        if (isSuccess)
        {
            Debug.Log("Image generated successfully.");

            ImageData newImage = new ImageData
            {
                Src = imagePath,
                UserPrompt = userPrompt,
                Format = format,
                Style = style,
                View = view,
                Colors = colors,
                Light = light,
                Mood = mood,
            };

            return newImage;
        }
        else
        {
            Debug.Log($"Error generating image: {error}");
            return null;
        }
    }
}
