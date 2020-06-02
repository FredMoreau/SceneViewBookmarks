using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if CINEMACHINE
using Cinemachine;
#endif

// TODO : move to Project Settings
// TODO : use UIElements and reorderable list

namespace UnityEditor.SceneViewBookmarks
{
    public class BookmarkEditorWindow : EditorWindow
    {
        public static void Open()
        {
            BookmarkEditorWindow window = GetWindow<BookmarkEditorWindow>();
            window.titleContent = new GUIContent("Bookmark Editor");
        }

        void OnGUI()
        {
            if (Bookmarks.Count == 0)
                return;

            GUILayout.BeginVertical();
            for (int i = 0; i < Bookmarks.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("[]", GUILayout.Width(32)))
                    Bookmarks.Instance[i].Load(SceneView.lastActiveSceneView);

                Bookmarks.Instance[i].name = EditorGUILayout.TextField(Bookmarks.Instance[i].name);
                Bookmarks.Instance[i].staysInPlace = EditorGUILayout.Toggle(Bookmarks.Instance[i].staysInPlace);
                Bookmarks.Instance[i].visibleLayers = EditorGUILayout.MaskField("Visible", Bookmarks.Instance[i].visibleLayers, UnityEditorInternal.InternalEditorUtility.layers);
                Bookmarks.Instance[i].lockedLayers = EditorGUILayout.MaskField("Locked", Bookmarks.Instance[i].lockedLayers, UnityEditorInternal.InternalEditorUtility.layers);

                if (GUILayout.Button("~", GUILayout.Width(32)))
                    Bookmarks.Instance[i].Save(SceneView.lastActiveSceneView);

                if (GUILayout.Button("up", GUILayout.Width(64)))
                {
                    if (i <= 0)
                        return;

                    var item = Bookmarks.Instance.viewpoints[i];
                    Bookmarks.Instance.viewpoints.RemoveAt(i);

                    Bookmarks.Instance.viewpoints.Insert(i-1, item);
                }

                if (GUILayout.Button("down", GUILayout.Width(64)))
                {
                    if (i >= Bookmarks.Instance.viewpoints.Count-1)
                        return;

                    var item = Bookmarks.Instance.viewpoints[i];
                    Bookmarks.Instance.viewpoints.RemoveAt(i);

                    Bookmarks.Instance.viewpoints.Insert(i + 1, item);
                }

                if (GUILayout.Button("x", GUILayout.Width(32)))
                    Bookmarks.Instance.viewpoints.RemoveAt(i);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}