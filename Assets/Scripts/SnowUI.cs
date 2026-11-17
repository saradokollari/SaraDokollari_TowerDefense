using UnityEngine;
using UnityEngine.InputSystem;

// Drawing helpers only. Do not attach this file to a GameObject.
public static class SnowUI
{
    public const float HeaderHeight = 200f;
    public static float Scale => Mathf.Max(0.1f,
        Mathf.Min(Screen.width / 1600f, Screen.height / 1080f));
    public static float Width => Screen.width / Scale;
    public static float Height => Screen.height / Scale;

    public static readonly Color Navy = new Color(0.035f, 0.10f, 0.17f, 1f);
    public static readonly Color PanelColor = new Color(0.06f, 0.18f, 0.27f, 1f);
    public static readonly Color Ice = new Color(0.42f, 0.85f, 0.96f, 1f);
    public static readonly Color Muted = new Color(0.72f, 0.85f, 0.91f, 1f);

    public static Vector2 MousePosition()
    {
        if (Mouse.current == null) return new Vector2(-1000f, -1000f);
        Vector2 p = Mouse.current.position.ReadValue();
        return new Vector2(p.x / Scale, (Screen.height - p.y) / Scale);
    }

    public static void Fill(Rect rect, Color color, float radius = 0f)
    {
        GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill,
            true, 0f, color, 0f, radius);
    }

    public static void Panel(Rect rect)
    {
        Fill(new Rect(rect.x, rect.y + 4f, rect.width, rect.height),
            new Color(0f, 0.02f, 0.06f, 0.35f), 14f);
        Fill(rect, new Color(0.25f, 0.52f, 0.65f, 1f), 14f);
        Fill(new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f,
            rect.height - 4f), PanelColor, 12f);
    }

    public static void Text(Rect rect, string text, Font font, int size,
        Color color, TextAnchor alignment = TextAnchor.MiddleLeft)
    {
        GUIStyle style = new GUIStyle(GUIStyle.none);
        style.font = font != null ? font : GUI.skin.font;
        style.fontSize = size;
        style.alignment = alignment;
        style.wordWrap = true;
        style.normal.textColor = color;
        GUI.Label(rect, text, style);
    }

    public static bool Button(Rect rect, string text, Font font,
        bool available = true, bool sell = false)
    {
        Color normal = sell ? new Color(0.55f, 0.24f, 0.20f, 1f)
                            : new Color(0.08f, 0.39f, 0.56f, 1f);
        Color hover = sell ? new Color(0.72f, 0.34f, 0.23f, 1f)
                           : new Color(0.10f, 0.55f, 0.73f, 1f);
        Color background = available
            ? (rect.Contains(MousePosition()) ? hover : normal)
            : new Color(0.13f, 0.22f, 0.28f, 1f);

        Fill(rect, available ? Ice : new Color(0.25f, 0.36f, 0.42f), 12f);
        Fill(new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f,
            rect.height - 4f), background, 10f);

        GUIStyle style = new GUIStyle(GUIStyle.none);
        style.font = font != null ? font : GUI.skin.font;
        style.fontSize = 22;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = available ? Color.white : Muted;
        style.hover.textColor = style.normal.textColor;
        style.active.textColor = style.normal.textColor;
        style.focused.textColor = style.normal.textColor;

        bool previous = GUI.enabled;
        GUI.enabled = previous && available;
        bool clicked = GUI.Button(rect, text, style);
        GUI.enabled = previous;
        return clicked;
    }
}
