using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GFramework
{
    public class UIGenerator : EditorWindow
    {
  

        private const string SCRIPT_TEMPLATE_PATH = "Assets/GFramework/Editor/UIGenerator/ScriptTemplate.cs.txt";
        private const string GENERATE_ROOT_PATH = "Assets/UI";
        private const string PREFAB_SAVE_PATH = GENERATE_ROOT_PATH + "/Prefab";
        private const string CSHARP_SAVE_PATH = GENERATE_ROOT_PATH + "/CSharp";
        private const string REFRESH_FLAG_PATH = CSHARP_SAVE_PATH + "/refresh.txt";
        private const string LUA_SAVE_PATH = GENERATE_ROOT_PATH + "/Lua";

        private static string _saveName = null;
        private static UIGenerator _instance = null;
        private static GameObject _rootObject = null;
        private static NodeInfo _rootInfo = null;
        private static Dictionary<string, int> _nameDict = new Dictionary<string, int>();

        [MenuItem("GameObject/GFramework/一键生成代码", false, 0)]
        static void MenuClicked()
        {
            if (Selection.activeGameObject == null)
            {
                EditorUtility.DisplayDialog("错误", "当前未选中节点", "确认");
                return;
            }

            _nameDict.Clear();
            _rootObject = Selection.activeGameObject;
            _rootInfo = CreateNode(null, _rootObject.transform);
            _saveName = _rootInfo.Name;
            _instance = (UIGenerator)GetWindow(typeof(UIGenerator));
            _instance.minSize = new Vector2(800, 500);
            _instance.titleContent = new GUIContent("脚本生成");
        }

        private void OnGUI()
        {
            if (_rootObject != null)
            {
                GUILayout.Space(20);
                GUILayout.BeginHorizontal(GUIStyle.none, GUILayout.Height(35));
                {
                    GUILayout.Space(20);
                    GUILayout.Label("当前节点：", GUILayout.Width(60));
                    GUILayout.Label(_rootObject.name, GUILayout.Width(100));
                    GUILayout.Label("保存文件：", GUILayout.Width(60));
                    _saveName = GUILayout.TextField(_saveName, GUILayout.Width(100));
                    GUILayout.Space(20);
                }
                GUILayout.EndHorizontal();

                Vector2 scroll = Vector2.zero;
                scroll = GUILayout.BeginScrollView(scroll, GUILayout.ExpandHeight(true));
                {
                    ShowHierarchy(_rootInfo);
                }
                GUILayout.EndScrollView();

                GUILayout.BeginHorizontal(GUIStyle.none, GUILayout.Height(35));
                {
                    GUILayout.Space(20);
                    if (GUILayout.Button("保存Lua"))
                    {
                        PreCheckLua();
                        CreateLuaScript();
                        _instance.Close();
                    }

                    if (GUILayout.Button("保存C#"))
                    {
                        PreCheckCSharp();
                        GenerateCSharpScript();
                        _instance.Close();
                    }

                    if (GUILayout.Button("重置"))
                    {

                    }

                    if (GUILayout.Button("清空"))
                    {

                    }
                    GUILayout.Space(20);
                }
                GUILayout.EndHorizontal();
            }
        }

        private static void CreateLuaScript()
        {

        }

        private static void GenerateCSharpScript()
        {
            string savePath = $"{CSHARP_SAVE_PATH}/{_saveName}.cs";
            StringBuilder definition = new StringBuilder();
            StringBuilder assignment = new StringBuilder();
            StringBuilder binding = new StringBuilder();
            StringBuilder listenerDefinition = new StringBuilder();

            CreateCSharpScript(_rootInfo, ref definition, ref assignment, ref binding, ref listenerDefinition);

            definition.AppendLine();
            
            string script = AssetDatabase.LoadAssetAtPath<TextAsset>(SCRIPT_TEMPLATE_PATH).text;
            script = script.Replace("#Name#", _saveName);
            script = script.Replace("#Definition#", definition.ToString());
            script = script.Replace("#Assignment#", assignment.ToString());
            script = script.Replace("#Binding#", binding.ToString());
            script = script.Replace("#ListenerDefinition#", listenerDefinition.ToString());
            File.WriteAllText(savePath, script.ToString());
            File.WriteAllText(REFRESH_FLAG_PATH, _saveName + " " + _rootInfo.Name + " " + _rootObject.GetInstanceID().ToString());
            AssetDatabase.Refresh();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void BindScriptAfterCompile()
        {
            if(File.Exists(REFRESH_FLAG_PATH))
            {
                string[] info = AssetDatabase.LoadAssetAtPath<TextAsset>(REFRESH_FLAG_PATH).text.Split(' ');
                string scriptName = info[0];
                string prefabName = info[0];
                int instanceID = int.Parse(info[2]);
                string scriptPath = string.Format("{0}/{1}.cs", CSHARP_SAVE_PATH, scriptName);
                string prefabPath = string.Format("{0}/{1}.prefab", PREFAB_SAVE_PATH, prefabName);
                GameObject refreshObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type type = asm.GetType(scriptName);
                    if (type != null)
                    {
                        refreshObject.AddComponent(type);
                        break;
                    }
                }

                PrefabUtility.SaveAsPrefabAsset(refreshObject, prefabPath);
                //PrefabUtility.CreatePrefab(prefabPath, refreshObject);
                File.Delete(REFRESH_FLAG_PATH);
                AssetDatabase.Refresh();
            }
        }

        private static void CreateCSharpScript(NodeInfo nodeInfo, ref StringBuilder definition, ref StringBuilder assignment, ref StringBuilder binding, ref StringBuilder listenerDefinition, bool isRoot = true, string findPath = "")
        {
            string findString = "";
            if (!isRoot)
            {
                if (findPath == "") findPath = nodeInfo.Name;
                else findPath += "/" + nodeInfo.Name;
                findString = $".Find(\"{findPath}\")";
            }

            if (nodeInfo.CreateVarible)
            {
                string transformName = "m_" + nodeInfo.VaribleName;
                definition.AppendLine();
                definition.Append($"\tprivate Transform {transformName};");
                assignment.AppendLine();
                assignment.Append($"\t\t{transformName} = transform{findString};");
                foreach (var component in nodeInfo.ComponentList)
                {
                    string componentName = "m_" + nodeInfo.VaribleName + component.Type.ToString();
                    if (component.Active)
                    {
                        definition.AppendLine();
                        definition.Append($"\tprivate {component.Type.ToString()} {componentName};");
                        assignment.AppendLine();
                        assignment.Append($"\t\t{componentName} = {transformName}.GetComponent<{component.Type.ToString()}>();");
                        foreach (var nodeEvent in component.EventList)
                        {
                            if (nodeEvent.Active)
                            {
                                binding.AppendLine();
                                binding.Append($"\t\t{componentName}.{nodeEvent.EventName}.AddListener(On{nodeInfo.VaribleName}{nodeEvent.EventName.Substring(2)});");

                                listenerDefinition.AppendLine();
                                listenerDefinition.AppendLine($"\tprivate void On{nodeInfo.VaribleName}{nodeEvent.EventName.Substring(2)}()");
                                listenerDefinition.AppendLine("\t{");
                                listenerDefinition.AppendLine();
                                listenerDefinition.AppendLine("\t}");
                            }
                        }
                    }
                }
            }

            foreach (var child in nodeInfo.ChildrenDict)
            {
                CreateCSharpScript(child.Value, ref definition, ref assignment, ref binding, ref listenerDefinition, false, findPath);
            }
        }

        private static void PreCheckLua()
        {

        }

        private static void PreCheckCSharp()
        {
            Directory.CreateDirectory(GENERATE_ROOT_PATH);
            Directory.CreateDirectory(PREFAB_SAVE_PATH);
            Directory.CreateDirectory(CSHARP_SAVE_PATH);
            if (AssetDatabase.IsMainAssetAtPathLoaded(REFRESH_FLAG_PATH))
            {
                AssetDatabase.DeleteAsset(REFRESH_FLAG_PATH);
            }

            Type generateType = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = asm.GetType(_saveName);
                if (type != null)
                {
                    generateType = type;
                    break;
                }
            }

            if (generateType != null)
            {
                foreach (var component in _rootObject.GetComponents<Component>())
                {
                    if (component.GetType() == generateType)
                    {
                        DestroyImmediate(component);
                    }
                }
            }

            string scriptSavePath = string.Format("{0}/{1}.cs", CSHARP_SAVE_PATH, _saveName);
            if (AssetDatabase.IsMainAssetAtPathLoaded(scriptSavePath))
            {
                AssetDatabase.DeleteAsset(scriptSavePath);
            }
            string prefabSavePath = string.Format("{0}/{1}.prefab", PREFAB_SAVE_PATH, _rootInfo.Name);
            if (AssetDatabase.IsMainAssetAtPathLoaded(prefabSavePath))
            {
                AssetDatabase.DeleteAsset(prefabSavePath);
            }
        }

        private static void ShowHierarchy(NodeInfo nodeInfo, int level = 1)
        {
            int childrenCount = nodeInfo.ChildrenDict.Count;

            GUILayout.BeginHorizontal(GUIStyle.none, GUILayout.Height(20));
            {
                GUILayout.Space(level * 20);

                if (childrenCount > 0)
                {
                    if (GUILayout.Button(nodeInfo.ShowChild ? "-" : "+", EditorStyles.foldout, GUILayout.Width(20)))
                        nodeInfo.ShowChild = !nodeInfo.ShowChild;
                }
                else
                {
                    GUILayout.Space(24);
                }

                GUILayout.Label(nodeInfo.Name, GUILayout.ExpandWidth(false));
                nodeInfo.CreateVarible = GUILayout.Toggle(nodeInfo.CreateVarible, "生成", GUILayout.Width(60));
                nodeInfo.VaribleName = GUILayout.TextField(nodeInfo.VaribleName, GUILayout.Width(100));


                foreach (var component in nodeInfo.ComponentList)
                {
                    component.Active = GUILayout.Toggle(component.Active, component.Type.ToString(), GUILayout.Width(60));
                    if (component.Active && component.EventList.Count > 0)
                    {
                        foreach (var nodeEvent in component.EventList)
                        {
                            nodeEvent.Active = GUILayout.Toggle(nodeEvent.Active, nodeEvent.EventName, GUILayout.Width(60));
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();

            if (childrenCount > 0 && nodeInfo.ShowChild)
            {
                foreach (var child in nodeInfo.ChildrenDict)
                {
                    ShowHierarchy(child.Value, level + 1);
                }
            }
        }

        private static NodeInfo CreateNode(NodeInfo parentNode, Transform curTransform)
        {
            NodeInfo nodeInfo = new NodeInfo(curTransform);

            if (_nameDict.ContainsKey(curTransform.name))
            {
                nodeInfo.VaribleName = nodeInfo.Name + Convert.ToChar('A' + _nameDict[nodeInfo.Name]);
                _nameDict[nodeInfo.Name]++;
            }
            else
            {
                nodeInfo.VaribleName = nodeInfo.Name;
                _nameDict.Add(curTransform.name, 0);
            }

            if (parentNode != null)
            {
                if (parentNode.ChildrenDict.ContainsKey(nodeInfo.Name))
                {
                    Debug.LogError($"UI Name repeated：{nodeInfo.Name}");
                }
                else
                {
                    parentNode.ChildrenDict.Add(nodeInfo.Name, nodeInfo);
                }
            }
            else
                nodeInfo.VaribleName = "UIRoot";

            for (int index = 0; index < curTransform.childCount; index++)
            {
                CreateNode(nodeInfo, curTransform.GetChild(index));
            }
            
            return nodeInfo;
        }
    }

    public class NodeInfo
    {
        public string Name;
        public string VaribleName;
        public bool CreateVarible;
        public bool ShowChild;
        public List<NodeComponent> ComponentList;
        public Dictionary<string, NodeInfo> ChildrenDict;
        public List<NodeEvent> EventList;

        public NodeInfo(Transform transform)
        {
            Name = transform.name;
            VaribleName = null;
            CreateVarible = true;
            ShowChild = true;
            ComponentList = new List<NodeComponent>();
            ChildrenDict = new Dictionary<string, NodeInfo>();

            if (transform.GetComponent<Text>()) ComponentList.Add(new NodeComponent(true, ComponentType.Text));
            if (transform.GetComponent<Image>()) ComponentList.Add(new NodeComponent(true, ComponentType.Image));
            if (transform.GetComponent<RawImage>()) ComponentList.Add(new NodeComponent(true, ComponentType.RawImage));
            if (transform.GetComponent<Button>()) ComponentList.Add(new NodeComponent(true, ComponentType.Button));
            if (transform.GetComponent<Toggle>()) ComponentList.Add(new NodeComponent(true, ComponentType.Toggle));
            if (transform.GetComponent<Slider>()) ComponentList.Add(new NodeComponent(true, ComponentType.Slider));
            if (transform.GetComponent<InputField>()) ComponentList.Add(new NodeComponent(true, ComponentType.InputField));
            if (transform.GetComponent<ScrollRect>()) ComponentList.Add(new NodeComponent(true, ComponentType.ScrollRect));
            if (transform.GetComponent<Scrollbar>()) ComponentList.Add(new NodeComponent(true, ComponentType.Scrollbar));
            if (transform.GetComponent<Dropdown>()) ComponentList.Add(new NodeComponent(true, ComponentType.Dropdown));
        }
    }
    
    public class NodeComponent
    {
        public bool Active;
        public ComponentType Type;
        public List<NodeEvent> EventList;

        public NodeComponent(bool acvtive, ComponentType type)
        {
            Active = acvtive;
            Type = type;
            EventList = new List<NodeEvent>();
            if (UIDict.UiEventDict.ContainsKey(Type))
            {
                foreach (var eventName in UIDict.UiEventDict[type])
                {
                    EventList.Add(new NodeEvent(true, eventName));
                }
            }
        }
    }

    public class NodeEvent
    {
        public bool Active;
        public string EventName;

        public NodeEvent(bool active, string eventName)
        {
            Active = active;
            EventName = eventName;
        }
    }

    public static class UIDict
{
    public static Dictionary<ComponentType, List<string>> UiEventDict = new Dictionary<ComponentType, List<string>> {
            {ComponentType.Button, new List<string> {"onClick"}},
            {ComponentType.Toggle, new List<string> {"onValueChanged"}},
            {ComponentType.Slider, new List<string> {"onValueChanged"}},
            {ComponentType.Scrollbar,new List<string> {"onValueChanged"}},
            {ComponentType.ScrollRect,new List<string> {"onValueChanged"}},
            {ComponentType.Dropdown,new List<string> {"onValueChanged"}},
            {ComponentType.InputField,new List<string> {"onValueChanged", "onEndEdit"}},
        };
}

public enum ComponentType
    {
        Text,
        Image,
        RawImage,
        Button,
        Toggle,
        Slider,
        InputField,
        ScrollRect,
        Scrollbar,
        Dropdown,
    }
}