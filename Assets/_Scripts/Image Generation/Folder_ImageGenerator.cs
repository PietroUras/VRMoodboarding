using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;

public class Folder_ImageGenerator : I_ImageGenerator
{
    private readonly string folderPath;

    public Folder_ImageGenerator(string path)
    {
        folderPath = path;

        if (!Directory.Exists(folderPath))
        {
            UnityEngine.Debug.LogError($"Directory does not exist: {folderPath}");
        }
    }

    public async Task<(bool isSuccess, string error, string path)> GenerateImageAsync(
        string subject, string aspectRatio, string medium, string cameraPosition,
        string colors, string lighting, string mood, string projectBrief)
    {
        try
        {
            // Get a random image from the folder
            string imagePath = await GetRandomImageFromFolderAsync();

            if (string.IsNullOrEmpty(imagePath))
                return (false, "No images found in folder", string.Empty);

            return (true, string.Empty, imagePath);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, string.Empty);
        }
    }

    private async Task<string> GetRandomImageFromFolderAsync()
    {
        return await Task.Run(() =>
        {
            if (!Directory.Exists(folderPath))
                return string.Empty;

            string[] imageFiles = Directory.GetFiles(folderPath, "*.*")
                                           .Where(f => f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                                       f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                       f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                                           .ToArray();

            if (imageFiles.Length == 0)
                return string.Empty;

            // Pick a random file
            System.Random random = new System.Random();
            int index = random.Next(imageFiles.Length);
            return imageFiles[index];
        });
    }

}