using System.Collections;
using UnityEditor;
using UnityEngine;

namespace EditorExtensions
{
    public class FileTreeCreator : EditorWindow
    {
        private static FileTreeCreator instance;

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
            if(instance == null)
            {
                instance = CreateInstance<FileTreeCreator>();
            }
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
            if(root == null)
            {
                return;
            }
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUIStyle style = new GUIStyle();
            style.fontSize = 12;
            style.normal.textColor = Color.white;

            EditorGUILayout.LabelField("根路径：", style, GUILayout.Width(50), GUILayout.Height(20));
            string path = (root.path + "\\" + root.name).Replace('/', '\\');
            EditorGUILayout.SelectableLabel(path, style, GUILayout.Height(20));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("当前路径：", style, GUILayout.Width(62), GUILayout.Height(20));
            if (selectedNode != null)
            {
                path = (selectedNode.path + "\\" + selectedNode.name).Replace('/', '\\');
                EditorGUILayout.SelectableLabel(path, style, GUILayout.Height(20));
            }
            else
            {
                EditorGUILayout.LabelField("未选中任何文件", style);
            }
            GUILayout.EndHorizontal();

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUILayout.Space(5);
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
            style.fontSize = 12;
            if (node == selectedNode)
            {
                style.normal.textColor = Color.green;
            }

            EditorGUILayout.BeginHorizontal();
            if (node.isDirectory)
            {
                GUILayout.Space(5 + level * 10);
                //node.isOpen = EditorGUILayout.Foldout(node.isOpen, node.name);
                if (node.isOpen)
                {
                    if (GUILayout.Button("-", GUILayout.Width(18), GUILayout.Height(14)))
                    {
                        node.isOpen = false;
                    }
                }
                else
                {
                    if (GUILayout.Button("+", GUILayout.Width(18), GUILayout.Height(14)))
                    {
                        node.isOpen = true;
                    }
                }
                GUILayout.Space(2);
                if (GUILayout.Button(node.name, style))
                {
                    selectedNode = node;
                }
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
            }
            GUILayout.Space(8);

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