using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BetterHierarchy
{
    [InitializeOnLoad]
    public static class CustomHierarchyEditor
    {
        private static Color DefaultBackgroundColor = new Color(0.2196078431372549f, 0.2196078431372549f, 0.2196078431372549f);
        private static HierarchyData styleDatabase;

        static CustomHierarchyEditor()
        {
            LoadDatabase();
            EditorSceneManager.sceneOpened += (old, newScene) => LoadDatabase();

            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static void LoadDatabase()
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string path = $"Assets/BetterHierarchy/Editor/Data/HierarchyStyleDatabase_{sceneName}.asset";

            styleDatabase = AssetDatabase.LoadAssetAtPath<HierarchyData>(path);

            if (styleDatabase == null)
            {
                styleDatabase = ScriptableObject.CreateInstance<HierarchyData>();
                AssetDatabase.CreateAsset(styleDatabase, path);
                AssetDatabase.SaveAssets();
            }

            styleDatabase.Load();
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            if (styleDatabase == null)
            {
                LoadDatabase();
                return;
            }

            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null) return;

            string key = obj.name;

            Color objectColor = styleDatabase.data.ContainsKey(key) ? styleDatabase.data[key].color : Color.clear;

            Rect iconRect = new Rect(selectionRect.x, selectionRect.y, 16, 16);

            EditorGUI.DrawRect(iconRect, DefaultBackgroundColor);

            if (objectColor != Color.clear)
            {
                EditorGUI.DrawRect(selectionRect, objectColor);
            }

            string iconName = styleDatabase.data.ContainsKey(key) && !string.IsNullOrEmpty(styleDatabase.data[key].iconName)
                ? styleDatabase.data[key].iconName
                : "GameObject Icon";

            Texture icon;

            if (iconName.Contains("/"))
            {
                icon = EditorGUIUtility.FindTexture(iconName);
            }
            else
            {
                icon = EditorGUIUtility.IconContent(iconName).image;
            }

            if (icon != null)
            {
                GUI.DrawTexture(iconRect, icon);
            }

            if (Event.current.type == EventType.ContextClick && selectionRect.Contains(Event.current.mousePosition) && Event.current.alt)
            {
                OpenStyleEditor(obj);
            }
        }

        public static void OpenStyleEditor(GameObject obj)
        {
            string key = obj.name;

            Color color;
            string icon;

            if (styleDatabase.data.ContainsKey(key))
            {
                color = styleDatabase.data[key].color;
                icon = styleDatabase.data[key].iconName;
            }
            else
            {
                color = new Color(1, 1, 1, 0.4f);
                icon = "GameObject Icon";
            }

            HierarchyStyleWindow.Show(color, icon, (newColor, newIcon) =>
            {
                styleDatabase.Set(key, newColor, newIcon);
                styleDatabase.Save();
                EditorUtility.SetDirty(styleDatabase);
                AssetDatabase.SaveAssets();
                EditorApplication.RepaintHierarchyWindow();
            });
        }
    }
}