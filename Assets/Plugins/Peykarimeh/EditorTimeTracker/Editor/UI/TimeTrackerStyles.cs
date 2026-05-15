using UnityEngine;
using UnityEditor;

namespace Peykarimeh.EditorTimeTracker.UI
{
    public static class TimeTrackerStyles
    {
        // Color Palette (Properties based on Theme)
        public static Color BG_DARK => GetThemeColor("BG_DARK");
        public static Color BG_CARD => GetThemeColor("BG_CARD");
        public static Color BG_CARD_HOVER => GetThemeColor("BG_CARD_HOVER");
        public static Color ACCENT_GREEN => GetThemeColor("ACCENT_GREEN");
        public static Color ACCENT_YELLOW => GetThemeColor("ACCENT_YELLOW");
        public static Color ACCENT_BLUE => GetThemeColor("ACCENT_BLUE");
        public static Color ACCENT_RED => GetThemeColor("ACCENT_RED");
        public static Color ACCENT_GOLD => GetThemeColor("ACCENT_GOLD");
        public static Color TEXT_PRIMARY => GetThemeColor("TEXT_PRIMARY");
        public static Color TEXT_SECONDARY => GetThemeColor("TEXT_SECONDARY");
        public static Color TEXT_MUTED => GetThemeColor("TEXT_MUTED");
        public static Color DIVIDER => GetThemeColor("DIVIDER");

        private static Color GetThemeColor(string colorName)
        {
            int theme = TimeTrackerEngine.Settings.themeIndex;
            
            // 0: Default, 1: Monochrome, 2: Cyber, 3: Midnight
            switch(colorName)
            {
                case "BG_DARK": 
                    if(theme == 1) return new Color(0.1f, 0.1f, 0.1f);
                    if(theme == 2) return new Color(0.05f, 0.02f, 0.08f);
                    if(theme == 3) return new Color(0.08f, 0.11f, 0.15f);
                    return new Color(0.15f, 0.15f, 0.15f);
                case "BG_CARD": 
                    if(theme == 1) return new Color(0.15f, 0.15f, 0.15f);
                    if(theme == 2) return new Color(0.10f, 0.05f, 0.15f);
                    if(theme == 3) return new Color(0.12f, 0.16f, 0.22f);
                    return new Color(0.2f, 0.2f, 0.2f);
                case "BG_CARD_HOVER":
                    if(theme == 1) return new Color(0.18f, 0.18f, 0.18f);
                    if(theme == 2) return new Color(0.15f, 0.08f, 0.20f);
                    if(theme == 3) return new Color(0.15f, 0.19f, 0.26f);
                    return new Color(0.22f, 0.22f, 0.22f);
                case "ACCENT_GREEN":
                    if(theme == 1) return new Color(0.7f, 0.7f, 0.7f);
                    if(theme == 2) return new Color(0.0f, 1f, 0.8f);
                    if(theme == 3) return new Color(0.3f, 0.7f, 0.9f);
                    return new Color(0.4f, 0.8f, 0.5f);
                case "ACCENT_YELLOW":
                    if(theme == 1) return new Color(0.5f, 0.5f, 0.5f);
                    if(theme == 2) return new Color(1f, 0.0f, 0.6f);
                    if(theme == 3) return new Color(0.6f, 0.4f, 0.9f);
                    return new Color(1f, 0.85f, 0.4f);
                case "ACCENT_BLUE":
                    if(theme == 1) return new Color(0.4f, 0.4f, 0.4f);
                    if(theme == 2) return new Color(0.8f, 0.0f, 1f);
                    if(theme == 3) return new Color(0.4f, 0.5f, 0.8f);
                    return new Color(0.35f, 0.65f, 0.95f);
                case "ACCENT_RED":
                    if(theme == 1) return new Color(0.4f, 0.4f, 0.4f);
                    if(theme == 2) return new Color(1f, 0.2f, 0.2f);
                    if(theme == 3) return new Color(0.8f, 0.3f, 0.4f);
                    return new Color(0.82f, 0.36f, 0.36f);
                case "ACCENT_GOLD":
                    if(theme == 1) return new Color(0.8f, 0.8f, 0.8f);
                    if(theme == 2) return new Color(1f, 0.0f, 0.6f);
                    if(theme == 3) return new Color(0.5f, 0.7f, 1f);
                    return new Color(1f, 0.84f, 0.0f);
                case "TEXT_PRIMARY":
                    if(theme == 1) return new Color(0.9f, 0.9f, 0.9f);
                    if(theme == 2) return new Color(1f, 1f, 1f);
                    if(theme == 3) return new Color(0.9f, 0.95f, 1f);
                    return new Color(0.95f, 0.95f, 0.95f);
                case "TEXT_SECONDARY":
                    if(theme == 1) return new Color(0.6f, 0.6f, 0.6f);
                    if(theme == 2) return new Color(0.7f, 0.9f, 1f);
                    if(theme == 3) return new Color(0.6f, 0.7f, 0.8f);
                    return new Color(0.7f, 0.7f, 0.7f);
                case "TEXT_MUTED":
                    if(theme == 1) return new Color(0.4f, 0.4f, 0.4f);
                    if(theme == 2) return new Color(0.5f, 0.4f, 0.6f);
                    if(theme == 3) return new Color(0.4f, 0.5f, 0.6f);
                    return new Color(0.5f, 0.5f, 0.5f);
                case "DIVIDER":
                    if(theme == 1) return new Color(0.2f, 0.2f, 0.2f);
                    if(theme == 2) return new Color(0.3f, 0.1f, 0.4f);
                    if(theme == 3) return new Color(0.2f, 0.3f, 0.4f);
                    return new Color(0.3f, 0.3f, 0.3f);
            }
            return Color.white;
        }

