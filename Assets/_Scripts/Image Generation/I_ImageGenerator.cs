using System.Threading.Tasks;
using UnityEngine;
public interface I_ImageGenerator
{
   Task<(bool isSuccess, string error, string path)> GenerateImageAsync(
        string subject, string aspectRatio, string medium, string cameraPosition,
        string colors, string lighting, string mood, string projectBrief);

}