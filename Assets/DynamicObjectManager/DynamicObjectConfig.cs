using System;
using System.Collections.Generic;
using DynamicObjectManager;
using EgoTool;
using UnityEditor;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;

namespace DynamicObjectManager
{
    [Serializable]
    public class DynamicObjectDictionary : UnityDictionary<string, DynamicObjectData>
    {
    }

    [CreateAssetMenu(fileName = "DynamicObjectConfig", menuName = "DynamicObject/Conifg")]
    public class DynamicObjectConfig : ScriptableObject
    {
        [HideInInspector] public DynamicObjectDictionary datas;

        public void AddKey(string key)
        {
            if (!datas.ContainsKey(key))
            {
                datas.Add(key, new DynamicObjectData());
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

        public void Remove(string key)
        {
            datas.Remove(key);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public List<string> GetKeys()
        {
            foreach (var item in datas.GetKey())
            {
                datas[item].key = item;
            }

            return datas.GetKey();
        }

        public DynamicObjectData GetValueByIndex(int index)
        {
            return datas[GetKeys()[index]];
        }
    }
}