        // GUIStyles
        public static GUIStyle BigTimerStyle { get; private set; }
        public static GUIStyle SubTimerStyle { get; private set; }
        public static GUIStyle HeaderStyle { get; private set; }
        public static GUIStyle SubtextStyle { get; private set; }
        public static GUIStyle SectionHeaderStyle { get; private set; }
        public static GUIStyle CardStyle { get; private set; }

        private static int lastThemeIndex = -1;

        public static void Init()
        {
            if (lastThemeIndex == TimeTrackerEngine.Settings.themeIndex && BigTimerStyle != null) return;
            lastThemeIndex = TimeTrackerEngine.Settings.themeIndex;

            BigTimerStyle = new GUIStyle(EditorStyles.label) { 
                fontSize = 28, 
                alignment = TextAnchor.MiddleCenter, 
                fontStyle = FontStyle.Bold 
            };
            BigTimerStyle.normal.textColor = TEXT_PRIMARY;
            
            SubTimerStyle = new GUIStyle(EditorStyles.label) { 
                fontSize = 13, 
                alignment = TextAnchor.MiddleCenter 
            };
            SubTimerStyle.normal.textColor = TEXT_MUTED;

            HeaderStyle = new GUIStyle(EditorStyles.boldLabel) {
                fontSize = 22,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            HeaderStyle.normal.textColor = ACCENT_GOLD;

            SubtextStyle = new GUIStyle(EditorStyles.label) {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };
            SubtextStyle.normal.textColor = TEXT_PRIMARY;

            SectionHeaderStyle = new GUIStyle(EditorStyles.foldout) { 
                fontStyle = FontStyle.Bold,
                fontSize = 11
            };

            CardStyle = new GUIStyle(EditorStyles.helpBox);
            CardStyle.normal.background = MakeBackgroundTexture(2, 2, BG_CARD);
        }

        public static void DrawBackgroundGradient(Rect position)
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), BG_DARK);
        }
        
        public static void DrawCard(System.Action content, float padding = 12)
        {
            Init();
            EditorGUILayout.BeginVertical(CardStyle);
            GUILayout.Space(padding);
            content?.Invoke();
            GUILayout.Space(padding);
            EditorGUILayout.EndVertical();
        }
        
        public static Texture2D MakeBackgroundTexture(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++) pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        
        public static bool DrawStyledButton(string text, Color accentColor, float height, float width = 180)
        {
            Color oldBg = GUI.backgroundColor;
            GUI.backgroundColor = accentColor;
            
            GUIStyle btnStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                fixedHeight = height,
                fixedWidth = width
            };
            
            bool clicked = GUILayout.Button(text, btnStyle);
            GUI.backgroundColor = oldBg;
            return clicked;
        }

        public static bool DrawMiniAdjustButton(string text, Color accentColor, string tooltip)
        {
            Color oldBg = GUI.backgroundColor;
            GUI.backgroundColor = accentColor;

            GUIStyle miniButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                fixedHeight = 46,
                fixedWidth = 50
            };

            bool clicked = GUILayout.Button(new GUIContent(text, tooltip), miniButtonStyle);

            GUI.backgroundColor = oldBg;
            return clicked;
        }
        
        public static void DrawRectBorder(Rect rect, Color color, float thickness)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
            EditorGUI.DrawRect(new Rect(rect.x + rect.width - thickness, rect.y, thickness, rect.height), color);
        }

        public static void DrawCircularProgress(Rect rect, float progress, Color trackColor, Color fillColor, float thickness)
        {
            Vector2 center = rect.center;
            float radius = Mathf.Min(rect.width, rect.height) * 0.5f;
            float centerRadius = radius - (thickness * 0.5f);
            float capRadius = thickness * 0.5f;
            Vector3 startDir = new Vector3(-0.5f, 0.866f, 0f).normalized;

            Handles.BeginGUI();
            Color previous = Handles.color;

            // Draw Track Arc
            Handles.color = trackColor;
            Handles.DrawSolidArc(center, Vector3.forward, startDir, 300f, radius);
            
            // Track Caps (Start and End)
            Handles.DrawSolidDisc((Vector3)center + startDir * centerRadius, Vector3.forward, capRadius);
            Vector3 trackEndDir = Quaternion.AngleAxis(300f, Vector3.forward) * startDir;
            Handles.DrawSolidDisc((Vector3)center + trackEndDir * centerRadius, Vector3.forward, capRadius);

            if (progress > 0f)
            {
                // Draw Fill Arc
                Handles.color = fillColor;
                Handles.DrawSolidArc(center, Vector3.forward, startDir, 300f * progress, radius);
                
                // Fill Caps (Start and End)
                Handles.DrawSolidDisc((Vector3)center + startDir * centerRadius, Vector3.forward, capRadius);
                Vector3 fillEndDir = Quaternion.AngleAxis(300f * progress, Vector3.forward) * startDir;
                Handles.DrawSolidDisc((Vector3)center + fillEndDir * centerRadius, Vector3.forward, capRadius);
            }

            // Cut the inner hole to create the ring effect
            Handles.color = BG_CARD;
            Handles.DrawSolidDisc(center, Vector3.forward, Mathf.Max(0f, radius - thickness));

            Handles.color = previous;
            Handles.EndGUI();
        }

        public static void DrawDivider()
        {
            GUILayout.Space(8);
            Rect divRect = GUILayoutUtility.GetRect(1, 1);
            EditorGUI.DrawRect(divRect, new Color(DIVIDER.r, DIVIDER.g, DIVIDER.b, 0.6f));
            GUILayout.Space(8);
        }
    }
}