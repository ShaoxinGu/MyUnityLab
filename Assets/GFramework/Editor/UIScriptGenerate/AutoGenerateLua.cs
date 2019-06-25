using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Text;

public class UiNodeEvent
{
    public bool IsActive;
    public string Type;
}

public class UiNode
{
    public string Name;
    public string Type;
    public string VarName;
    public string Key;
    public string MidPath = "";
    public bool ShowChildren = true;
    public bool IgnoreCreate = false;
    public bool MergeCreate = true;
    public UiNodeEvent[] NodeEvents;
    public Dictionary<string, UiNode> Children = new Dictionary<string, UiNode>();
}

public static class UiDict
{
    public static readonly Dictionary<string, string[]> UiEventDict = new Dictionary<string, string[]> {
        {"Button", new string[] {"onClick"}},
        {"Toggle", new string[]  {"onValueChanged"}},
        {"Slider", new string[]  {"onValueChanged"}},
        {"Scrollbar", new string[]  {"onValueChanged"}},
        {"ScrollRect", new string[]  {"onValueChanged"}},
        {"Dropdown", new string[]  {"onValueChanged"}},
        {"InputField", new string[]  {"onValueChanged", "onEndEdit"}},
    };


    public static readonly Dictionary<string, string> UiTypeDict = new Dictionary<string, string> {
        {"Txt"  , "Text"        },
        {"Img"  , "Image"       },
        {"RImg" , "RawImage"    },
        {"Btn"  , "Button"      },
        {"Tog"  , "Toggle"      },
        {"Sli"  , "Slider"      },
        {"InF"  , "InputField"  },
        {"SView", "ScrollRect"  },
        {"Scr"  , "Scrollbar"   },
        {"Drd"  , "Dropdown"    },
        {"Ui"   , ""            },
        {"Panel", ""            },
        {"Grid" , ""            },
    };
}


public static class AutoGenerateLua
{
    static string ROOT_PANEL_PREFIX = "Ui";
    static string SCRIPT_FILE_PATH = "Assets/UI/Lua/";
    static string PREFAB_FILE_PATH = "Assets/UI/Prefab";

    static string UiViewClassStr = "local {className} = XLuaUiManager.Register(XLuaUi, \"{uiName}\", CsXUiType.Normal,CsXUiResType.Bundle,false)\r\n" +
        "\r\nfunction {className}:OnAwake()\r\n" +
        "    self:InitAutoScript()\r\n" +
        "end\r\n\r\n" +
        "\r\nfunction {className}:OnStart(...)\r\n" +
        "end\r\n\r\n" +
        "\r\nfunction {className}:OnEnable()\r\n" +
        "end\r\n\r\n" +
        "\r\nfunction {className}:OnDisable()\r\n" +
        "end\r\n\r\n" +
        "\r\nfunction {className}:OnDestroy()\r\n" +
        "end\r\n\r\n" +
        "\r\nfunction {className}:OnGetEvents()\r\n" +
        "    return nil\r\n" +
        "end\r\n\r\n" +
        "\r\nfunction {className}:OnNotify(evt,...)\r\n" +
        "end\r\n\r\n";

    static string UiChildViewClassStr = "local {className} = XLuaUiManager.RegisterChild(XLuaUi, \"{uiName}\", \"{parentUiName}\", false, CsXUiResType.{resType},false)\r\n" +
        "\r\nfunction {className}:OnAwake()\r\n" +
        "    self:InitAutoScript()\r\n" +
        "end\r\n\r\n" +
        "\r\nfunction {className}:OnStart(...)\r\n" +
        "end\r\n\r\n" +
        "\r\nfunction {className}:OnEnable()\r\n" +
        "end\r\n\r\n" +
        "\r\nfunction {className}:OnDisable()\r\n" +
        "end\r\n\r\n" +
        "\r\nfunction {className}:OnDestroy()\r\n" +
        "end\r\n\r\n" +
        "\r\nfunction {className}:OnGetEvents()\r\n" +
        "    return nil\r\n" +
        "end\r\n\r\n" +
        "\r\nfunction {className}:OnNotify(evt,...)\r\n" +
        "end\r\n\r\n";

    static string UiClassStr = "local {className} = XClass()\r\n" +
        "\r\nfunction {className}:Ctor(ui)\r\n" +
        "    self.GameObject = ui.gameObject\r\n" +
        "    self.Transform = ui.transform\r\n" +
        "    self:InitAutoScript()\r\n" +
        "end\r\n\r\n";

    static List<UiNode> UiNodeList = new List<UiNode>();
    static Dictionary<string, int> NameDict = new Dictionary<string, int>();
    static bool IsGridNode = false;
    static string ParentUiName = null;
    static bool IsChildUiSetGameObject = false;

