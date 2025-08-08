# VRMoodboarding

This repository contains the codebase for my master's thesis:

**_Designing for Spatial Computing: Exploring Natural and Traditional Interfaces in a VR Moodboarding Tool_**

This thesis was submitted in partial fulfillment of the requirements for the **Master’s Degree in Cinema and Media Engineering** at the **Politecnico di Torino**.

The full document will be available at: [https://webthesis.biblio.polito.it/](https://webthesis.biblio.polito.it/)

---


### 👀 Key Elements

| In-Headset View | External View |
|:-------------------------:|:-------------------------:|
| <img src="./gifs/moodboarding.gif" alt="VR Moodboarding Interface" width="400"/> | <img src="./gifs/person.gif" alt="User Wearing Headset Using the App" width="400"/> |
| *Image manipulation inside the virtual moodboard* | *Gesture-based interaction with free hands* |

---


## 🧩 Project Overview

This project presents a VR desktop application developed for **Meta Quest 2**, with a strong focus on **user experience (UX)** and **usability** within immersive environments.  
A custom prototyping system was designed and implemented using **Figma**, taking into account the **field of view (FoV)** and emphasizing a **modular component structure**. This approach aimed to simplify interaction without limiting creative possibilities, supporting both accessibility and expressiveness in the interface design.

This study investigates the effectiveness of **multimodal interaction techniques** within a spatial computing context, applied to a moodboarding system based on AI-generated images.

Three interaction modes are tested:

- 🎛️ A traditional interface relying on virtual buttons  
- ✋ A gesture-driven configuration using hand tracking for key operations such as triggering image generation, initiating speech recognition, and navigating the UI  
- 🔁 A hybrid interface combining both modalities

In addition to evaluating task accuracy, speed, and user engagement, the study explores user behavior within the hybrid setup to identify which interaction method users prefer for each type of task. This provides insights into the most congenial input strategies in creative spatial workflows.

---

## 🛠️ Setup and Installation

For detailed instructions on how to install and run the project locally, please refer to the [Installation Guide](./INSTALLATION.md) page.

---

## 🧠 Project Details

An in-depth explanation of the system architecture and software components is available in the [Architecture and Interaction Design Overview](./ARCHITECTURE_OVERVIEW.md) page.

## 📊 Study Results

A user study involving 21 participants was conducted to evaluate which interaction mode was the most efficient and preferred. The study included a detailed analysis of the custom gesture set designed specifically for this application, as well as emerging patterns in combining multiple input types in hybrid interaction modes.

For more information about the experiment and its outcomes, see the [Test Results](./TEST_RESULTS.md) page.

## 🙏 Acknowledgements

I would like to express my gratitude to the **Politecnico di Torino** for supporting this work and providing access to the **Azure Speech Recognition** service, which played a fundamental role in the implementation of voice-based interaction.

Special thanks also go to the following contributors and open platforms:

- [**Kenney**](https://kenney.nl/) for providing high-quality 3D assets used in the prototype  
- [**Unity Asset Store** – Loading Screen Animation](https://assetstore.unity.com/packages/tools/loading-screen-animation-98505) for the loading animation  
- [**Freesound.org**](https://freesound.org/) for several UI sound effects used throughout the application
- [**NScale**](https://www.nscale.com/) for offering free initial credits that enabled image generation via their inference service

---

## 📄 License

This work is licensed under the **Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License** (CC BY-NC-SA 4.0).  
This means you are free to:

- 🔄 Share and adapt the material  
- 🧪 Use it for personal, research, or educational purposes  

As long as you:

- ✍️ Give appropriate credit  
- 🚫 Do not use it for commercial purposes  
- 📎 Distribute any derivative works under the same license

[![License: CC BY-NC-SA 4.0](https://img.shields.io/badge/License-CC%20BY--NC--SA%204.0-lightgrey.svg)](https://creativecommons.org/licenses/by-nc-sa/4.0/)
