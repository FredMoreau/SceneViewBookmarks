//#define USE_UI_ELEMENTS

using System.Collections.Generic;
using UnityEngine;
#if USE_UI_ELEMENTS
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#endif

namespace UnityEditor.SceneViewBookmarks
{
    static class DefaultBookmarksSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider InitSettingsProvider()
        {
            var provider = new SettingsProvider("Preferences/Scene View/Bookmarks", SettingsScope.User)
            {
                label = "Bookmarks",
#if !USE_UI_ELEMENTS
                guiHandler = (searchContext) =>
                {
                    var settings = DefaultBookmarks.GetSerializedSettings();
                    EditorGUILayout.PropertyField(settings.FindProperty("viewpoints"), new GUIContent("Viewpoints"));
                },
#else
                activateHandler = (searchContext, rootElement) =>
                {
                    var vpp = DefaultBookmarks.GetSerializedSettings();

                    var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.unity.sceneview.bookmarks/Editor/SceneViewBookmarks.Styles.uss");
                    rootElement.styleSheets.Add(styleSheet);
                    var title = new Label()
                    {
                        text = "-default-"
                    };
                    title.AddToClassList("title");
                    rootElement.Add(title);

                    var properties = new VisualElement()
                    {
                        style =
                        {
                            flexDirection = FlexDirection.Column
                        }
                    };
                    properties.AddToClassList("property-list");
                    rootElement.Add(properties);

                    var vp = new PropertyField(vpp.FindProperty("viewpoints"));
                    //vp.AddToClassList("property-value");
                    properties.Add(vp);

                },
#endif
                keywords = new HashSet<string>(new[] { "SceneView", "Bookmarks" })
            };

            return provider;
        }
    }
}