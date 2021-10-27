using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class Excel2JsonEditor
    {
        #region 字段

        public const string CONFIG_PATH = "Assets/Scripts/Data/Config";
        public const string SCRIPT_TAG_PATH = "Assets/Scripts/Data/GenData";
        public static string DataTagPath => "Assets/Scripts/Data/Json";
        
        public const string SCRIPT_TEMPLATE =
            @"using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// {2}
/// </summary>
public partial class {0} : IData
{{
    {1}

    public string DataID => this.{3};
}}
";

        public const string SCRIPT_FIELD_TEMPLATE =
            @"
    /// <summary>
    /// {2}
    /// </summary>
    public {0} {1};
";
        
        
        #endregion

        #region 属性



        #endregion

        #region 回调方法



        #endregion

        #region 公共方法

        [MenuItem("游戏数据/Excel数据/生成数据",priority = 0)]
        public static void GenerateExcelData()
        {
            ClearExcelData();
            foreach (var file in Directory.GetFiles(CONFIG_PATH))
            {
                if(file.EndsWith(".meta",StringComparison.OrdinalIgnoreCase))
                    continue;
                var dataConfig = AssetDatabase.LoadAssetAtPath<Excel2JsonConfig>(file);
                Debug.Log($"读取配置 {file}");
                if(dataConfig!=null)
                {
                    GenerateExcelData(dataConfig);
                }
            }
            AssetDatabase.Refresh();
        }
        
        [MenuItem("游戏数据/Excel数据/生成数据 [刷新脚本]",priority = 1)]
        public static void GenerateAllData()
        {
            ClearAllData();
            foreach (var file in Directory.GetFiles(CONFIG_PATH))
            {
                if(file.EndsWith(".meta",StringComparison.OrdinalIgnoreCase))
                    continue;
                var dataConfig = AssetDatabase.LoadAssetAtPath<Excel2JsonConfig>(file);
                Debug.Log($"读取配置 {file}");
                if(dataConfig!=null)
                {
                    GenerateExcelData(dataConfig);
                    GenerateClassData(dataConfig);
                }
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("游戏数据/Excel数据/清理数据",priority = 20)]
        public static void ClearAllData()
        {
            if(Directory.Exists(DataTagPath))
                Directory.Delete(DataTagPath,true);
            if(Directory.Exists(SCRIPT_TAG_PATH))
                Directory.Delete(SCRIPT_TAG_PATH,true);
            
            Directory.CreateDirectory(DataTagPath);
            Directory.CreateDirectory(SCRIPT_TAG_PATH);
            AssetDatabase.Refresh();
        }
        
        public static void ClearExcelData()
        {
            if(Directory.Exists(DataTagPath))
                Directory.Delete(DataTagPath,true);

            Directory.CreateDirectory(DataTagPath);
            AssetDatabase.Refresh();
        }
        
        #endregion

        #region 私有方法

        public static void GenerateExcelData(Excel2JsonConfig dataConfig)
        {
            foreach (var inputFile in dataConfig.inputFiles)
            {
                using (var stream = File.Open(inputFile, FileMode.Open, FileAccess.Read,FileShare.ReadWrite))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet();
                        var table = result.Tables[0];

                        Dictionary<string, int> headerPos = new Dictionary<string, int>();
                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            var header = table.Rows[0][i].ToString();
                            headerPos.Add(header,i);
                        }

                        List<JObject> jsonDic = new List<JObject>();

                        for (int i = 1; i < table.Rows.Count; i++)
                        {
                            JObject rowData = new JObject();
                            foreach (var data in dataConfig.fieldSetting)
                            {
                                if (headerPos.TryGetValue(data.excelHeader, out var pos))
                                {
                                    var getData =  table.Rows[i][pos];
                                    switch (data.fieldType)
                                    {
                                        case Excel2JsonFieldType.Text:
                                            rowData.Add(data.fieldName,
                                                JToken.FromObject(Convert.ToString(getData)));
                                            break;
                                        case Excel2JsonFieldType.TextArray:
                                            rowData.Add(data.fieldName,
                                                JToken.FromObject(Convert.ToString(getData).Split('\n')));
                                            break;
                                        case Excel2JsonFieldType.NumberInt:
                                            rowData.Add(data.fieldName,
                                                JToken.FromObject(Convert.ToInt32(getData)));
                                            break;
                                        case Excel2JsonFieldType.NumberFloat:
                                            rowData.Add(data.fieldName,
                                                JToken.FromObject(Convert.ToSingle(getData)));
                                            break;
                                        case Excel2JsonFieldType.Bool:
                                            rowData.Add(data.fieldName,
                                                JToken.FromObject(Convert.ToInt32(getData) != 0));
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                }
                            }

                            jsonDic.Add(rowData);
                        
                        }

                        var jsonData = JToken.FromObject(jsonDic);
                        var tagDir = $"{DataTagPath}/{dataConfig.dataClass}";
                        Directory.CreateDirectory(tagDir);
                        var path = $"{tagDir}/{Path.GetFileNameWithoutExtension(inputFile)}.json";
                        Debug.Log($"生成数据 [{dataConfig.dataClass}] {path}");
                        File.WriteAllText(path,
                            jsonData.ToString(Formatting.Indented));
                        
                    }
                }
            }
        }

        public static void GenerateClassData(Excel2JsonConfig dataConfig)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var propertyData in dataConfig.fieldSetting)
            {
                switch (propertyData.fieldType)
                {
                    case Excel2JsonFieldType.Text:
                        stringBuilder.AppendFormat(SCRIPT_FIELD_TEMPLATE, "string", 
                            propertyData.fieldName,propertyData.excelHeader);
                        break;
                    case Excel2JsonFieldType.TextArray:
                        stringBuilder.AppendFormat(SCRIPT_FIELD_TEMPLATE, "string[]", 
                            propertyData.fieldName,propertyData.excelHeader);
                        break;
                    case Excel2JsonFieldType.NumberInt:
                        stringBuilder.AppendFormat(SCRIPT_FIELD_TEMPLATE, "int", 
                            propertyData.fieldName,propertyData.excelHeader);
                        break;
                    case Excel2JsonFieldType.NumberFloat:
                        stringBuilder.AppendFormat(SCRIPT_FIELD_TEMPLATE, "float",
                            propertyData.fieldName,propertyData.excelHeader);
                        break;
                    case Excel2JsonFieldType.Bool:
                        stringBuilder.AppendFormat(SCRIPT_FIELD_TEMPLATE, "bool",
                            propertyData.fieldName,propertyData.excelHeader);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            string classText = string.Format(SCRIPT_TEMPLATE, dataConfig.dataClass, 
                stringBuilder,dataConfig.dataName,dataConfig.bindKey);
            var path = $"{SCRIPT_TAG_PATH}/{dataConfig.dataClass}.cs";
            Debug.Log($"生成类型 [{dataConfig.dataClass}] {path}");
            File.WriteAllText(path,classText);
        }

        #endregion

        
    }
}