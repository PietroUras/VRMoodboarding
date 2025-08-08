## 🖼️ Image Generation Setup (NScale)

To enable image generation features via NScale's inference platform:

1. **Install [Python 3.12](https://www.python.org/downloads/release/python-3120/)** on your system.  
   - Ensure Python is added to your system's environment variables (`PATH`) during installation.
2. Navigate to `Assets/_Users` and open the file named `python_path`.  
   - Update it with the absolute path to your Python 3.12 installation and the folder containing the Unity project.  
3. Open the script `Image_Generation_API_Request_NScale.py` located in the `pythonProject1` directory.  
   - Insert your NScale API key in the following line:  
     ```python
     nscale_api_key = Api_Token.NSCALE_API_KEY
     ```
   - You can obtain an API token by following the instructions at:  
     [https://www.nscale.com/press-releases/ai-hyperscaler-nscale-launches-serverless-inference-platform](https://www.nscale.com/press-releases/ai-hyperscaler-nscale-launches-serverless-inference-platform)


## 🗣️ Speech Recognition Setup (Azure)

To enable voice input features via Azure Speech Services:

1. Generate a Speech-to-Text API key by following the official guide:  
   [https://learn.microsoft.com/en-us/azure/ai-services/speech-service/](https://learn.microsoft.com/en-us/azure/ai-services/speech-service/)
2. Navigate to `Assets/_Scripts/SpeechToText` and open the script `azureConfig.cs`.
3. Replace the placeholder in the `subscriptionKey` field with your actual API key.

---

## 🧱 MRTK3 Installation

The project relies on **Mixed Reality Toolkit 3 (MRTK3)**.  
Follow the official Microsoft documentation to install and configure MRTK3 in your Unity environment:

[https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk3-overview/getting-started/overview](https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk3-overview/getting-started/overview)

---

## 🧪 Testing the Application

To ensure correct behavior when testing the application:

- **Set the VR headset microphone as your default input device.**  
  For example, on Windows: `Settings > System > Sound > Input > Choose your input device`.
- **Install Meta Quest Link** and connect a supported headset (tested with both Quest 2 and Quest 3).
- **Open the Unity project** and run the scene titled `XR_BaseScene` only.
- **From desktop (first-time setup):**
  - Create a new user or select an existing one.
  - Choose an input mode. For full feature access, the *Hybrid* mode is recommended.
- **Continue the experience using the headset only.**
- To return to the main menu at any time, look at your palm with fingers extended to activate the **hand menu** gesture.

---

For any issues during installation or testing, please refer to the documentation or contact the repository maintainer.
