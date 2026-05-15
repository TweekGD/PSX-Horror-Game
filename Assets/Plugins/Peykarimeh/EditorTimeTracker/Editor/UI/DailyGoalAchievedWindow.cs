using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Peykarimeh.EditorTimeTracker.UI
{
    public class DailyGoalAchievedWindow : EditorWindow
    {
        // Confetti Configuration
        private const int PARTICLE_COUNT = 100;
        private const float MIN_START_SPEED = 50f;
        private const float MAX_START_SPEED = 150f;
        private const float MAX_HORIZONTAL_SPEED = 30f;
        private const float GRAVITY_FORCE = 10f;

        private struct Confetti
        {
            public Vector2 position;
            public Vector2 velocity;
            public Color color;
            public float size;
            public float rotation;
            public float rotationSpeed;
        }

        private List<Confetti> confettiParticles = new List<Confetti>();
        private double lastTime;
        private readonly Color[] confettiColors = new Color[] 
        { 
            new Color(1f, 0.4f, 0.4f), new Color(0.4f, 1f, 0.4f), new Color(0.4f, 0.6f, 1f), 
            new Color(1f, 1f, 0.4f), new Color(1f, 0.6f, 1f) 
        };

        public static void ShowWindow()
        {
            DailyGoalAchievedWindow window = GetWindow<DailyGoalAchievedWindow>(true, "Goal Achieved!", true);
            Vector2 windowSize = new Vector2(400, 300);
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

        private void OnEnable()
        {
            lastTime = EditorApplication.timeSinceStartup;
            EditorApplication.update += UpdateConfetti;
            SpawnConfetti();
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateConfetti;
        }

        private void SpawnConfetti()
        {
            confettiParticles.Clear();
            for (int i = 0; i < PARTICLE_COUNT; i++)
            {
                confettiParticles.Add(new Confetti
                {
                    position = new Vector2(UnityEngine.Random.Range(0, 400), UnityEngine.Random.Range(-200, 0)),
                    velocity = new Vector2(UnityEngine.Random.Range(-MAX_HORIZONTAL_SPEED, MAX_HORIZONTAL_SPEED), UnityEngine.Random.Range(MIN_START_SPEED, MAX_START_SPEED)),
                    color = confettiColors[UnityEngine.Random.Range(0, confettiColors.Length)],
                    size = UnityEngine.Random.Range(3f, 6f),
                    rotation = UnityEngine.Random.Range(0, 360),
                    rotationSpeed = UnityEngine.Random.Range(-90, 90)
                });
            }
        }

        private void UpdateConfetti()
        {
            double time = EditorApplication.timeSinceStartup;
            float dt = (float)(time - lastTime);
            lastTime = time;

            for (int i = 0; i < confettiParticles.Count; i++)
            {
                Confetti p = confettiParticles[i];
                p.position += p.velocity * dt;
                p.velocity.y += GRAVITY_FORCE * dt;
                p.rotation += p.rotationSpeed * dt;

                if (p.position.y > 300)
                {
                    p.position.y = -10;
                    p.position.x = UnityEngine.Random.Range(0, 400);
                    p.velocity = new Vector2(UnityEngine.Random.Range(-MAX_HORIZONTAL_SPEED, MAX_HORIZONTAL_SPEED), UnityEngine.Random.Range(MIN_START_SPEED, MAX_START_SPEED));
                }
                confettiParticles[i] = p;
            }
            Repaint();
        }

        private void OnGUI()
        {
            TimeTrackerStyles.Init();
            TimeTrackerStyles.DrawBackgroundGradient(position);

            // Draw Confetti
            foreach (var p in confettiParticles)
            {
                var matrix = GUI.matrix;
                GUIUtility.RotateAroundPivot(p.rotation, p.position);
                EditorGUI.DrawRect(new Rect(p.position.x - p.size/2, p.position.y - p.size/2, p.size, p.size), p.color);
                GUI.matrix = matrix;
            }

            GUILayout.BeginArea(new Rect(20, 20, position.width - 40, position.height - 40));
            GUILayout.FlexibleSpace();

            GUILayout.Label("🎉 Daily Goal Achieved! 🎉", TimeTrackerStyles.HeaderStyle);
            GUILayout.Space(20);
            GUILayout.Label("Great job! You've hit your daily target.", TimeTrackerStyles.SubtextStyle);
            GUILayout.Label("Keep up the momentum or take a well-deserved rest.", TimeTrackerStyles.SubtextStyle);

            GUILayout.FlexibleSpace();

            TimeTrackerStyles.DrawCard(() =>
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (TimeTrackerStyles.DrawStyledButton("Awesome! Close", TimeTrackerStyles.ACCENT_GOLD, 40))
                {
                    Close();
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            });

            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
        }
    }
}