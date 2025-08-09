
# 📈 Analysis of User Study Results

This section summarizes the key findings from a user study, focusing on the **effectiveness**, **usability**, and **user preferences** of different input modalities. The results are exclusively based on the provided images.

---

## 📋 Test Procedure

The test followed the procedure illustrated in the graph below. The study involved 21 participants.
<div align="center"> <img src="./Images/study_results/Test_modality.png" alt="Test procedure workflow and duration per task" width="900"/> </div>

## 📊 User Satisfaction and Workload

<table>
  <tr>
    <td align="center"><img src="./Images/study_results/SUS Score Distribution by Input Method.png" alt="SUS Score Distribution by Input Method" width="400"/><br/><b>SUS Score Distribution</b></td>
    <td align="center"><img src="./Images/study_results/NASA-TLX Score Distribution by Input Method.png" alt="NASA-TLX Score Distribution by Input Method" width="400"/><br/><b>NASA-TLX Workload</b></td>
  </tr>
</table>

* **SUS Score**: The "Hybrid" and "Traditional" input methods show similarly high usability. The "Gesture" method has a slightly lower median score and wider distribution, indicating mixed reception.
* **NASA-TLX**: Workload is relatively even across all input types. Gestures are slightly more demanding, but not significantly so.

---

## 🎯 User Preferences

<table>
  <tr>
    <td align="center"><img src="./Images/study_results/Users' Favorite Interaction Mode.png" alt="Favorite Interaction Mode" width="500"/><br/><b>Favorite Interaction Mode</b></td>
    <td align="center"><img src="./Images/study_results/Users' Most Natural Perceived Interaction Mode.png" alt="Most Natural Interaction Mode" width="500"/><br/><b>Most Natural Interaction Mode</b></td>
  </tr>
</table>

* **Favorite Mode**: The Hybrid mode is by far the most appreciated.
* **Naturalness**: Users also perceive the Hybrid mode as the most natural. The Gesture mode scores slightly better than Traditional on this aspect.

---

## ⏱️ Task Performance and Efficiency

<div align="center">
  <img src="./Images/study_results/Average Duration per Task.png" alt="Average Duration per Task" width="600"/>
</div>

* **Task Duration**: Tasks were completed fastest with the Hybrid mode. Gestures are slightly faster than Traditional, suggesting potential for efficiency when well-integrated.

---

## 👋 Gesture Evaluation

<div align="center">
  <img src="./Images/study_results/Gesture Evaluation.png" alt="Gesture Evaluation" width="500"/>
</div>

**Overall Ratings**
All gestures were positively received by users.

<table> <tr> <td align="center"> <img src="./Images/study_results/Frame + Gesture Usability Radar Chart.png" alt="Frame Gesture Radar" width="300"/> </td> <td align="center"> <img src="./Images/study_results/Start Mic + Gesture Usability Radar Chart.png" alt="Start Mic Gesture Radar" width="300"/> </td> </tr> <tr> <td> <ul> <li>🟥 <b>Frame</b></li> <li>Most physically demanding</li> <li>Clear and appreciated, but may cause fatigue over time</li> </ul> </td> <td> <ul> <li>🟩 <b>Start Mic</b></li> <li>One of the least intuitive gestures</li> <li>Often confused or requiring explanation</li> </ul> </td> </tr> <tr> <td align="center"> <img src="./Images/study_results/Swipe + Gesture Usability Radar Chart.png" alt="Swipe Gesture Radar" width="300"/> </td> <td align="center"> <img src="./Images/study_results/Thumb + Gesture Usability Radar Chart.png" alt="Thumb Gesture Radar" width="300"/> </td> </tr> <tr> <td> <ul> <li>🟪 <b>Swipe</b></li> <li>Slightly lower responsiveness</li> <li>Confirmed by lower preference scores</li> </ul> </td> <td> <ul> <li>🟦 <b>Thumbs-Up</b></li> <li>Highly intuitive and easy to perform</li> <li>Praised for simplicity and effectiveness</li> </ul> </td> </tr> </table>

---

## 🧠 Gesture vs Button Usage in Hybrid Mode

In hybrid mode, user preferences vary depending on the type of action and the spatial context.

<div align="center"> <img src="./Images/study_results/Hybrid Mode Average User Preferences.png" alt="Example of close window" width="500"/> </div>

* 👋 **Gestures** are preferred for *activating voice input* and *creating images* or *generating moodboards*.
* ⬤ **Buttons** are more frequently used for navigating UI elements, particularly when they appear close to the user.

This distinction becomes clearer when comparing input usage between *close* and *far* interaction windows.

---

## 🔄 Hybrid Mode Input Preferences for navigating UI elements

<table>
  <tr>
    <td align="center">
      <img src="./Images/app_screenshots/close_window.png" alt="Example of close window" width="500"/><br/>
      <b>Example of Close Window</b>
    </td>
    <td align="center">
      <img src="./Images/study_results/avg_input_distribution_yes.png" alt="Yes Action Input Distribution" width="500"/><br/>
      <b>"Yes" (Close Window)</b>
    </td>
  </tr>
  <tr>
    <td align="center">
      <img src="./Images/app_screenshots/far_window.png" alt="Example of far window" width="500"/><br/>
      <b>Example of Far Window</b>
    </td>
    <td align="center">
      <img src="./Images/study_results/avg_input_distribution_next.png" alt="Next Action Input Distribution" width="500"/><br/>
      <b>"Next" (Far Window)</b>
    </td>
  </tr>
</table>

* ✅ **"Yes" Action (Close Interaction)**:
  Button input is predominant. The Thumbs-Up gesture is used in only **\~20%** of cases, suggesting a preference for more precise input when the UI is near.

* ⏭️ **"Next" Action (Far Interaction)**:
  While button use remains high, **gesture use increases significantly**. The Thumbs-Up gesture reaches **\~40%**, reflecting a shift toward more natural input at a distance.

---
