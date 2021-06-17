// Shameless rip from the Live Capture package
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEditor.SceneViewBookmarks
{
    class SceneViewProjectBookmarksSettingsProvider : SettingsProvider
    {
        const string k_SettingsMenuPath = "Project/Scene Bookmarks";

        static class Contents
        {
            public static readonly GUIContent SettingMenuIcon = EditorGUIUtility.IconContent("_Popup");
            public static readonly GUIContent TakeNameFormatLabel = EditorGUIUtility.TrTextContent("Take Name Format", "The format of the file name of the output take.");
            public static readonly GUIContent AssetNameFormatLabel = EditorGUIUtility.TrTextContent("Asset Name Format", "The format of the file name of the generated assets.");
            public static readonly GUIContent ResetLabel = EditorGUIUtility.TrTextContent("Reset", "Reset to default.");
        }

        SerializedObject m_SerializedObject;
        SerializedProperty m_TakeNameFormatProp;
        SerializedProperty m_AssetNameFormatProp;

        /// <summary>
        /// Open the settings in the Project Settings.
        /// </summary>
        public static void Open()
        {
            SettingsService.OpenProjectSettings(k_SettingsMenuPath);
        }

        public SceneViewProjectBookmarksSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_SerializedObject = new SerializedObject(SceneViewProjectBookmarksAsset.Instance);
            m_TakeNameFormatProp = m_SerializedObject.FindProperty("m_TakeNameFormat");
            m_AssetNameFormatProp = m_SerializedObject.FindProperty("m_AssetNameFormat");
        }

        public override void OnDeactivate()
        {
        }

        public override void OnTitleBarGUI()
        {
            if (EditorGUILayout.DropdownButton(Contents.SettingMenuIcon, FocusType.Passive, EditorStyles.label))
            {
                var menu = new GenericMenu();
                menu.AddItem(Contents.ResetLabel, false, reset =>
                {
                    SceneViewProjectBookmarksAsset.Instance.Reset();
                    SceneViewProjectBookmarksAsset.Save();
                }, null);
                menu.ShowAsContext();
            }
        }

        public override void OnGUI(string searchContext)
        {
            m_SerializedObject.Update();

            using (var change = new EditorGUI.ChangeCheckScope())
            using (new SettingsWindowGUIScope())
            {
                EditorGUILayout.PropertyField(m_TakeNameFormatProp, Contents.TakeNameFormatLabel);
                EditorGUILayout.PropertyField(m_AssetNameFormatProp, Contents.AssetNameFormatLabel);

                if (change.changed)
                {
                    m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
                    SceneViewProjectBookmarksAsset.Save();
                }
            }
        }

        public override void OnFooterBarGUI()
        {
        }

        public override void OnInspectorUpdate()
        {
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SceneViewProjectBookmarksSettingsProvider(
                k_SettingsMenuPath,
                SettingsScope.Project,
                GetSearchKeywordsFromSerializedObject(new SerializedObject(SceneViewProjectBookmarksAsset.Instance))
            );
        }
    }
}