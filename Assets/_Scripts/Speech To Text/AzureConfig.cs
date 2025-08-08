using UnityEngine;

[CreateAssetMenu(fileName = "AzureConfig", menuName = "Scriptable Objects/AzureConfig")]
public class AzureConfig : ScriptableObject
{
    public string subscriptionKey = "";
    public string region = "";
}
