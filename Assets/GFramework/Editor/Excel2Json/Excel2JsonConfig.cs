using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Editor
{
    public enum Excel2JsonFieldType
    {
        [LabelText("文本")]
        Text = 0,
        [LabelText("文本数组")]
        TextArray = 1,
        [LabelText("整数")]
        NumberInt = 10,
        [LabelText("小数")]
        NumberFloat = 20,
        [LabelText("布尔值")]
        Bool = 30,
    }
    
    [Serializable]
    public class Excel2JsonPropertyData
    {
        [LabelText("Excel字段头")] 
        [LabelWidth(80)]
        public string excelHeader;
        [LabelText("Json属性名")]
        [LabelWidth(70)]
        public string fieldName;
        [LabelText("字段类型")]
        [LabelWidth(50)]
        public Excel2JsonFieldType fieldType;
    }
    
    [CreateAssetMenu(menuName = "游戏数据/Excel转换配置")]
    public class Excel2JsonConfig : SerializedScriptableObject
    {
        #region 字段
        [LabelText("数据名称")]
        public string dataName;
        
        [LabelText("Excel文件地址")]
        [FilePath(AbsolutePath = false,Extensions = "xlsx",RequireExistingPath = true)]
        public string[] inputFiles;

        [LabelText("生成数据类名")]
        public string dataClass;

        [LabelText("唯一键值")]
        public string bindKey;

        [LabelText("字段设置")]
        [InlineProperty]
        [TableList]
        public List<Excel2JsonPropertyData> fieldSetting = new List<Excel2JsonPropertyData>();

        #endregion

        #region 属性



        #endregion

        #region 回调方法



        #endregion

        #region 公共方法



        #endregion

        #region 私有方法



        #endregion


    }
}