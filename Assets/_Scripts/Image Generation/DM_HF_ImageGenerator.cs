using UnityEngine;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;

public class DM_HF_ImageGenerator : I_ImageGenerator
{

    async public Task<(bool isSuccess, string error, string path)> GenerateImageAsync(
        string subject, string aspectRatio, string medium, string cameraPosition,
        string colors, string lighting, string mood, string projectBrief)
    {
        int seed = Random.Range(0, int.MaxValue);

        string args = $"\"{VM_AppData.Instance.GetScriptPath()}\" \"{subject}\" \"{aspectRatio}\" \"{medium}\" \"{cameraPosition}\" " +
                      $"\"{colors}\" \"{lighting}\" \"{mood}\" \"{projectBrief}\" \"{seed}\"";

        var startInfo = new ProcessStartInfo
        {
            FileName = VM_AppData.Instance.GetPythonPath(),
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        try
        {
            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();

                string imagePath = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                await Task.Run(() => process.WaitForExit());

                if (process.ExitCode == 0)
                {
                    return (true, string.Empty, imagePath.Trim());
                }
                else
                {
                    return (false, $"Python script error: {error}", string.Empty);
                }
            }
        }
        catch (System.Exception ex)
        {
            return (false, $"Exception: {ex.Message}", string.Empty);
        }
    }
}
