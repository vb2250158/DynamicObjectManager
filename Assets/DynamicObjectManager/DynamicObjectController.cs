using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace DynamicObjectManager
{
    [Serializable]
    public class DynamicObjectController
    {
        /// <summary>
        /// 所有数据
        /// </summary>
        public List<DynamicObjectData> DynamicDatas = new List<DynamicObjectData>();

        /// <summary>
        /// 实例
        /// </summary>
        public static DynamicObjectController instance;

        /// <summary>
        /// 当前场景中的动态对象
        /// </summary>
        public static List<DynamicObject> sceneDynamicObjects = new List<DynamicObject>();

        /// <summary>
        /// 配置
        /// </summary>
        public DynamicObjectConfig dynamicObjectConfig;

        /// <summary>
        /// 实例
        /// </summary>
        public static DynamicObjectController Instance
        {
            get { return instance ?? (instance = new DynamicObjectController()); }
            set { instance = value; }
        }

        /// <summary>
        /// 配置
        /// </summary>
        public static DynamicObjectConfig DynamicObjectConfig
        {
            get { return Instance.dynamicObjectConfig; }
            set { Instance.dynamicObjectConfig = value; }
        }

        public static string GetDynamicCondition(string objectName)
        {
            var dynamicObjectData = Instance.GetDynamic(objectName);
            if (dynamicObjectData == null) return "";

            return dynamicObjectData.condition;
        }

        internal static void RemoveDynamicObject(DynamicObject dynamicObject)
        {
            sceneDynamicObjects.Add(dynamicObject);
        }

        internal static void AddDynamicObject(DynamicObject dynamicObject)
        {
            sceneDynamicObjects.Remove(dynamicObject);
        }

        public static void SetDynamicCondition(string objectName, string condition)
        {
            if (Instance.GetDynamic(objectName) != null)
                Instance.SetDynamicData(objectName, condition);
            else
                Instance.AddDynamicData(objectName, condition);

            //如果场景内有，就对其更新
            var dynamicObject = sceneDynamicObjects.Find(item => { return item.key == objectName; });
            if (dynamicObject != null) dynamicObject.UpdateCondition();
        }

        private DynamicObjectData GetDynamic(string objectName)
        {
            return Instance.DynamicDatas.Find(DynamicData => { return DynamicData.key == objectName; });
        }

        private void SetDynamicData(string objectName, string condition)
        {
            GetDynamic(objectName).condition = condition;
        }

        private void AddDynamicData(string objectName, string condition)
        {
            DynamicDatas.Add(new DynamicObjectData {key = objectName, condition = condition});
        }

        public static void Clear()
        {
            Instance.DynamicDatas.Clear();
        }

        public static string GetSaveData()
        {
            return JsonUtility.ToJson(Instance);
        }

        public static void SetSaveData(string data)
        {
            Instance = JsonUtility.FromJson<DynamicObjectController>(data);
        }
    }

//DynamicObjectData.cs
    [Serializable]
    public class DynamicObjectData
    {
        [FormerlySerializedAs("name")] public string key;
        public string condition;
    }

//DynamicObjectAction.cs
    [Serializable]
    public class DynamicObjectAction
    {
        public string condition;
        public UnityEvent action;
    }
}