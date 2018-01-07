using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EditorExtensions
{
    public class FileNode
    {
        public string path;
        public string name;
        public string extension;
        public bool isDirectory;
        public SortedList children;

        public bool isOpen;         //节点是否展开
    }

    public class FilePathReader : Editor
    {
        public static SortedList fileMap = new SortedList();

        public static void CreateSelectedFilePathTree()
        {
            fileMap.Clear();
            string fullPath;
            string fileName;
            string rootPath;
            Object[] selection = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            if (selection.Length == 1)
            {
                fullPath = AssetDatabase.GetAssetPath(selection[0]);
                fileName = Path.GetFileName(fullPath);
                rootPath = fullPath.Remove(fullPath.Length - fileName.Length - 1);
                FileNode root = new FileNode();
                root.path = rootPath;
                root.name = fileName;
                if (!Directory.Exists(fullPath))                    //选中单一文件
                {
                    root.extension = Path.GetExtension(fullPath);
                }
                else                                                //选中空文件夹
                {
                    root.isDirectory = true;
                }
                fileMap[fullPath] = root;
            }
            else if (selection.Length > 1)
            {
                bool isSelectTopAssets = false;
                for (int i = 0; i < selection.Length; i++)
                {
                    fullPath = AssetDatabase.GetAssetPath(selection[i]);
                    fileName = Path.GetFileName(fullPath);
                    if (fullPath == "Assets")
                    {
                        isSelectTopAssets = true;
                        continue;
                    }
                    else
                    {
                        rootPath = fullPath.Remove(fullPath.Length - fileName.Length - 1);
                    }

                    FileNode node;
                    if (Directory.Exists(fullPath) && fileMap.ContainsKey(fullPath))
                    {
                        node = fileMap[fullPath] as FileNode;
                    }
                    else
                    {
                        node = new FileNode();
                        node.name = fileName;
                    }

                    FileNode parent;
                    if (fileMap.ContainsKey(rootPath))          //已经包含路径节点
                    {
                        parent = fileMap[rootPath] as FileNode;

                        if (parent.children == null)
                        {
                            parent.children = new SortedList();
                        }
                    }
                    else                                        //不包含路径节点，创建并合并
                    {
                        parent = new FileNode();
                        parent.name = Path.GetFileName(rootPath);
                        if (rootPath == "Assets")
                        {
                            parent.path = Application.dataPath.Remove(Application.dataPath.Length - 7);
                        }
                        else
                        {
                            parent.path = rootPath.Remove(rootPath.Length - parent.name.Length - 1);
                        }
                        parent.isDirectory = true;
                        parent.children = new SortedList();
                        fileMap[rootPath] = parent;
                    }

                    node.path = rootPath;
                    if (!Directory.Exists(fullPath))
                    {
                        node.extension = Path.GetExtension(fullPath);
                    }
                    else
                    {
                        node.isDirectory = true;
                        fileMap[fullPath] = node;
                    }
                    parent.children[fileName] = node;
                }

                //移除选中路径节点的父节点
                if (!isSelectTopAssets)
                {
                    fileMap.RemoveAt(0);
                }
            }

            IList pathList = fileMap.GetKeyList();
            FileNode current = fileMap[pathList[0]] as FileNode;
            ShowAllPath(current);
        }

        private static void ShowAllPath(FileNode node)
        {
            //Debug.Log(node.path + "/" + node.name);
            if (node.children != null)
            {
                IList list = node.children.GetKeyList();
                for (int i = 0; i < list.Count; i++)
                {
                    FileNode current = node.children[list[i]] as FileNode;
                    ShowAllPath(current);
                }
            }
        }
    }
}