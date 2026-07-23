using UnityEngine;
using RapidGUI;
using System;

namespace ReplayTestMod.Utils
{
    public static class UIextensions
    {
        public static Color ColorSwitch(bool toggle, Color color1, Color color2)
        {
            if (toggle)
            {
                return color1;
            }
            else
            {
                return color2;
            }
        }
        public static void CenteredLabel(string label)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"<i><b>{label}</b></i>", GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        public static void FlexableButton(string label, Action buttonAction, Color color)
        {
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = color;
            if (GUILayout.Button($"{label}", RGUIStyle.button, GUILayout.ExpandWidth(true)))
            {
                buttonAction?.Invoke();
            }
            GUILayout.EndHorizontal();
        }
        public static void StandardButton(string label, Action buttonAction, Color color, int width)
        {
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = color;
            if (GUILayout.Button($"{label}", GUILayout.MaxWidth(width)))
            {
                buttonAction?.Invoke();
            }
            GUILayout.EndHorizontal();
        }
    }
}
