using UnityEditor;
using UnityEngine;

namespace EditorExtensions
{
    public class LayoutTest : ScriptableWizard
    {
        private static LayoutTest window;

        private Vector2 firstPos;

        private Vector2 secondPos;

        private bool isExpand;

        [MenuItem("Tools/窗口布局测试/展开折叠")]
        public static void Init()
        {
            window = DisplayWizard<LayoutTest>("展开折叠");
            window.maxSize = window.minSize = new Vector2(400, 400);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            GUILayout.Space(10);

            firstPos = EditorGUILayout.BeginScrollView(firstPos, GUILayout.Width(340), GUILayout.ExpandWidth(false));
            GUILayout.Button("左侧内容", GUILayout.Width(2000), GUILayout.Height(500), GUILayout.ExpandWidth(false));
            EditorGUILayout.EndScrollView();

            GUILayout.Space(5);
            if (!isExpand)
            {
                if (GUILayout.Button("展开", GUILayout.Width(40), GUILayout.MaxHeight(2000), GUILayout.ExpandWidth(false)))
                {
                    isExpand = true;
                    window.maxSize = window.minSize = new Vector2(800, 400);
                }
            }
            else
            {
                if (GUILayout.Button("折叠", GUILayout.Width(40), GUILayout.MaxHeight(2000), GUILayout.ExpandWidth(false)))
                {
                    isExpand = false;
                    window.maxSize = window.minSize = new Vector2(400, 400);
                }

                GUILayout.Space(5);
                secondPos = EditorGUILayout.BeginScrollView(secondPos, GUILayout.Width(390), GUILayout.MaxWidth(2000), GUILayout.ExpandWidth(false));
                GUILayout.Button("右侧内容", GUILayout.Width(2000), GUILayout.Height(500), GUILayout.ExpandWidth(false));
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
