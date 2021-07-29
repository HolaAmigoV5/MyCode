﻿using DaJuTestDemo.Core;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace DaJuTestDemo
{
    public class ExcelHelper
    {
        public ExcelHelper() { }

        /// <summary>
        /// 文件流初始化对象
        /// </summary>
        /// <param name="stream"></param>
        public ExcelHelper(Stream stream)
        {
            _IWorkbook = CreateWorkbook(stream);
        }

        /// <summary>
        /// 传入文件名
        /// </summary>
        /// <param name="fileName"></param>
        public ExcelHelper(string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                _IWorkbook = CreateWorkbook(fileStream);
            }
        }

        /// <summary>
        /// 工作薄
        /// </summary>
        private IWorkbook _IWorkbook;

        /// <summary>
        /// 创建工作簿对象
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private IWorkbook CreateWorkbook(Stream stream)
        {
            try
            {
                return new XSSFWorkbook(stream); //07
            }
            catch
            {
                return new HSSFWorkbook(stream); //03
            }

        }

        /// <summary>
        /// 把Sheet中的数据转换为DataTable
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private DataTable ExportToDataTable(ISheet sheet)
        {
            DataTable dt = new DataTable();

            //默认，第一行是字段
            IRow headRow = sheet.GetRow(0);

            //设置datatable字段
            for (int i = headRow.FirstCellNum, len = headRow.LastCellNum; i < len; i++)
            {
                dt.Columns.Add(headRow.Cells[i].StringCellValue);
            }
            //遍历数据行
            for (int i = sheet.FirstRowNum + 1, len = sheet.LastRowNum + 1; i < len; i++)
            {
                IRow tempRow = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();

                //遍历一行的每一个单元格
                for (int r = 0, j = tempRow.FirstCellNum, len2 = tempRow.LastCellNum; j < len2; j++, r++)
                {

                    ICell cell = tempRow.GetCell(j);

                    if (cell != null)
                    {
                        dataRow[r] = cell.CellType switch
                        {
                            CellType.String => cell.StringCellValue,
                            CellType.Numeric => cell.NumericCellValue,
                            CellType.Boolean => cell.BooleanCellValue,
                            _ => "ERROR",
                        };
                    }
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        int count = 0;
        /// <summary>
        /// Sheet中的数据转换为List集合
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private async Task<IList<T>> ExportToListAsync<T>(ISheet sheet, string[] fields) where T : class, new()
        {
            try
            {
                IList<T> list = new List<T>();
                await Task.Run(() =>
                {
                    //遍历每一行数据
                    for (int i = sheet.FirstRowNum + 1, len = sheet.LastRowNum + 1; i < len; i++)
                    {
                        count++;
                        T t = new T();
                        IRow row = sheet.GetRow(i);

                        for (int j = 0, len2 = fields.Length; j < len2; j++)
                        {
                            ICell cell = row.GetCell(j);
                            if (cell != null)
                            {
                                object cellValue = cell.CellType switch
                                {
                                    CellType.String => cell.StringCellValue,        //文本

                                    CellType.Numeric => cell.NumericCellValue,     //数值

                                    CellType.Boolean => cell.BooleanCellValue,      //bool

                                    CellType.Formula => cell.NumericCellValue,

                                    CellType.Blank => "",       //空白
                                    _ => "ERROR",
                                };
                                typeof(T).GetProperty(fields[j]).SetValue(t, cellValue, null);
                            }
                        }
                        list.Add(t);
                    }
                });

                return list;
            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error(ex, $"执行ExportToList错误，错误行：{count}");
                throw ex;
            }
        }

        /// <summary>
        /// 获取第一个Sheet的第X行，第Y列的值。起始点为1
        /// </summary>
        /// <param name="X">行</param>
        /// <param name="Y">列</param>
        /// <returns></returns>
        public string GetCellValue(int X, int Y)
        {
            ISheet sheet = _IWorkbook.GetSheetAt(0);

            IRow row = sheet.GetRow(X - 1);

            return row.GetCell(Y - 1).ToString();
        }

        /// <summary>
        /// 获取一行的所有数据
        /// </summary>
        /// <param name="X">第x行</param>
        /// <returns></returns>
        public string[] GetCells(int X)
        {
            List<string> list = new List<string>();

            ISheet sheet = _IWorkbook.GetSheetAt(0);

            IRow row = sheet.GetRow(X - 1);

            for (int i = 0, len = row.LastCellNum; i < len; i++)
            {
                list.Add(row.GetCell(i).StringCellValue);//这里没有考虑数据格式转换，会出现bug
            }
            return list.ToArray();
        }

        /// <summary>
        /// 第一个Sheet数据，转换为DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable ExportExcelToDataTable()
        {
            return ExportToDataTable(_IWorkbook.GetSheetAt(0));
        }

        /// <summary>
        /// 第sheetIndex表数据，转换为DataTable
        /// </summary>
        /// <param name="sheetIndex">第几个Sheet，从1开始</param>
        /// <returns></returns>
        public DataTable ExportExcelToDataTable(int sheetIndex)
        {
            return ExportToDataTable(_IWorkbook.GetSheetAt(sheetIndex - 1));
        }


        /// <summary>
        /// Excel中默认第一张Sheet导出到集合
        /// </summary>
        /// <param name="fields">Excel各个列，依次要转换成为的对象字段名称</param>
        /// <returns></returns>
        public async Task<IList<T>> ExcelToListAsync<T>(string[] fields) where T : class, new()
        {
            return await ExportToListAsync<T>(_IWorkbook.GetSheetAt(0), fields);
        }

        /// <summary>
        /// Excel中指定的Sheet导出到集合
        /// </summary>
        /// <param name="sheetIndex">第几张Sheet,从1开始</param>
        /// <param name="fields">Excel各个列，依次要转换成为的对象字段名称</param>
        /// <returns></returns>
        public async Task<IList<T>> ExcelToList<T>(int sheetIndex, string[] fields) where T : class, new()
        {
            return await ExportToListAsync<T>(_IWorkbook.GetSheetAt(sheetIndex - 1), fields);
        }
    }
}
