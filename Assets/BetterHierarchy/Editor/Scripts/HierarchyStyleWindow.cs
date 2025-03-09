using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BetterHierarchy
{
    public class HierarchyStyleWindow : EditorWindow
    {

        private static Color selectedColor;
        private static string selectedIcon;
        private static System.Action<Color, string> onApply;

        private const int ButtonSize = 32;

        private static List<string> iconNames = new List<string>
        {
            "GameObject Icon",
            "Camera Icon",
            "Light Icon",
            "AudioSource Icon",
            "cs Script Icon",
            "Material Icon",
            "Prefab Icon",
            "Animation Icon",
            "MeshRenderer Icon",
            "Rigidbody Icon",
            "ParticleSystem Icon",
            "SceneAsset Icon",
        };

        private Vector2 scrollPosition;

        public static void Show(Color color, string icon, System.Action<Color, string> callback)
        {
            onApply = callback;
            selectedColor = color;
            selectedIcon = icon;

            var window = GetWindow<HierarchyStyleWindow>(true, "Set Style", true);
            window.minSize = new Vector2(300, 400);
        }

        private void OnGUI()
        {

            GUILayout.Label("Set Style", EditorStyles.boldLabel);

            GUILayout.Space(10);
            GUILayout.Label("Color:", EditorStyles.label);
            selectedColor = EditorGUILayout.ColorField(selectedColor);

            GUILayout.Space(10);
            GUILayout.Label("Icon:", EditorStyles.label);

            Texture[] contents = new Texture[iconNames.Count];

            for (int i = 0; i < iconNames.Count; i++)
            {
                string entry = iconNames[i];
                Texture icon;

                if (entry.Contains("/"))
                {
                    icon = EditorGUIUtility.FindTexture(entry);
                }
                else
                {
                    icon = EditorGUIUtility.IconContent(entry).image;
                }

                if (icon != null)
                {
                    contents[i] = icon;
                }
            }

            int columns = Mathf.FloorToInt(position.width / (ButtonSize + 4));
            if (columns < 1) columns = 1;

            int count = 0;
            GUILayout.BeginHorizontal();

            foreach (var entry in iconNames)
            {
                Texture icon;

                if (entry.Contains("/"))
                {
                    icon = EditorGUIUtility.FindTexture(entry);
                }
                else
                {
                    icon = EditorGUIUtility.IconContent(entry).image;
                }

                if (GUILayout.Button(icon, GUILayout.Width(ButtonSize), GUILayout.Height(ButtonSize)))
                {
                    selectedIcon = entry;
                }

                count++;
                if (count % columns == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            if(GUILayout.Button("Reset"))
            {
                selectedColor = Color.clear;
                selectedIcon = "";
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Apply"))
            {
                onApply?.Invoke(selectedColor, selectedIcon);
                Close();
            }

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
        }
    }
}