    static string GetUiType(string name)
    {
        foreach (var ui in UiDict.UiTypeDict)
        {
            if (name.StartsWith(ui.Key))
            {
                return ui.Value;
            }
        }

        return null;
    }

    static string GetClassStr(string uiName)
    {
        if (uiName.StartsWith(ROOT_PANEL_PREFIX))
        {
            string str = string.Empty;

            if (!string.IsNullOrEmpty(ParentUiName))
            {
                str = UiChildViewClassStr.Replace("{className}", GetClassName(uiName));
                str = str.Replace("{uiName}", uiName);
                str = str.Replace("{parentUiName}", ParentUiName);
                if (IsChildUiSetGameObject)
                    str = str.Replace("{resType}", "SetGameObject");
                else
                    str = str.Replace("{resType}", "Bundle");
            }
            else
            {
                str = UiViewClassStr.Replace("{className}", GetClassName(uiName));
                str = str.Replace("{uiName}", uiName);
            }

            return str;
        }
        else
        {
            return UiClassStr.Replace("{className}", GetClassName(uiName));
        }
    }

    public static string GetClassName(string uiName)
    {
        if (uiName.StartsWith(ROOT_PANEL_PREFIX))
        {
            return "X" + uiName;
        }
        else
        {
            return "XUi" + uiName;
        }
    }

    public static UiNode CreateNewUiNode(Transform root)
    {
        NameDict.Clear();
        string rootName = root.name;

        UiNode rootNode = new UiNode { Name = "root" };
        CreateUiNode(root, root.name, "", ref rootNode);

        return rootNode;
    }

    static void CreateUiNodeList(UiNode uiNode)
    {
        if (!uiNode.MergeCreate)
        {
            UiNodeList.Add(uiNode);
        }

        if (uiNode.Children.Count > 0)
        {
            List<UiNode> tmpList = new List<UiNode>();
            List<UiNode> ignoreList = new List<UiNode>();
            foreach (var child in uiNode.Children)
            {
                if (child.Value.IgnoreCreate)
                {
                    ignoreList.Add(child.Value);
                }
                else
                {
                    CreateUiNodeList(child.Value);

                    if (!child.Value.MergeCreate)   // 独立生成文件
                    {
                        tmpList.Add(child.Value);
                    }
                }
            }

            foreach (var node in ignoreList)
            {
                uiNode.Children.Remove(node.Key);
            }

            foreach (var node in tmpList)
            {
                uiNode.Children[node.Key] = new UiNode
                {
                    Name = node.Name,
                    VarName = node.VarName,
                    MidPath = node.MidPath,
                };
            }
        }
    }

    public static void HandlerPrefab(GameObject gameObject)
    {
        string prefabPath = string.Format("{0}/{1}.prefab", PREFAB_FILE_PATH, gameObject.name);
        PrefabUtility.SaveAsPrefabAsset(gameObject,prefabPath);
        //PrefabUtility.CreatePrefab(prefabPath, gameObject);
    }

    public static void HandlerUiNode(string parentUiName, bool isChildUiSetGameObject, string saveDirName, UiNode uiNode)
    {
        NameDict.Clear();
        UiNodeList.Clear();
        UiNodeList.Add(uiNode);
        ParentUiName = parentUiName;
        IsChildUiSetGameObject = isChildUiSetGameObject;

        IsGridNode = uiNode.Name.StartsWith("Grid");

        CreateUiNodeList(uiNode);

        foreach (var node in UiNodeList)
        {
            CreateScript(saveDirName, node);
        }
    }

