using UnityEditor;
using UnityEngine;

namespace GFramework.Editor
{
    public class EditorUtil
    {
        public static void CallMenuItem(string menuName)
        {
            EditorApplication.ExecuteMenuItem(menuName);
        }

        public static void CopyText(string text)
        {
            GUIUtility.systemCopyBuffer = text;
        }

        public static void OpenInFolder(string folderPath)
        {
            Application.OpenURL("file:///" + folderPath);
        }

        public static void ExportPackage(string assetPathName, string fileName)
        {
            AssetDatabase.ExportPackage(assetPathName, fileName, ExportPackageOptions.Recurse);
        }
    }
}