using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

public class SpeechToTextManager : MonoBehaviour
{
    public static SpeechToTextManager Instance { get; private set; }

    [SerializeField] private AzureConfig azureConfig;

    private SpeechRecognizer recognizer;

    private AudioHelper audioHelper;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeRecognizer();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioHelper = FindAnyObjectByType<AudioHelper>();
    }


    private void InitializeRecognizer()
    {
        var config = SpeechConfig.FromSubscription(azureConfig.subscriptionKey, azureConfig.region);
        var sourceLanguageConfig = SourceLanguageConfig.FromLanguage("it-IT");
        config.SetProfanity(ProfanityOption.Raw);

        recognizer = new SpeechRecognizer(config, sourceLanguageConfig);
    }

    public async Task<string> TryGetRecognitionResultAsync()
    {
        audioHelper.PlayListeningSound();
        
        var recognizeResult = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

        if (recognizeResult.Reason == ResultReason.RecognizedSpeech)
        {
            return Regex.Replace(recognizeResult.Text, @"[^\w\s]", ""); // return the cleaned result
        }

        return null; 
    }

    private async void OnDestroy()
    {
        if (recognizer != null)
        {
            // Stop continuous recognition asynchronously, if it was running
            await recognizer.StopContinuousRecognitionAsync();

            // Dispose the recognizer after stopping recognition
            recognizer.Dispose();
        }
    }
}
