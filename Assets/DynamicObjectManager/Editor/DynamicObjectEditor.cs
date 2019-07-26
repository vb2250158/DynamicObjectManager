using System.Collections.Generic;
using EgoTool;
using UnityEditor;
using UnityEngine;

namespace DynamicObjectManager.Editor
{
    
    [CustomEditor(typeof(DynamicObject))]
    public class DynamicObjectEditor : UnityEditor.Editor
    {
        public static DynamicObjectConfig DynamicObjectConfig
        {
            get { return DynamicObjectController.DynamicObjectConfig; }
            set { DynamicObjectController.DynamicObjectConfig = value; }
        }

        private void OnSceneGUI()
        {
            DynamicObject dynamicObject = target as DynamicObject;
        }

        private void OnEnable()
        {
            DynamicObject dynamicObject = target as DynamicObject;
            Debug.Log(dynamicObject.key);
        }


        public static void AddKey(string key)
        {
            DynamicObjectConfig.AddKey(key);
        }

        public static List<string> GetKeys()
        {
            return DynamicObjectConfig.GetKeys();
        }

        public static void Remove(string key)
        {
            DynamicObjectConfig.Remove(key);
        }
    }

    [CustomPropertyDrawer(typeof(DynamicObjectKeyAttribute))]
    public class DynamicObjectrExDawer : StringPopupExDawer
    {
        protected override void OnAddString(string str)
        {
            DynamicObjectEditor.AddKey(str);
        }

        protected override List<string> GetStringList()
        {
            return DynamicObjectEditor.GetKeys();
        }
    }

}