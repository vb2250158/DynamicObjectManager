using UnityEditor;
using UnityEngine;

namespace DynamicObjectManager.Editor
{
    public class DynamicObjectWindow : EditorWindow
    {
        [MenuItem("EgoWindow/DynamicObjectWindow #D")]
        public static void OnShow()
        {
            EditorWindow.GetWindow<DynamicObjectWindow>().Show();
        }

        public static string Url = "";
        private Vector2 _scrollPosition;
        private Vector2 _scrollPos;
        [SerializeField] private string t;

        [InitializeOnLoadMethod]
        public static void OnLoadEditor()
        {
            EditorApplication.wantsToQuit -= Quit;
            EditorApplication.wantsToQuit += Quit;

            Url = PlayerPrefs.GetString("DynamicObjectConfigUrl");
            DynamicObjectEditor.DynamicObjectConfig = AssetDatabase.LoadAssetAtPath<DynamicObjectConfig>(Url);
        }

        private static bool Quit()
        {
            Url = AssetDatabase.GetAssetPath(DynamicObjectEditor.DynamicObjectConfig);
            PlayerPrefs.SetString("DynamicObjectConfigUrl", Url);
            return true;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            DynamicObjectEditor.DynamicObjectConfig =
                EditorGUILayout.ObjectField("Config", DynamicObjectEditor.DynamicObjectConfig,
                        typeof(DynamicObjectConfig)) as
                    DynamicObjectConfig;
            EditorGUILayout.Space();
            var beginScrollView = EditorGUILayout.BeginScrollView(_scrollPosition, EditorStyles.helpBox);

            _scrollPosition = beginScrollView;
            if (DynamicObjectEditor.DynamicObjectConfig != null)
            {
                for (int i = 0; i < DynamicObjectEditor.DynamicObjectConfig.datas.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.textArea);
                    DynamicObjectData dynamicObjectEditorData =
                        DynamicObjectEditor.DynamicObjectConfig.GetValueByIndex(i);
                    string key = dynamicObjectEditorData.key;
                    GUILayout.Label(key);
                    if (GUILayout.Button("Remove", GUILayout.Width(100)))
                    {
                        DynamicObjectEditor.Remove(key);
                        i--;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}