using System;
using System.Data;
using System.IO;
using System.Text;
using Aspose.Cells;

namespace My.Util
{
    /// <summary>
    /// 描述：使用Aspose组件的Office文件操作帮助类
    /// 作者：wby 2019/10/23 8:16:15
    /// </summary>
    public class AsposeOfficeHelper
    {
        static AsposeOfficeHelper()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// 将DataTable输出为字节数组
        /// </summary>
        /// <param name="dt">表格数据</param>
        /// <returns>byte[]数组</returns>
        public static byte[] DataTableToExcelBytes(DataTable dt)
        {
            Workbook book = new Workbook();
            Worksheet sheet = book.Worksheets[0];
            Cells cells = sheet.Cells;
            int columnNum = dt.Columns.Count;  //表格列数
            int rowNum = dt.Rows.Count; //表格行数

            //生成列名
            for (int i = 0; i < columnNum; i++)
            {
                cells[0, i].PutValue(dt.Columns[i].ColumnName);
            }

            //行列中填充数据
            for (int i = 0; i < rowNum; i++)
            {
                for (int k = 0; k < columnNum; k++)
                {
                    cells[1 + i, k].PutValue(dt.Rows[i][k].ToString());
                }
            }

            //自动行高，列宽
            sheet.AutoFitRows();
            sheet.AutoFitColumns();

            //将DataTable写入内存流
            var ms = new MemoryStream();
            book.Save(ms, SaveFormat.Excel97To2003);
            return ms.ToArray();
        }

        /// <summary>
        /// 通过模板导出Excel
        /// </summary>
        /// <param name="templateFile">模板</param>
        /// <param name="dataSource">数据源</param>
        /// <returns></returns>
        public static byte[] ExportExcelByTemplate(string templateFile,params (string SourceName,object Data)[] dataSource)
        {
            if (templateFile.IsNullOrEmpty())
                throw new ArgumentNullException("模板不能为空!");
            if (dataSource?.Length == 0)
                throw new ArgumentNullException("数据源不能为空!");

            WorkbookDesigner designer = new WorkbookDesigner
            {
                Workbook = new Workbook(templateFile)
            };
            var workBook = designer.Workbook;

            dataSource.ForEach(aDataSource => {
                designer.SetDataSource(aDataSource.SourceName, aDataSource.Data);
            });

            designer.Process();

            using (MemoryStream ms = new MemoryStream())
            {
                workBook.Save(ms, SaveFormat.Excel97To2003);
                var fileBytes = ms.ToArray();
                return fileBytes;
            }
        }

        /// <summary>
        /// 从excel文件导入数据
        /// 注：默认将第一行当作标题行，即不当作数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static DataTable ReadExcel(string fileName, bool exportColumnName = true)
        {
            Workbook book = new Workbook(fileName);
            Worksheet sheet = book.Worksheets[0];
            Cells cells = sheet.Cells;

            return cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, exportColumnName);
        }

        /// <summary>
        /// 从excel文件字节源导入
        /// </summary>
        /// <param name="fileBytes">文件字节源</param>
        /// <param name="exportColumnName">是否将第一行当作标题行</param>
        /// <returns></returns>
        public static DataTable ReadExcel(byte[] fileBytes, bool exportColumnName = true)
        {
            using (MemoryStream ms = new MemoryStream(fileBytes))
            {
                Workbook book = new Workbook(ms);
                Worksheet sheet = book.Worksheets[0];
                Cells cells = sheet.Cells;

                return cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, exportColumnName);
            }
        }
    }
}
