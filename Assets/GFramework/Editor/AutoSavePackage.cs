using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GFramework.Editor
{
    public class AutoSavePackage
    {
        public static string GeneratePackageName()
        {
            return "GFramework_" + DateTime.Now.ToString("yyyyMMdd_HH");
        }

        [MenuItem("GFramework/自动导出UnityPackage %e")]
        private static void MenuClicked()
        {
            EditorUtil.ExportPackage("Assets/GFramework", GeneratePackageName() + ".unitypackage");
            EditorUtil.OpenInFolder(Path.Combine(Application.dataPath, "../"));
        }
    }
}