    public static void CreateScript(string saveDirName, UiNode uiNode)
    {
        string rootName = uiNode.Name;
        string className = GetClassName(rootName);
        string scriptFile = SCRIPT_FILE_PATH + className + ".lua";

        if (!Directory.Exists(SCRIPT_FILE_PATH))
        {
            Directory.CreateDirectory(SCRIPT_FILE_PATH);
        }

        string fileStr = "";
        if (File.Exists(scriptFile))
        {
            FileStream file = new FileStream(scriptFile, FileMode.Open, FileAccess.ReadWrite);
            StreamReader read = new StreamReader(file);
            fileStr = read.ReadToEnd();
            read.Close();
            file.Close();
        }

        // 生成替换 -- auto 部分的代码
        string[] strs = Regex.Split(fileStr, "-- auto\r\n", RegexOptions.IgnoreCase);
        string classStr = string.IsNullOrEmpty(strs[0]) ? GetClassStr(rootName) : strs[0];

        classStr += "-- auto\r\n";
        classStr += "-- Automatic generation of code, forbid to edit";
        classStr += GetInitAutoScript(className);
        classStr += GetAutoInitUiStr(uiNode, className);
        //classStr += GetAutoKeyStr(className);
        classStr += GetAutoAddListenerStr(uiNode, className);
        classStr += "-- auto\r\n";

        // 事件函数，函数体需要手动实现，不能直接被替换
        classStr += GetEventFuncsStr(uiNode, fileStr, className);
        if (strs.Length >= 3)
        {
            for (int i = 2; i < strs.Length; ++i)
            {
                classStr += strs[i];
            }
        }
        else
        {
            classStr += GetEndStr(rootName);
        }

        File.WriteAllText(scriptFile, classStr, new UTF8Encoding(false));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static string GetEndStr(string uiName)
    {
        if (!uiName.StartsWith(ROOT_PANEL_PREFIX))
        {
            return "\r\nreturn " + GetClassName(uiName) + "\r\n";
        }
        return "";
    }

    static string GetInitAutoScript(string className)
    {
        string initStr = "";
        initStr += "\r\nfunction " + className + ":InitAutoScript()\r\n";
        initStr += "    self:AutoInitUi()\r\n";
        //initStr += "    self.SpecialSoundMap = {}\r\n";
        initStr += "    self:AutoAddListener()\r\n";
        initStr += "end\r\n";

        return initStr;
    }

    static void AutoInitUi(UiNode uiNode, string className, string path, ref string uiStr)
    {
        if (!string.IsNullOrEmpty(uiNode.Name))
        {
            // string tmpStr = "";
            string tranStr = "self.Transform";
            string tmpPath = "";
            string tmpType = "";

            if (!string.IsNullOrEmpty(path))
            {
                tmpPath = path;
                if (path.StartsWith("/"))
                {
                    tmpPath = path.Remove(0, 1);
                }
                //if (uiNode.Is3dUi)
                //{
                //    tranStr += "3d";
                //}
            }


            if (!string.IsNullOrEmpty(uiNode.Type))
            {
                tmpType = uiNode.Type;
            }

            if (!string.IsNullOrEmpty(tmpPath) || !string.IsNullOrEmpty(tmpType))
            {
                if (IsGridNode)
                {
                    uiStr += "    self." + uiNode.VarName + " = XUiHelper.TryGetComponent(" + tranStr + ", ";
                    uiStr += !string.IsNullOrEmpty(tmpPath) ? "\"" + tmpPath + "\", " : "nil, ";
                    uiStr += !string.IsNullOrEmpty(tmpType) ? "\"" + tmpType + "\")" : "nil)";
                }
                else
                {
                    uiStr += "    self." + uiNode.VarName + " = " + tranStr;
                    if (!string.IsNullOrEmpty(tmpPath))
                    {
                        uiStr += ":Find(\"" + tmpPath + "\")";
                    }
                    if (!string.IsNullOrEmpty(tmpType))
                    {
                        uiStr += ":GetComponent(\"" + uiNode.Type + "\")";
                    }
                }
                uiStr += "\r\n";
            }

        }


        foreach (var child in uiNode.Children)
        {
            string nextPath = path + child.Value.MidPath;
            nextPath = string.IsNullOrEmpty(nextPath) ? child.Value.Name : nextPath + "/" + child.Value.Name;
            AutoInitUi(child.Value, className, nextPath, ref uiStr);
        }
    }

    // 自动查找生成Ui
    static string GetAutoInitUiStr(UiNode uiNode, string className)
    {
        string uiStr = "";
        uiStr += "\r\nfunction " + className + ":AutoInitUi()\r\n";
        AutoInitUi(uiNode, className, "", ref uiStr);
        uiStr += "end\r\n";

        return uiStr;
    }

    static void AutoAddListener(UiNode uiNode, string className, ref string eventStr)
    {

        if (uiNode.NodeEvents != null)
        {
            foreach (var nodeEvent in uiNode.NodeEvents)
            {
                if (nodeEvent.IsActive)
                {
                    //eventStr += "    self:RegisterListener(self." + uiNode.VarName + ", \"" + nodeEvent.Type + "\", self." + GetEventFuncName(uiNode.VarName, nodeEvent.Type) + ")\r\n";
                    eventStr += "    self:RegisterClickEvent(self." + uiNode.VarName + ", self." + GetEventFuncName(uiNode.VarName, nodeEvent.Type) + ")\r\n";

                }
            }
        }

        foreach (var child in uiNode.Children)
        {
            AutoAddListener(child.Value, className, ref eventStr);
        }
    }

    static string GetRegisterListenerStr(string className)
    {
        string listenerStr = "\r\nfunction " + className + ":RegisterClickEvent(uiNode, func)\r\n" +
            "    if func == nil then\r\n" +
            "        XLog.Error(\"" + className + ":RegisterClickEvent: func is nil\")\r\n" +
            "        return\r\n" +
            "    end\r\n" +
            "\r\n" +
            "    if type(func) ~= \"function\" then\r\n" +
            "        XLog.Error(\"" + className + ":RegisterClickEvent: func is not a function\")\r\n" +
            "    end\r\n" +
            "\r\n" +
            "    local listener = function(...)\r\n" +
            "        func(self, ...)\r\n" +
            "    end\r\n" +
            "\r\n" +
            "    CsXUiHelper.RegisterClickEvent(uiNode, listener)\r\n" +
            "end\r\n";

        return listenerStr;
    }

    static string GetAutoAddListenerStr(UiNode uiNode, string className)
    {
        string eventStr = string.Empty;
        //string eventStr = GetRegisterListenerStr(className);
        if (!uiNode.Name.StartsWith(ROOT_PANEL_PREFIX))
            eventStr += GetRegisterListenerStr(className);
        eventStr += "\r\nfunction " + className + ":AutoAddListener()\r\n";
        //eventStr += "    self.AutoCreateListeners = {}\r\n";
        AutoAddListener(uiNode, className, ref eventStr);
        eventStr += "end\r\n";
        return eventStr;
    }

    static string GetEventFuncName(string varName, string eventType)
    {
        //return "On" + varName + eventType.Substring(2);
        return "On" + varName + "Click";
    }

    static void AutoCreateEvent(UiNode uiNode, string fileStr, string className, ref string eventStr)
    {
        if (uiNode.NodeEvents != null)
        {
            foreach (var nodeEvent in uiNode.NodeEvents)
            {
                if (nodeEvent.IsActive)
                {
                    string func = "function " + className + ":" + GetEventFuncName(uiNode.VarName, nodeEvent.Type);
                    if (fileStr.IndexOf(func) == -1) // 没有对应的事件函数
                    {
                        eventStr += "\r\n" + func + "(eventData)\r\n" + "\r\nend\r\n";
                    }
                }
            }
        }

        foreach (var child in uiNode.Children)
        {
            AutoCreateEvent(child.Value, fileStr, className, ref eventStr);
        }
    }

    static string GetEventFuncsStr(UiNode uiNode, string fileStr, string className)
    {
        string funcsStr = "";
        AutoCreateEvent(uiNode, fileStr, className, ref funcsStr);
        return funcsStr;
    }

    static void GetEventFunc(UiNode uiNode)
    {
        if (uiNode == null)
        {
            return;
        }

        string[] events;

        if (!UiDict.UiEventDict.TryGetValue(uiNode.Type, out events))
        {
            return;
        }

        uiNode.NodeEvents = new UiNodeEvent[events.Length];

        for (int i = 0; i < events.Length; ++i)
        {
            string eventStr = events[i];
            uiNode.NodeEvents[i] = new UiNodeEvent()
            {
                IsActive = true,
                Type = eventStr
            };
        }
    }

    static void GetVarName(UiNode uiNode)
    {
        string varName = string.IsNullOrEmpty(uiNode.VarName) ? uiNode.Name : uiNode.VarName;
        if (NameDict.ContainsKey(varName))
        {
            string warnStr = "ui varName is repeated: path = " + uiNode.Key + " varName = " + varName;
            int repeatCount = NameDict[varName];
            NameDict[varName] += 1;
            varName = varName + Convert.ToChar('A' + repeatCount - 1);
            warnStr += " replace with " + varName;
            Debug.LogWarning(warnStr);
        }
        else
        {
            NameDict.Add(varName, 1);
        }

        uiNode.VarName = varName;
    }

    static void CreateUiNode(Transform root, string path, string midPath, ref UiNode parentNode)
    {
        if (root == null)
        {
            return;
        }

        string uiType = GetUiType(root.name);
        UiNode uiNode = null;

        if (uiType != null)
        {
            uiNode = new UiNode()
            {
                Name = root.name,
                Type = uiType,
                Key = path,
                MidPath = midPath
            };

            GetVarName(uiNode);
            GetEventFunc(uiNode);

            if (parentNode != null)
            {
                if (parentNode.Name == "root")
                {
                    parentNode = uiNode;
                }
                else
                {
                    if (parentNode.Children.ContainsKey(path))
                    {
                        Debug.LogError("ui name is repeated: " + path);
                    }
                    else
                    {
                        parentNode.Children.Add(path, uiNode);
                    }
                }
            }
        }

        if (root.childCount > 0)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                if (uiNode != null)
                {
                    CreateUiNode(child, path + "/" + child.name, "", ref uiNode);
                }
                else
                {
                    CreateUiNode(child, path + "/" + child.name, midPath + "/" + root.name, ref parentNode);
                }
            }
        }
    }
}