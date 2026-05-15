using UnityEngine;
using UnityEditor;

namespace Peykarimeh.EditorTimeTracker.UI
{
    public class FocusBreakReminderWindow : EditorWindow
    {
        private System.Action<bool> onComplete;
        private int workDurationMinutes;
        
        public static void ShowWindow(int durationMinutes, System.Action<bool> callback)
        {
            FocusBreakReminderWindow window = GetWindow<FocusBreakReminderWindow>(true, "Focus Break Time!", true);
            window.workDurationMinutes = durationMinutes;
            window.onComplete = callback;
            
            Vector2 windowSize = new Vector2(480, 450);
            window.minSize = windowSize;
            window.maxSize = windowSize;
            
            Rect mainWindowRect = EditorGUIUtility.GetMainWindowPosition();
            Rect windowRect = new Rect(
                mainWindowRect.x + (mainWindowRect.width - windowSize.x) * 0.5f,
                mainWindowRect.y + (mainWindowRect.height - windowSize.y) * 0.5f,
                windowSize.x, windowSize.y
            );
            window.position = windowRect;
            
            window.Show();
            window.Focus();
        }
        
        private void OnGUI()
        {
            TimeTrackerStyles.Init();
            TimeTrackerStyles.DrawBackgroundGradient(position);
            
            GUILayout.BeginArea(new Rect(20, 20, position.width - 40, position.height - 40));
            
            GUILayout.Space(20);
            
            GUIStyle headerWithColor = new GUIStyle(TimeTrackerStyles.HeaderStyle);
            headerWithColor.normal.textColor = TimeTrackerStyles.ACCENT_YELLOW;
            GUILayout.Label("⏰ Focus Break Reminder!", headerWithColor);
            
            GUILayout.Space(20);
            GUILayout.Label($"You've been working for {workDurationMinutes} minutes.", TimeTrackerStyles.SubtextStyle);
            GUILayout.Space(15);
            
            TimeTrackerStyles.DrawCard(() => {
                GUILayout.Space(10);
                
                GUIStyle messageStyle = new GUIStyle(EditorStyles.label) {
                    fontSize = 14, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, wordWrap = true
                };
                messageStyle.normal.textColor = TimeTrackerStyles.ACCENT_YELLOW;
                GUILayout.Label("Time to take a break!", messageStyle);
                
                GUILayout.Space(15);
                
                GUIStyle listStyle = new GUIStyle(EditorStyles.label) {
                    fontSize = 12, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold
                };
                listStyle.normal.textColor = TimeTrackerStyles.TEXT_SECONDARY;
                
                GUILayout.Label("Take 5-10 minutes to:", listStyle);
                GUILayout.Space(8);
                
                DrawBulletPoint("💪 Stretch your body");
                DrawBulletPoint("👁️ Rest your eyes");
                DrawBulletPoint("💧 Grab some water");
                DrawBulletPoint("🚶 Walk around");
                
                GUILayout.Space(10);
            });
            
            GUILayout.Space(25);
            
            GUIStyle motivationStyle = new GUIStyle(EditorStyles.label) {
                fontSize = 11, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Italic, wordWrap = true
            };
            motivationStyle.normal.textColor = TimeTrackerStyles.TEXT_SECONDARY;
            GUILayout.Label("Regular breaks improve focus and productivity!", motivationStyle);
            
            GUILayout.Space(25);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (TimeTrackerStyles.DrawStyledButton("☕ I'll Take a Break", TimeTrackerStyles.ACCENT_GREEN, 50, 170))
            {
                Respond(true);
                Close();
            }
            
            GUILayout.Space(15);
            
            if (TimeTrackerStyles.DrawStyledButton("⏰ Remind in 5 min", TimeTrackerStyles.ACCENT_YELLOW, 50, 170))
            {
                Respond(false);
                Close();
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            GUILayout.EndArea();
        }

        private void DrawBulletPoint(string text)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(40);
            
            GUIStyle bulletStyle = new GUIStyle(EditorStyles.label) {
                fontSize = 13, wordWrap = false
            };
            bulletStyle.normal.textColor = TimeTrackerStyles.TEXT_PRIMARY;
            GUILayout.Label(text, bulletStyle);
            
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        private void Respond(bool takingBreak)
        {
            var callback = onComplete;
            onComplete = null; 
            
            if (callback != null)
            {
                callback.Invoke(takingBreak);
            }
            else
            {
                if (takingBreak) TimeTrackerEngine.StartBreakMode();
                else TimeTrackerEngine.SnoozeBreakReminder();
            }
        }
        
        private void OnDestroy()
        {
            Respond(false);
        }
    }
}