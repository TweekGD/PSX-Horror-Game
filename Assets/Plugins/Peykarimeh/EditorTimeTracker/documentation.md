# Editor Time Tracker - Full Documentation

## 1. Introduction

The **Editor Time Tracker** is a robust Unity extension created to reliably measure productive time spent inside the Unity Editor. It accurately logs active work, monitors idle states, and offers deep configurability to support a healthy and focused development workflow.

---

## 2. Core Features Explained

### 📊 Automatic Activity Tracking
The tracker seamlessly integrates into the Unity Editor event loop. It watches for user interactions (e.g., mouse movements, clicks, scrolling, and keyboard actions). If you are actively interacting with elements in the Editor, the timer treats it as productive time and adds it to the current session and total project runtime. 

### ⏱ Goal Management
You have two distinct tracking styles based on your preferred project scope:
- **Fixed Daily Goal:** Helps you hit a specific number of working hours each day (e.g., 4 or 8 hours).
- **Dynamic Weekly Goal:** Spreads a set number of targeting hours across a customized workweek (e.g., 40 hours between Monday and Friday). The UI dynamically adjusts daily metrics to match the weekly pacing.

### 💤 Intelligent Idle Detection (AFK)
To prevent your runtime logs from falsely stacking while you are away from the keyboard, the system constantly monitors time elapsed since your last input.
- If the time extends beyond your **Idle Timeout Limit**, the tracker pauses automatically.
- Upon interacting with the Editor again, a Welcome Back dialog appears. You can decide if the away time was work-related (like browsing APIs, drawing concepts, or a meeting), allowing you to safely credit backend hours correctly or discard them.

### ⏰ Focus Break Reminders
Studies show Pomodoro-style breaks improve creative stamina. This tracker automatically warns you when you have been coding actively for an extended duration.
- It supports sound alerts.
- A dialog window will block the screen requesting you step away to rest your eyes, body, and hydrate.

---

## 3. Configuration & Tools

To configure the tracker to your preferences, open the Tracker window (`Tools > Project Time Tracker`) and unfold the **⚙ SETTINGS & TOOLS** tab at the bottom.

### User Interface (UI)
- **View Mode:** Toggle between `Default` (contains rich progress bars and full data) and `Minimalist` (simplifies the window to mostly show the circular timer and digits to preserve screen real estate).
- **UI Theme:** Color your workspace! Switch between themes: *Default (Dark)*, *Monochrome*, *Cyberpunk*, and *Midnight*.

### Daily & Weekly Goals
- **Goal Mode:** Setting between `Fixed Daily` and `Dynamic Weekly`.
- **Week Start / End:** Define the bounding calendar days of your work cycle for the Weekly Goal calculations.
- **Date Format:** Choose your optimal localized date reading formats. 

### Focus Breaks
- **Enable Reminders:** Toggle the system on/off.
- **Interval (Minutes):** Set the frequency between work blocks (e.g., every 45 mins or 60 mins).
- **Show Dialog Box:** Forces a visual breakup with a prompt to step away.
- **Play Sound Alert:** Chimes the Editor terminal beep alongside the prompt.

### Idle Settings (AFK)
- **Timeout (Seconds):** The tolerance before categorizing interaction as AFK. Default is 120 seconds.
- **Auto-Focus Window:** Pulls the Tracker Window into focus forcefully when an AFK session trips, ensuring the dialogue prompts you the moment you return.

---

## 4. Advanced Data & Persistence

### Safe Execution Loop
Unlike simplistic editor scripts, this tool maintains constant persistence logic utilizing Unity's `SessionState` for real-time tracking, keeping running states perfectly valid even during complex recompilations or temporary Unity freezes.

### Local Storage & Privacy
All stored values securely reside in Editor Preferences (`EditorPrefs`). Your logs are local, unshared, and explicitly tied to your current Unity project workspace footprint (using `Application.dataPath` hashing).

### CSV Exporting
To perform complex billing, tracking, or integration with team pipelines:
1. Open up the Tracker Settings.
2. Click **📊 Export CSV**.
3. A file automatically creates and saves alongside the root of the project `TimeLogs_Daily.csv`, generating an ordered list of Dates, Exact Durations (Seconds), and Formatted Timestamp durations.

---

## 5. Technical Compatibility & Best Practices

- **Namespace Scope:** Built cleanly inside `Peykarimeh.EditorTimeTracker`.
- **Unity Version:** Requires standard event loop behaviors built reliably into Unity 2020.3 LTS and beyond. No extra plugin dependencies are required. 
- **Script Customization:** Confetti parameters, UI drawing handles, and goal algorithms are open to adjustment through the source scripts in the `Editor/Core` and `Editor/UI` folders respectively. 

---
*Developed by Nima Peykarimeh*
