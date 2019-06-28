using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GFramework.Editor.AutoGenerateScript
{
    public class AutoGenerateCSharp
    {
        [MenuItem("GFramework/自动生成预制体和C#代码")]
        private static void MenuClicked()
        {
            if (Selection.activeGameObject == null)
            {
                EditorUtility.DisplayDialog("错误", "当前未选中UI", "确认");
                return;
            }

            string PREFAB_FILE_PATH = "Assets/UI/Prefab";
            string SCRIPT_FILE_PATH = "Assets/UI/CSharp";

            if (Selection.activeGameObject.tag == "UIView")
            {
                string prefabPath = string.Format("{0}/{1}.prefab", PREFAB_FILE_PATH, Selection.activeGameObject.name);
                //PrefabUtility.SaveAsPrefabAsset(Selection.activeGameObject, prefabPath);
                PrefabUtility.CreatePrefab(prefabPath, Selection.activeGameObject);

                string basePath = string.Format("{0}/{1}{2}.cs", SCRIPT_FILE_PATH, "UI", Selection.activeGameObject.name);
                string script = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GFramework/Editor/UIScriptGenerate/UIBase.cs.txt").text;

                StringBuilder property = new StringBuilder();
                StringBuilder get = new StringBuilder();
                Dictionary<string, int> uniqueName = new Dictionary<string, int>();
                HashSet<string> uniquePath = new HashSet<string>();

                foreach (var transform in Selection.activeGameObject.GetComponentsInChildren<Transform>(true))
                {
                    if (transform.tag == "UIProperty")
                    {
                        Component component = transform.GetComponent<Button>();
                        if (component == null)
                            component = transform.GetComponent<Image>();
                        if (component == null)
                            component = transform.GetComponent<Text>();
                        if (component == null)
                            component = transform.GetComponent<RectTransform>();
                        if (component != null)
                        {
                            string propertyName = component.name;
                            int uniqueValue;
                            if (uniqueName.TryGetValue(propertyName, out uniqueValue))
                            {
                                uniqueValue = uniqueName[propertyName] += 1;
                            }
                            else
                            {
                                uniqueValue = uniqueName[propertyName] = 0;
                            }

                            if (uniqueValue > 0)
                            {
                                propertyName = string.Format("{0}_{1}", propertyName, uniqueValue);
                            }

                            property.AppendFormat("\tprotected {0} m_{1};", component.GetType().ToString(), propertyName).AppendLine();

                            string findPath = AnimationUtility.CalculateTransformPath(component.transform, Selection.activeTransform);
                            if (!uniquePath.Contains(findPath))
                            {
                                uniquePath.Add(findPath);
                            }
                            else
                            {
                                Debug.LogErrorFormat("读取路径发生错误： {0}", findPath);
                            }

                            get.AppendFormat("\t\tm_{0} = transform.Find(\"{1}\").GetComponent<{2}>();", propertyName, findPath, component.GetType().ToString()).AppendLine();
                        }
                    }
                }
                script = script.Replace("#Name#", Path.GetFileNameWithoutExtension(basePath));
                script = script.Replace("#Property#", property.ToString());
                script = script.Replace("#Get#", get.ToString());
                File.WriteAllText(basePath, script);

                AssetDatabase.Refresh();
            }
        }
    }
}