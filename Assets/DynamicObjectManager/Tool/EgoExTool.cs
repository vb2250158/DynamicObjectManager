using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EgoTool
{
    /// <summary>
    /// Unity可序列化的字典
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class UnityDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField] protected List<TKey> _keys = new List<TKey>();
        [SerializeField] protected List<TValue> _values = new List<TValue>();

        protected Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (!_dictionary.ContainsKey(key))
                {
                    return default(TValue);
                }

                return _dictionary[key];
            }
            set { _dictionary[key] = value; }
        }

        public List<TKey> GetKey()
        {
            return _keys;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            _keys.Add(key);
            _values.Add(value);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        public void Remove(TKey key)
        {
            _values.Remove(this[key]);
            _keys.Remove(key);
            _dictionary.Remove(key);
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="key"></param>
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 当反序列化时,列表 To 字典(字符串转对象)
        /// </summary>
        public void OnAfterDeserialize()
        {
            _dictionary = new Dictionary<TKey, TValue>();

            for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++) _dictionary.Add(_keys[i], _values[i]);
        }

        /// <summary>
        /// 当序列化时,字典 To 列表(对象转字符串)
        /// </summary>
        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (var kvp in _dictionary)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }
    }
#if UNITY_EDITOR
    public class StringPopupExDawer : PropertyDrawer
    {
        const string NONE = "[NONE]";

//    public List<DrawerValuePair> drawerValuePairs;
        public string textInput;
        public bool isOpenInput;

//        public string searchText;
        private List<string> layers = new List<string>();
//        private static  GUIStyle toolbarSearchField;
//
//        public static GUIStyle ToolbarSearchField
//        {
//            get
//            {
//                if (toolbarSearchField==null)
//                {
//                    var propertyInfo = typeof(EditorStyles).GetProperty("toolbarSearchField",
//                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
//                    
//                    toolbarSearchField =propertyInfo.GetValue(null) as GUIStyle;
//                }
//                return toolbarSearchField;
//            }
//        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, "ERROR:", "May only apply to type  string");
                return;
            }

            EditorGUI.PrefixLabel(position, label);
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            if (isOpenInput)
            {
                //新建字段框
                textInput = EditorGUILayout.TextField(textInput, EditorStyles.textField);
                if (IsClickReturn())
                {
                    AddKey(property);
                }
            }
            else
            {
                //查询框
//                searchText = GUILayout.TextField(searchText,ToolbarSearchField,GUILayout.MinWidth(70));

                string value = property.stringValue;

                if (GUILayout.Button(value, EditorStyles.popup) ||
                    (IsClickReturn() && Event.current.modifiers == EventModifiers.Control))
                {
                    Selector(property);
                }
            }

            //添加字段的按钮
            if (GUILayout.Button("Add", GUILayout.Width(50)))
            {
                AddKey(property);
            }

            EditorGUILayout.EndHorizontal();
        }

        private static bool IsClickReturn()
        {
            return Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return;
        }

        private void AddKey(SerializedProperty property)
        {
            if (isOpenInput)
            {
                OnAddString(textInput);
                property.stringValue = textInput;
            }
            else
            {
                textInput = property.stringValue;
            }

            isOpenInput = !isOpenInput;
        }

        void Selector(SerializedProperty property)
        {
            List<string> stringList = GetStringList();
            layers.Clear();
            foreach (var item in stringList)
            {
//                if (string.IsNullOrEmpty(searchText) || item.StartsWith(searchText))
//                {
                    layers.Add(item);
//                }
            }

            layers.Insert(0, "");

            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < layers.Count; i++)
            {
                string name = layers[i];
                if (string.IsNullOrEmpty(name))
                {
                    name = NONE;
                }

                menu.AddItem(new GUIContent(name), name == property.stringValue, HandleSelect,
                    new DrawerValuePair(name, property));
            }

            menu.ShowAsContext();
        }

        void HandleSelect(object val)
        {
            var pair = (DrawerValuePair) val;
            string str = pair.str;
            if (str == NONE)
            {
                str = "";
            }

            pair.property.stringValue = str;
            pair.property.serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnAddString(string str)
        {
        }

        protected virtual List<string> GetStringList()
        {
            return new List<string>();
        }

        public struct DrawerValuePair
        {
            public string str;
            public SerializedProperty property;

            public DrawerValuePair(string val, SerializedProperty property)
            {
                this.str = val;
                this.property = property;
            }
        }
    }
#endif
}