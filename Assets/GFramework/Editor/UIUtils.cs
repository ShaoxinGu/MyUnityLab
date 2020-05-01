using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIUtils
{
    [MenuItem("UI/Tools/Export")]
    static void ExportUI()
    {
        string prefabPathRoot = "Assets/UI/UIPrefab";
        string scriptPathRoot = "Assets/UI/UIScript";

        if (Selection.activeTransform)
        {
            if (Selection.activeGameObject.tag == "UIView")
            {
                string prefabPath = string.Format("{0}/{1}{2}", prefabPathRoot, Selection.activeGameObject.name, ".prefab");
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                PrefabUtility.SaveAsPrefabAssetAndConnect(Selection.activeGameObject, prefabPath, InteractionMode.AutomatedAction);
                string basePath = string.Format("{0}/{1}{2}", scriptPathRoot, Selection.activeGameObject.name, ".cs");
                string script = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/UI/UIBase.cs.txt").text;

                StringBuilder property = new StringBuilder();
                StringBuilder get = new StringBuilder();
                Dictionary<string, int> uniqueName = new Dictionary<string, int>();
                HashSet<string> uniquePath = new HashSet<string>();

                foreach (var transform in Selection.activeGameObject.GetComponentsInChildren<Transform>())
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
                            string propertyName = component.name.ToLower();
                            int uniqueValue;
                            if (uniqueName.TryGetValue(propertyName, out uniqueValue))
                                uniqueValue = uniqueName[propertyName] += 1;
                            else
                                uniqueValue = uniqueName[propertyName] = 0;
                            if (uniqueValue > 0)
                                propertyName = string.Format("{0}_{1}", propertyName, uniqueValue);
                            property.AppendFormat("\tprotected {0} m_{1};", component.GetType().ToString(), propertyName).AppendLine();

                            string findPath = AnimationUtility.CalculateTransformPath(component.transform, Selection.activeTransform);
                            if (!uniquePath.Contains(findPath))
                                uniquePath.Add(findPath);
                            else
                                Debug.LogErrorFormat("读取路径发生重复：{0}", findPath);
                            get.AppendFormat("\t\tm_{0}=transform.Find(\"{1}\").GetComponent<{2}>();", propertyName, findPath, component.GetType().ToString()).AppendLine();
                        }
                    }
                }

                script = script.Replace("#NAME#", Selection.activeGameObject.name);
                script = script.Replace("#PROPERTY#", property.ToString());
                script = script.Replace("#GET#", get.ToString());
                File.WriteAllText(basePath, script);

                AssetDatabase.Refresh();
            }
        }
    }
}
