using System.Collections;
using UnityEditor;
using UnityEngine;

namespace EditorExtensions
{
    public class FileTreeCreator : EditorWindow
    {
        private static FileTreeCreator instance = new FileTreeCreator();

        private static FileTreeCreator window;

        private static SortedList fileMap;

        private static FileNode root;

        private FileNode selectedNode;

        private Vector2 scrollPos;

        [MenuItem("Tools/创建文件路径树")]
        private static void Init()
        {
            window = GetWindow<FileTreeCreator>();
            window.titleContent = new GUIContent("文件路径树");
            window.Show();
            instance.GetTreeRoot();
        }

        private void GetTreeRoot()
        {
            fileMap = FilePathReader.fileMap;
            FilePathReader.CreateSelectedFilePathTree();
            root = fileMap.GetByIndex(0) as FileNode;
            selectedNode = null;
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("根路径：", GUILayout.Width(44), GUILayout.Height(20));
            string path = (root.path + "\\" + root.name).Replace('/', '\\');
            EditorGUILayout.SelectableLabel(path, GUILayout.Height(20));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("当前路径：", GUILayout.Width(55), GUILayout.Height(20));
            if (selectedNode != null)
            {
                path = (selectedNode.path + "\\" + selectedNode.name).Replace('/', '\\');
                EditorGUILayout.SelectableLabel(path, GUILayout.Height(20));
            }
            else
            {
                EditorGUILayout.LabelField("未选中任何文件");
            }
            GUILayout.EndHorizontal();

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            DrawTree(root, 0);
            GUILayout.EndScrollView();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("刷新", "LargeButton", GUILayout.Width(120f)))
            {
                GetTreeRoot();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }

        private void DrawTree(FileNode node, int level)
        {
            if(node == null)
            {
                return;
            }

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            if(node == selectedNode)
            {
                style.normal.textColor = Color.green;
            }

            EditorGUILayout.BeginHorizontal();
            if (node.isDirectory)
            {
                GUILayout.Space(5 + level * 10);
                node.isOpen = EditorGUILayout.Foldout(node.isOpen, node.name);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Space(7 + level * 10);
                if (GUILayout.Button(node.name, style))
                {
                    selectedNode = node;
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);
            }

            if (!node.isOpen || node.children == null || node.children.Count == 0)
            {
                return;
            }

            IList list = node.children.GetValueList();
            for(int i = 0; i < list.Count; i++)
            {
                DrawTree(list[i] as FileNode, level + 1);
            }
        }
    }
}