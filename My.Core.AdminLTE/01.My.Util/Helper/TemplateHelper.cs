using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;
using System.Linq;

namespace My.Util
{
    /// <summary>
    /// 描述：通过模板生成代码
    /// 作者：wby 2019/11/28 16:34:50
    /// </summary>
    public class TemplateHelper
    {
        #region 公有成员
        public static DbHelper DbHelper { get; set; }

        /// <summary>
        /// 生成实体文件
        /// </summary>
        /// <param name="infos">表字段信息</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableDescription">表描述信息</param>
        /// <param name="filePath">文件路径（包含文件名）</param>
        /// <param name="nameSpace">实体命名空间</param>
        /// <param name="schemaName">架构（模式）名</param>
        public static void SaveEntityToFile(List<TableInfo> infos, string tableName,
            string tableDescription, string filePath, string nameSpace, string schemaName = null)
        {
            StringBuilder sb = new StringBuilder();
            string schema = "";
            if (!schemaName.IsNullOrEmpty())
                schema = $@", Schema=""{schemaName}""";
            infos.ForEach((item) =>
            {
                Type type = DbHelper?.DbTypeStr_To_CsharpType(item.ColumnType);
                sb.AppendLine(GenerateEntityProperty(item, type));
            });

            var content = ReadTemplate("ModelTemplate.txt");
            content = content.Replace("{GeneratorTime}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                .Replace("{ModelsNamespace}", nameSpace)
                .Replace("{Author}", "wby")
                .Replace("{Comment}", tableDescription)
                .Replace("{ModelName}", tableName)
                .Replace("{ModelProperties}", sb.ToString());

            FileHelper.WriteTxt(content, filePath, Encoding.UTF8, FileMode.Create);
        }

        /// <summary>
        /// 生成IBusiness文件
        /// </summary>
        /// <param name="areaName">命名空间</param>
        /// <param name="entityName">实体名</param>
        /// <param name="contentRootPath">当前根目录</param>
        public static void BuildIBusiness(string areaName,string entityName, string contentRootPath)
        {
            string className = $"I{entityName}Business";

            var content = ReadTemplate("IBusinessTemplate.txt");
            content = content.Replace("Comment", className)
                .Replace("{Author}", "wby")
                .Replace("{GeneratorTime}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                .Replace("{areaName}", areaName)
                .Replace("{className}", className)
                .Replace("{entityName}", entityName);

            string rootPath = contentRootPath.Replace("My.Web", "My.Business");
            string filePath = Path.Combine(rootPath, "IBusiness", areaName, $"{className}.cs");

            FileHelper.WriteTxt(content, filePath, Encoding.UTF8, FileMode.Create);
        }

        /// <summary>
        /// 生成Business文件
        /// </summary>
        /// <param name="areaName">命名空间</param>
        /// <param name="entityName">实体名</param>
        /// <param name="contentRootPath">当前根目录</param>
        public static void BuildBusiness(string areaName, string entityName, string contentRootPath)
        {
            string className = $"{entityName}Business";
            var content = ReadTemplate("BusinessTemplate.txt");
            content = content.Replace("Comment", className)
                .Replace("{Author}", "wby")
                .Replace("{GeneratorTime}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                .Replace("{areaName}", areaName)
                .Replace("{className}", className)
                .Replace("{entityName}", entityName);

            string rootPath = contentRootPath.Replace("My.Web", "My.Business");
            string filePath = Path.Combine(rootPath, "Business", areaName, $"{className}.cs");

            FileHelper.WriteTxt(content, filePath, Encoding.UTF8, FileMode.Create);
        }

        /// <summary>
        /// 生成Controller文件
        /// </summary>
        /// <param name="areaName">命名空间</param>
        /// <param name="entityName">实体名</param>
        /// <param name="contentRootPath">当前根目录</param>
        public static void BuildController(string areaName, string entityName, string contentRootPath)
        {
            string ibusName = $"I{entityName}Business";
            string varBusiness = $@"{entityName.ToFirstLowerStr()}Bus";
            string _varBusiness = $@"_{entityName.ToFirstLowerStr()}Bus";

            var content = ReadTemplate("ControllerTemplate.txt");
            content = content.Replace("{areaName}", areaName)
                .Replace("{entityName}", entityName)
                .Replace("{ibusName}", ibusName)
                .Replace("{varBusiness}", varBusiness)
                .Replace("{_varBusiness}", _varBusiness);

            string rootPath = contentRootPath;
            string filePath = $@"{rootPath}Areas\{areaName}\Controllers\{entityName}Controller.cs";

            FileHelper.WriteTxt(content, filePath, FileMode.Create);
        }

        /// <summary>
        /// 生成View文件
        /// </summary>
        /// <param name="tableInfoList">表格信息</param>
        /// <param name="areaName">命名空间</param>
        /// <param name="entityName">实体名</param>
        /// <param name="contentRootPath">当前根目录</param>
        public static void BuildView(List<TableInfo> tableInfoList, string areaName, string entityName, string contentRootPath)
        {
            //生成Index页面
            StringBuilder searchConditionSelectHtml = new StringBuilder();
            StringBuilder tableColsBuilder = new StringBuilder();
            StringBuilder formRowBuilder = new StringBuilder();

            var formHeight = tableInfoList.Where(x => x.ColumnName != "Id").Count() * 2;
            if (formHeight > 8)
                formHeight = 8;

            tableInfoList.Where(x => x.ColumnName != "Id").ForEach((aField, index) =>
            {
                //搜索的下拉选项
                Type fieldType = DbHelper?.DbTypeStr_To_CsharpType(aField.ColumnType);
                if (fieldType == typeof(string))
                {
                    string newOption = $@"<option value=""{aField.ColumnName}"">{aField.ColumnDescription}</option>";
                    searchConditionSelectHtml.Append(newOption);
                }

                //数据表格列
                string newCol = $@"{{title: '{aField.ColumnDescription}', field: '{aField.ColumnName}', width:'5%'}},";
                tableColsBuilder.Append(newCol);

                string newFormRow = $@"
                        <div class=""form-group form-group-sm"">
                            <label class=""col-sm-2 control-label"">{aField.ColumnDescription}</label>
                            <div class=""col-sm-5"">
                                <input name=""{aField.ColumnName}"" value=""@obj.{aField.ColumnName}"" type=""text"" class=""form-control"" required>
                                <div class=""help-block with-errors""></div>
                            </div>
                        </div>";

                formRowBuilder.Append(newFormRow);
            });

            var indexHtml = ReadTemplate("IndexHtmlTemplate.txt");
            indexHtml = indexHtml.Replace("{areaName}", areaName)
                .Replace("{entityName}", entityName)
                .Replace("{searchConditionSelectHtml}", searchConditionSelectHtml.ToString())
                .Replace("{tableColsBuilder}", tableColsBuilder.ToString())
                .Replace("{formHeight}", formHeight.ToString());

            string rootPath = contentRootPath;
            string indexPath = $@"{rootPath}Areas\{areaName}\Views\{entityName}\Index.cshtml";
            FileHelper.WriteTxt(indexHtml, indexPath);


            //生成Form页面
            var formHtml = ReadTemplate("FormHtmlTemplate.txt");
            formHtml = formHtml.Replace("{areaName}", areaName)
                .Replace("{entityName}", entityName)
                .Replace("{formRowBuilder}", formRowBuilder.ToString());

            string formPath = $@"{rootPath}Areas\{areaName}\Views\{entityName}\Form.cshtml";
            FileHelper.WriteTxt(formHtml, formPath);
        }
        #endregion

        #region 私有成员
        /// <summary>
        /// 生成属性
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="column">列</param>
        /// <returns></returns>
        private static string GenerateEntityProperty(TableInfo tableInfo, Type columnType)
        {
            var sb = new StringBuilder();
            if (tableInfo != null && columnType != null)
            {
                sb.AppendLine("\t\t/// <summary>");
                sb.AppendLine("\t\t/// " + tableInfo.ColumnDescription);
                sb.AppendLine("\t\t/// </summary>");

                if (tableInfo.IsKey)
                {
                    sb.AppendLine("\t\t[Key]");
                    sb.AppendLine($"\t\tpublic {columnType} Id " + "{get;set;}");
                }
                else
                {
                    if (tableInfo.IsNullable)
                        sb.AppendLine($"\t\tpublic {columnType.Name}? {tableInfo.ColumnName} " + "{get;set;}");
                    else
                    {
                        sb.AppendLine("\t\t[Required]");
                        sb.AppendLine($"\t\tpublic {columnType.Name} {tableInfo.ColumnName} " + "{get;set;}");
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 读取模板
        /// </summary>
        /// <param name="templateName">模板名称</param>
        /// <returns></returns>
        private static string ReadTemplate(string templateName)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var content = string.Empty;
            using (var stream = currentAssembly.GetManifestResourceStream
                ($"{currentAssembly.GetName().Name}.CodeTemplate.{templateName}"))
            {
                if (stream != null)
                    using (var reader = new StreamReader(stream))
                        content = reader.ReadToEnd();
            }
            return content;
        } 
        #endregion
    }
}
