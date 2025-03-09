using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BetterHierarchy
{

    public class HierarchyData : ScriptableObject
    {
        public Dictionary<string, GameObjectData> data = new Dictionary<string, GameObjectData>();
        [SerializeField] private List<HierarchyDataData> dataList = new List<HierarchyDataData>();

        public void Save()
        {
            dataList.Clear();

            foreach (var objectData in data)
            {
                Debug.Log(objectData.ToString());
                dataList.Add(new HierarchyDataData() { name = objectData.Key.ToString(), color = objectData.Value.color, iconName = objectData.Value.iconName });
            }
            EditorUtility.SetDirty(this);
            Debug.Log("Saved");
        }

        public void Load()
        {
            data.Clear();

            foreach (var objectData in dataList)
            {
                data.Add(objectData.name, new GameObjectData() { color = objectData.color, iconName = objectData.iconName });
            }
        }

        public void Set(string key, Color color, string iconName)
        {
            if(data.ContainsKey(key))
            {
                data[key] = new GameObjectData() { color = color, iconName = iconName };
            }
            else
            {
                data.Add(key, new GameObjectData() { color = color, iconName = iconName });
            }
        }
    }

    [System.Serializable]
    public class GameObjectData
    {
        public Color color = Color.clear;
        public string iconName = null;
    }

    [System.Serializable]
    public struct HierarchyDataData
    {
        public string name;
        public Color color;
        public string iconName;
    }

}
