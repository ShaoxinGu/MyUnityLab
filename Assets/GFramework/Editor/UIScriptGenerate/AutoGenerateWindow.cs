using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.IO;
using UnityEngine.SceneManagement;
using System;

namespace GFramework.Editor.AutoGenerateScript
{

    public static class Json
    {
        public static T Parse<T>(string jsonString)
        {
            using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonString)))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        public static string ToString(object jsonObject)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                return System.Text.Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }

    public class UiJson
    {
        public string SaveDirName;
        public bool IsChildUi;
        public string ParentName;
        public UiNode UiNodeData;
    }

    public class AutoGenerateWindow : EditorWindow
    {
        static string CACHE_FILE_PATH = "Assets/UI/Cache/{filename}.json";

        private static GameObject rootGameObject;
        private static string SaveDirName;
        private static string SceneName;
        private static string ClassName;
        private static UiNode NewUiNode;
        private static UiNode ShowUiNode;
        private static AutoGenerateWindow Instance;

        static bool IsChildUi;
        static string ParentUiName;
        static bool IsChildUiSetGameObject;

        static TResult GetCacheData<TResult>(string className)
        {
            string cachePath = CACHE_FILE_PATH.Replace("{filename}", className);
            string cacheStr = "";
            if (File.Exists(cachePath))
            {
                FileStream file = new FileStream(cachePath, FileMode.Open, FileAccess.ReadWrite);
                StreamReader read = new StreamReader(file);
                cacheStr = read.ReadToEnd();
                read.Close();
                file.Close();
            }

            if (string.IsNullOrEmpty(cacheStr))
            {
                return default(TResult);
            }
            else
            {
                return Json.Parse<TResult>(cacheStr);
            }
        }

        public static void SaveCacheData(string className, UiJson jsonData)
        {
            string cachePath = CACHE_FILE_PATH.Replace("{filename}", className);
            File.WriteAllText(cachePath, Json.ToString(jsonData), System.Text.Encoding.UTF8);
        }

        static void CheckUiNode(UiNode parentNode, UiNode cacheNode, UiNode newNode)
        {
            if (parentNode != null)
            {
                // 已删节点
                if (newNode == null && cacheNode != null)
                {
                    // 兼容旧版节点配置
                    if (string.IsNullOrEmpty(cacheNode.Key))
                    {
                        cacheNode.Key = cacheNode.Name;
                    }
                    parentNode.Children.Remove(cacheNode.Key);
                }

                // 新增节点
                if (newNode != null && cacheNode == null)
                {
                    parentNode.Children.Add(newNode.Key, newNode);
                }
            }

            // 修改节点
            if (newNode != null && cacheNode != null)
            {
                cacheNode.Type = newNode.Type;
                cacheNode.Name = newNode.Name;
                cacheNode.MidPath = newNode.MidPath;
                //cacheNode.Is3dUi = newNode.Is3dUi;

                if (cacheNode.Children.Count > 0)
                {
                    List<UiNode> rmList = new List<UiNode>();
                    foreach (var UiNode in cacheNode.Children)
                    {
                        UiNode tmpNode = null;
                        if (newNode.Children.Count > 0 && newNode.Children.TryGetValue(UiNode.Key, out tmpNode))
                        {
                            CheckUiNode(cacheNode, UiNode.Value, tmpNode);
                        }
                        else
                        {
                            rmList.Add(UiNode.Value);
                        }
                    }

                    foreach (var UiNode in rmList)
                    {
                        CheckUiNode(cacheNode, UiNode, null);
                    }
                }

                foreach (var UiNode in newNode.Children)
                {
                    if (!cacheNode.Children.ContainsKey(UiNode.Key))
                    {
                        CheckUiNode(cacheNode, null, UiNode.Value);
                    }
                }
            }
        }

        void DrawUiNode(UiNode UiNode, int level = 0)
        {
            if (UiNode.Children.Count > 0)
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Box(GUIContent.none, "AnimationEventBackground", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(new GUIContent(""), GUILayout.Width(level * 50));
                if (UiNode.Children.Count > 0)
                {
                    if (GUILayout.Button(UiNode.ShowChildren ? new GUIContent("-") : new GUIContent("+"), EditorStyles.toolbarButton, GUILayout.Width(20)))
                    {
                        UiNode.ShowChildren = !UiNode.ShowChildren;
                    }
                }

                GUILayout.Label(new GUIContent(UiNode.Name), GUILayout.Width(150));

                if (level > 0)
                {
                    UiNode.VarName = GUILayout.TextField(string.IsNullOrEmpty(UiNode.VarName) ? UiNode.Name : UiNode.VarName, GUILayout.Width(100));
                    UiNode.IgnoreCreate = GUILayout.Toggle(UiNode.IgnoreCreate, new GUIContent("忽略生成"), GUILayout.Width(100));
                    UiNode.MergeCreate = GUILayout.Toggle(UiNode.MergeCreate, new GUIContent("合并生成"), GUILayout.Width(100));

                    if (UiNode.NodeEvents != null && UiNode.NodeEvents.Length > 0)
                    {
                        for (int i = 0; i < UiNode.NodeEvents.Length; i++)
                        {
                            UiNodeEvent UiNodeEvent = UiNode.NodeEvents[i];
                            UiNode.NodeEvents[i].IsActive = GUILayout.Toggle(UiNodeEvent.IsActive, new GUIContent(UiNodeEvent.Type), GUILayout.Width(100));
                        }
                    }
                }
                else
                {
                    GUILayout.Label(new GUIContent("保存文件夹："), GUILayout.Width(100));
                    SaveDirName = GUILayout.TextField(SaveDirName, GUILayout.Width(100));
                }
            }


            if (UiNode.ShowChildren)
            {
                foreach (var child in UiNode.Children)
                {
                    DrawUiNode(child.Value, level + 1);
                }
            }
        }

        Vector2 scroll = Vector2.zero;
        private void OnGUI()
        {
            if (ShowUiNode != null)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("子UI:", GUILayout.Width(30));
                    IsChildUi = GUILayout.Toggle(IsChildUi, "", GUILayout.Width(50));
                    GUILayout.Space(20);

                    if (IsChildUi)
                    {
                        GUILayout.Label("父UI名称:", GUILayout.Width(80));
                        ParentUiName = EditorGUILayout.TextField(ParentUiName, GUILayout.Width(200));
                    }

                }
                GUILayout.EndHorizontal();

                GUILayout.Space(20);

                using (GUILayout.ScrollViewScope s = new GUILayout.ScrollViewScope(scroll, GUILayout.ExpandHeight(true)))
                {
                    scroll = s.scrollPosition;
                    DrawUiNode(ShowUiNode);

                }

                using (new GUILayout.HorizontalScope("AnimationEventBackground", GUILayout.Height(35)))
                {
                    if (GUILayout.Button("保存"))
                    {
                        if (!IsChildUi)
                            ParentUiName = null;

                        if (IsChildUi && string.IsNullOrEmpty(ParentUiName))
                        {
                            EditorUtility.DisplayDialog("警告", "请填写父UI名字", "确定");
                            return;
                        }

                        if (IsChildUi && !ShowUiNode.Name.StartsWith("Ui"))
                        {
                            EditorUtility.DisplayDialog("警告", "子UI的前缀必须为Ui 如:UiXXXX", "确定");
                            return;
                        }

                        SaveCacheData(ClassName, new UiJson()
                        {
                            SaveDirName = SaveDirName,
                            UiNodeData = ShowUiNode,
                            IsChildUi = IsChildUi,
                            ParentName = ParentUiName
                        });
                        AutoGenerateLua.HandlerPrefab(rootGameObject);
                        AutoGenerateLua.HandlerUiNode(ParentUiName, IsChildUiSetGameObject, SaveDirName, ShowUiNode);

                        Instance.Close();
                    }

                    if (GUILayout.Button("重置"))
                    {
                        RefreshShowUiNode();
                    }

                    if (GUILayout.Button("清空"))
                    {
                        ShowUiNode = new UiNode { };
                        CheckUiNode(null, ShowUiNode, NewUiNode);
                    }
                }
            }
        }

        private static string ResetNodeKey(string key, string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                return key;
            }

            string key2d = sceneName + "2d";
            string key3d = sceneName + "3d";

            if (key.StartsWith(key2d))
            {
                return sceneName + "/SafeAreaContentPane" + key.Substring(key2d.Length);
            }

            if (key.StartsWith(key3d))
            {
                return sceneName + "/FullScreenBackground" + key.Substring(key3d.Length);
            }

            return key;
        }

        //todo: 新ui框架重置节点逻辑
        static UiNode ResetUiNode(UiNode uiNode, string sceenName)
        {
            if (!string.IsNullOrEmpty(uiNode.Key))
            {
                uiNode.Key = ResetNodeKey(uiNode.Key, sceenName);
            }

            if (uiNode.Children.Count > 0)
            {
                List<UiNode> nodes = new List<UiNode>(uiNode.Children.Count);
                foreach (var pair in uiNode.Children)
                {
                    nodes.Add(ResetUiNode(pair.Value, sceenName));
                }

                uiNode.Children.Clear();
                foreach (UiNode node in nodes)
                {
                    uiNode.Children.Add(node.Key, node);
                }
            }

            return uiNode;
        }

        static void RefreshShowUiNode()
        {
            UiJson jsonData = GetCacheData<UiJson>(ClassName);

            if (jsonData != null)
            {
                if (!string.IsNullOrEmpty(jsonData.SaveDirName))
                {
                    SaveDirName = jsonData.SaveDirName;
                }

                if (jsonData.IsChildUi)
                {
                    IsChildUi = jsonData.IsChildUi;
                    if (!string.IsNullOrEmpty(jsonData.ParentName))
                        ParentUiName = jsonData.ParentName;
                }

                if (jsonData.UiNodeData != null)
                {
                    ShowUiNode = ResetUiNode(jsonData.UiNodeData, SceneName);
                }
                else
                {
                    ShowUiNode = GetCacheData<UiNode>(ClassName);
                }
            }

            if (ShowUiNode == null)
            {
                ShowUiNode = new UiNode { };
            }

            CheckUiNode(null, ShowUiNode, NewUiNode);
        }

        public static void OpenWindow(string sceneName, string className, UiNode newUiNode)
        {
            SceneName = sceneName;
            ClassName = className;
            NewUiNode = newUiNode;
            SaveDirName = "X" + SceneName;
            IsChildUi = false;
            ParentUiName = null;
            IsChildUiSetGameObject = false;

            RefreshShowUiNode();

            if (sceneName != newUiNode.Name)
            {
                ParentUiName = sceneName;
                IsChildUiSetGameObject = true;
            }

            Instance = (AutoGenerateWindow)EditorWindow.GetWindow(typeof(AutoGenerateWindow));
            Instance.minSize = new Vector2(800, 500);
            Instance.titleContent = new GUIContent("脚本生成");
        }

        [MenuItem("GFramework/自动生成预制体和Lua代码")]
        static void CreateUiScript()
        {
            if (Selection.activeGameObject == null)
            {
                EditorUtility.DisplayDialog("错误", "当前未选中UI", "确认");
                return;
            }

            rootGameObject = Selection.activeGameObject.gameObject;
            NewUiNode = AutoGenerateLua.CreateNewUiNode(rootGameObject.transform);
            string className = AutoGenerateLua.GetClassName(rootGameObject.name);
            string sceneName = rootGameObject.name;
            OpenWindow(sceneName, className, NewUiNode);
        }

        /// <summary>
        /// 用于Json版本更新
        /// </summary>
        public class UiJsonOld
        {
            public string SaveDirName;
            public UiNode[] UiNodeDatas;
        }

        static void UpgradeJsons()
        {
            DirectoryInfo dir = new DirectoryInfo(CACHE_FILE_PATH.Replace("{filename}.json", ""));
            var files = dir.GetFiles("*.json");
            foreach (var file in files)
            {
                string content = File.ReadAllText(file.FullName, System.Text.Encoding.UTF8);
                content = content.Replace("UI", "Ui");
                string uiName = file.Name.Replace("XUI", "Ui").Replace(".json", "");
                Debug.Log(uiName);
                UiJsonOld oldJson = Json.Parse<UiJsonOld>(content);
                UiJson json = new UiJson();
                json.SaveDirName = oldJson.SaveDirName.Replace("2d", "");
                json.UiNodeData = new UiNode();
                UiNode node = json.UiNodeData;
                node.Children = new Dictionary<string, UiNode>();
                foreach (var oldData in oldJson.UiNodeDatas)
                {
                    foreach (var oldChild in oldData.Children)
                    {
                        node.Children.Add(oldChild.Key, oldChild.Value);
                    }
                }
                content = Json.ToString(json);
                File.WriteAllText(file.FullName, content, System.Text.Encoding.UTF8);
            }
            Debug.Log("更新Json文件完成。");
        }
    }
}