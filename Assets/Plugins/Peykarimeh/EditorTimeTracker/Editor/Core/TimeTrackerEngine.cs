using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Peykarimeh.EditorTimeTracker
{
    [InitializeOnLoad]
    public static class TimeTrackerEngine
    {
        // --- Data Classes inside namespace, kept identical to preserve JSON structure ---
        [Serializable]
        public class TrackerData
        {
            public double totalProjectTime = 0f;
            public List<DailyEntry> history = new List<DailyEntry>();
        }

        [Serializable]
        public class DailyEntry
        {
            public string dateID;
            public double duration;
        }

        [Serializable]
        public class TrackerSettings
        {
            public enum GoalType { FixedDaily, DynamicWeekly }
            public GoalType goalType = GoalType.FixedDaily;
            
            public int dailyGoalHours = 4;
            public int weeklyGoalHours = 20;
            public DayOfWeek startOfWeek = DayOfWeek.Monday;
            public DayOfWeek endOfWeek = DayOfWeek.Friday;
            
            public int autoPauseLimitSeconds = 120;
            public bool enableFocusReminder = true;
            public int focusReminderIntervalMinutes = 45;
            public bool focusReminderUseDialog = true;
            public bool focusReminderPlaySound = true;
            public bool afkAutoFocus = true;
            public bool afkPlaySound = true;
            public int dateFormatIndex = 0;
            public int themeIndex = 0;
            public bool minimalMode = false;
            public bool showSettings = false;
            public bool showFocusReminderSettings = false;
            public bool showIdleDetectionSettings = false;
        }

        // --- Core Properties ---
        public static TrackerData Data { get; private set; }
        public static TrackerSettings Settings { get; private set; }

        public static bool IsRunning { get; private set; }
        public static bool IsWaitingForAfkResponse { get; private set; }
        public static bool IsOnBreak { get; private set; }
        
        public static double CurrentSessionTime { get; private set; }
        public static double BreakStartTime { get; private set; }
        public static double PotentialIdleTimeToAdd { get; private set; }

        private static string PREFS_DATA => "PTT_Data_V8_" + Application.dataPath.GetHashCode();
        private static string PREFS_SETTINGS => "PTT_Settings_V8_" + Application.dataPath.GetHashCode();

        private static double lastUpdateTime;
        private static double lastActivityTimestamp;
        private static double lastFocusReminderTimestamp;
        private static string lastTrackedDate;
        private static double timeAddedSinceLastActivity;

        public static event Action OnDataUpdated;

        static TimeTrackerEngine()
        {
            LoadData();

            CurrentSessionTime = SessionState.GetFloat("PTT_SessionTime", 0f);
            IsOnBreak = SessionState.GetBool("PTT_IsOnBreak", false);
            BreakStartTime = SessionState.GetFloat("PTT_BreakStartTime", 0f);
            IsRunning = SessionState.GetBool("PTT_IsRunning", true);
            IsWaitingForAfkResponse = SessionState.GetBool("PTT_IsWaitingForAfkResponse", false);
            PotentialIdleTimeToAdd = SessionState.GetFloat("PTT_PotentialIdleTimeToAdd", 0f);
            lastActivityTimestamp = SessionState.GetFloat("PTT_LastActivityTimestamp", (float)EditorApplication.timeSinceStartup);
            lastFocusReminderTimestamp = SessionState.GetFloat("PTT_LastFocusReminderTimestamp", (float)EditorApplication.timeSinceStartup);
            lastTrackedDate = SessionState.GetString("PTT_LastTrackedDate", "");
            timeAddedSinceLastActivity = SessionState.GetFloat("PTT_TimeAddedSinceLastActivity", 0f);

            lastUpdateTime = EditorApplication.timeSinceStartup;

            EditorApplication.update += UpdateTimer;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public static void RegisterActivity()
        {
            lastActivityTimestamp = EditorApplication.timeSinceStartup;
            timeAddedSinceLastActivity = 0f;
            SessionState.SetFloat("PTT_LastActivityTimestamp", (float)lastActivityTimestamp);
            SessionState.SetFloat("PTT_TimeAddedSinceLastActivity", 0f);
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;
            if (e != null && (e.type == EventType.MouseMove || e.type == EventType.MouseDown || 
                             e.type == EventType.MouseDrag || e.type == EventType.MouseUp ||
                             e.type == EventType.KeyDown || e.type == EventType.KeyUp ||
                             e.type == EventType.ScrollWheel))
            {
                RegisterActivity();
            }
        }

        private static void UpdateTimer()
        {
            double now = EditorApplication.timeSinceStartup;
            double delta = now - lastUpdateTime;
            lastUpdateTime = now;

            if (delta > 3600 || delta < 0) delta = 0; 

            double timeSinceInput = now - lastActivityTimestamp;

            if (IsOnBreak)
            {
                OnDataUpdated?.Invoke();
                return;
            }

            if (IsRunning && !IsWaitingForAfkResponse)
            {
                if (timeSinceInput > Settings.autoPauseLimitSeconds)
                {
                    IsRunning = false;
                    IsWaitingForAfkResponse = true;

                    // Remove only the exact amount of time that was previously credited before going AFK
                    RemoveTime(timeAddedSinceLastActivity);
                    PotentialIdleTimeToAdd = timeAddedSinceLastActivity + delta;
                    
                    if (Settings.afkPlaySound) EditorApplication.Beep();
                    if (Settings.afkAutoFocus) FocusMainWindow();

                    SaveSessionState();
                }
                else
                {
                    AddValidTime(delta);
                    timeAddedSinceLastActivity += delta;

                    if (Settings.enableFocusReminder)
                    {
                        if ((now - lastFocusReminderTimestamp) > (Settings.focusReminderIntervalMinutes * 60f))
                        {
                            ShowFocusReminderNotification();
                            lastFocusReminderTimestamp = now; 
                            SaveSessionState();
                        }
                    }
                }
            }
            else if (IsWaitingForAfkResponse)
            {
                PotentialIdleTimeToAdd += delta;
                if (now % 5 < delta) SaveSessionState();
            }

            if (IsRunning || IsWaitingForAfkResponse)
            {
                if (now % 60 < delta) SaveData();
                OnDataUpdated?.Invoke();
            }
        }

        private static void FocusMainWindow()
        {
            EditorWindow window = EditorWindow.GetWindow<UI.EditorTimeTrackerWindow>("Tracker", false);
            if (window != null) window.Focus();
        }

        public static void AddValidTime(double seconds)
        {
            CurrentSessionTime += seconds;
            Data.totalProjectTime += seconds;
            
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            DailyEntry todayEntry = Data.history.FirstOrDefault(x => x.dateID == currentDate);

            float targetSeconds = CalculateTargetDailyHours() * 3600f;
            bool wasGoalReached = todayEntry != null && todayEntry.duration >= targetSeconds;

            if (todayEntry != null) 
            {
                todayEntry.duration += seconds;
            }
            else 
            {
                todayEntry = new DailyEntry { dateID = currentDate, duration = seconds };
                Data.history.Add(todayEntry);
            }

            if (todayEntry != null && !wasGoalReached && todayEntry.duration >= targetSeconds)
            {
                string sessionKey = "PTT_GoalReached_" + currentDate;
                if (!SessionState.GetBool(sessionKey, false))
                {
                    UI.DailyGoalAchievedWindow.ShowWindow();
                    SessionState.SetBool(sessionKey, true);
                }
            }
            
            if (!string.IsNullOrEmpty(lastTrackedDate) && lastTrackedDate != currentDate)
            {
                Debug.Log($"[Time Tracker] Day changed from {lastTrackedDate} to {currentDate}.");
            }
            
            lastTrackedDate = currentDate;
            
            SaveSessionState();
            OnDataUpdated?.Invoke();
        }

        public static void RemoveTime(double seconds)
        {
            CurrentSessionTime -= seconds;
            if(CurrentSessionTime < 0) CurrentSessionTime = 0;
            
            Data.totalProjectTime -= seconds;

            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            DailyEntry todayEntry = Data.history.FirstOrDefault(x => x.dateID == currentDate);
            if (todayEntry != null)
            {
                todayEntry.duration -= seconds;
                if (todayEntry.duration < 0) todayEntry.duration = 0;
            }
            SaveSessionState();
            OnDataUpdated?.Invoke();
        }

        public static void StartTimer()
        {
            IsRunning = true;
            IsWaitingForAfkResponse = false;
            lastUpdateTime = EditorApplication.timeSinceStartup;
            lastActivityTimestamp = EditorApplication.timeSinceStartup;
            lastFocusReminderTimestamp = EditorApplication.timeSinceStartup;
            SaveSessionState();
            OnDataUpdated?.Invoke();
        }

        public static void PauseTimer()
        {
            IsRunning = false;
            SaveSessionState();
            OnDataUpdated?.Invoke();
        }

        public static void ResumeFromAfk(bool addTime)
        {
            if (addTime) AddValidTime(PotentialIdleTimeToAdd);
            PotentialIdleTimeToAdd = 0;
            StartTimer();
        }

        public static void StartBreakMode()
        {
            IsRunning = false;
            IsOnBreak = true;
            BreakStartTime = EditorApplication.timeSinceStartup;
            SaveData();
            SaveSessionState();
            FocusMainWindow();
        }

        public static void EndBreak()
        {
            IsOnBreak = false;
            BreakStartTime = 0f;
            StartTimer();
        }

        public static void SnoozeBreakReminder()
        {
            lastFocusReminderTimestamp = EditorApplication.timeSinceStartup - ((Settings.focusReminderIntervalMinutes - 5f) * 60f);
            SaveSessionState();
        }

        private static void ShowFocusReminderNotification()
        {
            if (Settings.focusReminderPlaySound) EditorApplication.Beep();
            
            if (Settings.focusReminderUseDialog)
            {
                UI.FocusBreakReminderWindow.ShowWindow(Settings.focusReminderIntervalMinutes, (takingBreak) => {
                    if (takingBreak) StartBreakMode();
                    else SnoozeBreakReminder();
                });
            }
            else
            {
                Debug.Log("⏰ Take a Break!");
                FocusMainWindow();
            }
        }

        public static void SaveData()
        {
            EditorPrefs.SetString(PREFS_DATA, JsonUtility.ToJson(Data));
            EditorPrefs.SetString(PREFS_SETTINGS, JsonUtility.ToJson(Settings));
        }

        private static void LoadData()
        {
            Data = new TrackerData();
            Settings = new TrackerSettings();
            
            if (EditorPrefs.HasKey(PREFS_DATA)) 
                JsonUtility.FromJsonOverwrite(EditorPrefs.GetString(PREFS_DATA), Data);
            if (EditorPrefs.HasKey(PREFS_SETTINGS)) 
                JsonUtility.FromJsonOverwrite(EditorPrefs.GetString(PREFS_SETTINGS), Settings);
        }

        private static void SaveSessionState()
        {
            SessionState.SetFloat("PTT_SessionTime", (float)CurrentSessionTime);
            SessionState.SetBool("PTT_IsOnBreak", IsOnBreak);
            SessionState.SetFloat("PTT_BreakStartTime", (float)BreakStartTime);
            SessionState.SetBool("PTT_IsRunning", IsRunning);
            SessionState.SetBool("PTT_IsWaitingForAfkResponse", IsWaitingForAfkResponse);
            SessionState.SetFloat("PTT_PotentialIdleTimeToAdd", (float)PotentialIdleTimeToAdd);
            SessionState.SetFloat("PTT_LastActivityTimestamp", (float)lastActivityTimestamp);
            SessionState.SetFloat("PTT_LastFocusReminderTimestamp", (float)lastFocusReminderTimestamp);
            SessionState.SetString("PTT_LastTrackedDate", lastTrackedDate);
            SessionState.SetFloat("PTT_TimeAddedSinceLastActivity", (float)timeAddedSinceLastActivity);
        }

        // --- Stats & Calculations ---
        public static double GetTodayTotalSeconds()
        {
            string todayDate = DateTime.Now.ToString("yyyy-MM-dd");
            DailyEntry todayEntry = Data.history.FirstOrDefault(x => x.dateID == todayDate);
            return todayEntry != null ? todayEntry.duration : 0f;
        }

        public static void GetWeeklyStats(out double totalWorkedInWeek, out double workedStrictlyBeforeToday, out int daysRemainingInWeek)
        {
            DateTime today = DateTime.Now.Date;
            int diff = (7 + (today.DayOfWeek - Settings.startOfWeek)) % 7;
            DateTime weekStart = today.AddDays(-1 * diff).Date;
            daysRemainingInWeek = 7 - diff;
            totalWorkedInWeek = 0;
            workedStrictlyBeforeToday = 0;

            foreach(var entry in Data.history)
            {
                if(DateTime.TryParse(entry.dateID, out DateTime entryDate))
                {
                    if(entryDate >= weekStart && entryDate <= today) 
                    {
                        totalWorkedInWeek += entry.duration;
                        if(entryDate < today) workedStrictlyBeforeToday += entry.duration;
                    }
                }
            }
        }

        public static float CalculateTargetDailyHours()
        {
            if (Settings.goalType == TrackerSettings.GoalType.FixedDaily) return Settings.dailyGoalHours;
            
            GetWeeklyStats(out _, out double workedBeforeToday, out _);
            double weeklyTargetSeconds = Settings.weeklyGoalHours * 3600.0;
            double remainingSeconds = weeklyTargetSeconds - workedBeforeToday;
            
            if (remainingSeconds <= 0) return 0f;
            
            DateTime today = DateTime.Now.Date;
            int diffToStart = (7 + (today.DayOfWeek - Settings.startOfWeek)) % 7;
            DateTime weekStart = today.AddDays(-1 * diffToStart).Date;
            
            int daysFromStartToEnd = ((int)Settings.endOfWeek - (int)Settings.startOfWeek + 7) % 7;
            DateTime workEnd = weekStart.AddDays(daysFromStartToEnd);
            
            int divisor = (today <= workEnd) ? (workEnd - today).Days + 1 : 1;
            return (float)((remainingSeconds / divisor) / 3600.0);
        }

        public static double GetWeeklyTargetSeconds()
        {
            if (Settings.goalType == TrackerSettings.GoalType.DynamicWeekly) return Settings.weeklyGoalHours * 3600.0;
            int workDays = Mathf.Max(1, ((int)Settings.endOfWeek - (int)Settings.startOfWeek + 7) % 7 + 1);
            return Settings.dailyGoalHours * workDays * 3600.0;
        }

        public static string FormatTime(double timeInSeconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(timeInSeconds);
            return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);
        }

        public static string FormatTimeShort(double timeInSeconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(timeInSeconds);
            return string.Format("{0:D2}h {1:D2}m", (int)t.TotalHours, t.Minutes);
        }

        public static DateTime GetStartOfCurrentCycle(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - Settings.startOfWeek)) % 7;
            return date.AddDays(-diff).Date;
        }

        public static string FormatDate(DateTime date)
        {
            switch (Settings.dateFormatIndex)
            {
                case 1: return date.ToString("MMM dd, yyyy");
                case 2: return date.ToString("yyyy-MM-dd");
                case 3: return date.ToString("dd/MM/yyyy");
                default: return date.ToString("dd MMM yyyy");
            }
        }

        public static void ExportCSV()
        {
            string path = Application.dataPath + "/../TimeLogs_Daily.csv";
            var lines = new List<string> { "Date,Duration(Seconds),Formatted" };

            foreach (var entry in Data.history)
            {
                TimeSpan t = TimeSpan.FromSeconds(entry.duration);
                string formatted = string.Format("{0}h {1}m", (int)t.TotalHours, t.Minutes);
                lines.Add($"{entry.dateID},{entry.duration},{formatted}");
            }
            lines.Add($",,");
            lines.Add($"TOTAL,,{FormatTime(Data.totalProjectTime)}");

            File.WriteAllLines(path, lines);
            EditorUtility.RevealInFinder(path);
        }
    }
}