using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WbyCodeLibrary.ExcelOperation
{
    public class NpoiExcelOperationService
    {
        /// <summary>
        /// Excel数据导出
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="excelFullPath">导出完整路径</param>
        /// <param name="excelFilePath">保存excel文件路径</param>
        /// <returns></returns>
        public bool ExcelDataExport(List<ControlElementDto> data, out string excelFullPath, string excelFilePath, string elementName)
        {
            excelFullPath = "";
            if (data.Count == 0)
            {
                return false;
            }

            bool result = true;
            List<ControlElementDetail> detail;

            //Excel导出名称
            string excelName = "管控要素审查报告";
            try
            {
                //首先创建Excel文件对象
                HSSFWorkbook workbook = new HSSFWorkbook();

                //创建工作表，也就是Excel中的sheet，给工作表赋一个名称(Excel底部名称)
                ISheet sheet = workbook.CreateSheet("审查报告");
                sheet.DefaultColumnWidth = 20;//默认列宽
                sheet.SetColumnWidth(0, 3 * 256);

                #region table 表格内容设置

                #region 标题样式

                //设置顶部大标题样式
                var cellStyleFont = NpoiExcelHelper.ExcelHelper.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 26, false, true, "楷体");

                //第二行表单
                HSSFRow row = NpoiExcelHelper.ExcelHelper.CreateRow(sheet, 1, 34);
                ICell cell = row.CreateCell(1);


                //TODO:关于Excel行列单元格合并问题
                /**
                  第一个参数：从第几行开始合并
                  第二个参数：到第几行结束合并
                  第三个参数：从第几列开始合并
                  第四个参数：到第几列结束合并
                **/
                CellRangeAddress region = new CellRangeAddress(1, 1, 1, 6);
                sheet.AddMergedRegion(region);

                cell.SetCellValue("管控要素审查报告");  //合并单元格后，只需对第一个位置赋值即可（TODO:顶部标题）
                cell.CellStyle = cellStyleFont;

                //二级标题列样式设置
                HSSFCellStyle headTopStyle = NpoiExcelHelper.ExcelHelper.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 15,
                    true, true, "楷体", true, false, false, true, FillPattern.SolidForeground, HSSFColor.Grey25Percent.Index, HSSFColor.Black.Index,
                    FontUnderlineType.None, FontSuperScript.None, false);

                //表头名称
                string[] headerName = new[] { "要素类", "要素子类", "要素子类", "控制要求", "图片", "审核状态" };
                row = NpoiExcelHelper.ExcelHelper.CreateRow(sheet, 2, 24);      //第三行
                for (int i = 0; i < headerName.Length; i++)
                {
                    cell = NpoiExcelHelper.ExcelHelper.CreateCells(row, headTopStyle, i + 1, headerName[i]);

                    //设置单元格宽度
                    if (headerName[i] == "控制要求")
                    {
                        sheet.SetColumnWidth(4, 20000);
                    }
                    else if (headerName[i] == "图片")
                    {
                        sheet.SetColumnWidth(5, 11000);
                    }
                }
                #endregion

                #region 单元格内容信息

                //单元格边框样式
                HSSFCellStyle CenterboldWithBorderStyle = NpoiExcelHelper.ExcelHelper.CreateStyle(workbook, HorizontalAlignment.Center,
                    VerticalAlignment.Center, 11, true, true);

                HSSFCellStyle RightboldNoBorderStyle = NpoiExcelHelper.ExcelHelper.CreateStyle(workbook, HorizontalAlignment.Right,
                    VerticalAlignment.Center, 11, false, true);

                HSSFCellStyle LeftboldNoBorderStyle = NpoiExcelHelper.ExcelHelper.CreateStyle(workbook, HorizontalAlignment.Left,
                    VerticalAlignment.Center, 11, false, true);

                HSSFCellStyle CenterBoldNoBorderStyle = NpoiExcelHelper.ExcelHelper.CreateStyle(workbook, HorizontalAlignment.Center,
                    VerticalAlignment.Center, 11, false, true);

                // 控制要求和图片样式
                HSSFCellStyle contentStyle = NpoiExcelHelper.ExcelHelper.CreateStyle(workbook, HorizontalAlignment.Left, VerticalAlignment.Top,
                    10, true, false, isLineFeed: true);

                /**
                  第一个参数：从第几行开始合并
                  第二个参数：到第几行结束合并
                  第三个参数：从第几列开始合并
                  第四个参数：到第几列结束合并
                **/
                // 第一列：要素类（要合并）
                int count = 0; // 数据总行数
                data.ForEach(m =>
                {
                    count += m.ControlElementDetails.Count;
                });
                CellRangeAddress leftOne = new CellRangeAddress(3, count + 2, 1, 1);
                sheet.AddMergedRegion(leftOne);

                int preCount = 2;
                int k = 0;
                for (int i = 0; i < data.Count; i++)
                {
                    detail = data[i].ControlElementDetails;
                    int subCount = detail.Count;

                    // 第二列：要素子类（要合并）
                    if (subCount > 1)
                    {
                        CellRangeAddress leftTwo = new CellRangeAddress(preCount + 1, subCount + preCount, 2, 2);
                        sheet.AddMergedRegion(leftTwo);
                    }
                    preCount += subCount;

                    // 要素详情
                    for (int j = 0; j < subCount; j++)
                    {
                        k++;
                        row = NpoiExcelHelper.ExcelHelper.CreateRow(sheet, k + 2, 20);
                        if (k == 1)
                        {
                            cell = NpoiExcelHelper.ExcelHelper.CreateCells(row, CenterBoldNoBorderStyle, 1, elementName);
                            cell = NpoiExcelHelper.ExcelHelper.CreateCells(row, CenterBoldNoBorderStyle, 2, data[i].ControlElementName);
                        }
                        else if (k > 1 && preCount - subCount - 1 == k)
                        {
                            cell = NpoiExcelHelper.ExcelHelper.CreateCells(row, CenterBoldNoBorderStyle, 2, data[i].ControlElementName);
                        }

                        cell = NpoiExcelHelper.ExcelHelper.CreateCells(row, CenterboldWithBorderStyle, 3, detail[j].Name);         // 要素子类2
                        cell = NpoiExcelHelper.ExcelHelper.CreateCells(row, contentStyle, 4, detail[j].Description);              // 控制要求

                        if (!string.IsNullOrEmpty(detail[j].Image))
                        {
                            //前四个参数(dx1,dy1,dx2,dy2)为图片在单元格的边距
                            //col1,col2表示图片插在col1和col2之间的单元格，索引从0开始
                            //row1,row2表示图片插在第row1和row2之间的单元格，索引从1开始
                            // 参数的解析: HSSFClientAnchor（int dx1,int dy1,int dx2,int dy2,int col1,int row1,int col2,int row2)
                            //dx1:图片左边相对excel格的位置(x偏移) 范围值为:0~1023;即输100 偏移的位置大概是相对于整个单元格的宽度的100除以1023大概是10分之一
                            //dy1:图片上方相对excel格的位置(y偏移) 范围值为:0~256 原理同上。
                            //dx2:图片右边相对excel格的位置(x偏移) 范围值为:0~1023; 原理同上。
                            //dy2:图片下方相对excel格的位置(y偏移) 范围值为:0~256 原理同上。
                            //col1和row1 :图片左上角的位置，以excel单元格为参考,比喻这两个值为(1,1)，那么图片左上角的位置就是excel表(1,1)单元格的右下角的点(A,1)右下角的点。
                            //col2和row2:图片右下角的位置，以excel单元格为参考,比喻这两个值为(2,2)，那么图片右下角的位置就是excel表(2,2)单元格的右下角的点(B,2)右下角的点。

                            string image = $"{Environment.CurrentDirectory}\\Pic\\{detail[j].Image}.png";
                            cell = NpoiExcelHelper.ExcelHelper.CreateCells(row, contentStyle, 5, detail[j].Image);      // 图片
                            row.HeightInPoints = 127;

                            InsertImgToCell(image, workbook, sheet, 5, k + 2, 6, k + 3);
                        }
                        else
                            cell = NpoiExcelHelper.ExcelHelper.CreateCells(row, contentStyle, 5, "");

                        cell = NpoiExcelHelper.ExcelHelper.CreateCells(row, contentStyle, 6, "");               // 审核状态
                    }
                }

                // 底部日期
                row = NpoiExcelHelper.ExcelHelper.CreateRow(sheet, count + 3, 20);
                cell = NpoiExcelHelper.ExcelHelper.CreateCells(row, RightboldNoBorderStyle, 4, "审查日期：");
                cell = NpoiExcelHelper.ExcelHelper.CreateCells(row, LeftboldNoBorderStyle, 5, DateTime.Now.ToString("yyyy-MM-dd"));

                #endregion

                SetAutoRowHeight(sheet, 4, 5);  // 根据第4,5列设置自动行高
                sheet.CreateFreezePane(0, 3);   // 冻结前3行
                #endregion

                //excel保存文件名
                string excelFileName = excelName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

                //创建目录文件夹
                if (!Directory.Exists(excelFilePath))
                {
                    Directory.CreateDirectory(excelFilePath);
                }

                //Excel的路径及名称
                string excelPath = Path.Combine(excelFilePath, excelFileName);

                //使用FileStream文件流来写入数据（传入参数为：文件所在路径，对文件的操作方式，对文件内数据的操作）
                FileStream fileStream = new FileStream(excelPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                //向Excel文件对象写入文件流，生成Excel文件
                workbook.Write(fileStream);

                //关闭文件流
                fileStream.Close();

                //释放流所占用的资源
                fileStream.Dispose();
                excelFullPath = excelPath;
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        private void SetAutoRowHeight(ISheet sheet, int cellNum1, int cellNum2)
        {
            for (int rowNum = 3; rowNum <= sheet.LastRowNum; rowNum++)
            {
                IRow currentRow = sheet.GetRow(rowNum);
                ICell currentCell = currentRow.GetCell(cellNum1);
                ICell currentCell2 = currentRow.GetCell(cellNum2);
                int length = Encoding.UTF8.GetBytes(currentCell.ToString()).Length;
                int length2 = Encoding.UTF8.GetBytes(currentCell2.ToString()).Length;
                if (length2 == 0)
                    currentRow.HeightInPoints = length > 0 ? 20 * ((length / 130) + 1) : 20;
            }
        }

        /// <summary>
        /// 将图片插入到Excel指定单元格中
        /// </summary>
        /// <param name="imgFullPath">图片完整路径</param>
        /// <param name="workbook">Excel工作簿</param>
        /// <param name="sheet">Excel表区间</param>
        /// <param name="col1">图片左上角的位置</param>
        /// <param name="row1">图片左上角的位置</param>
        /// <param name="col2">图片右下角的位置</param>
        /// <param name="row2">图片右下角的位置</param>
        //col1和row1 :图片左上角的位置，以excel单元格为参考,比喻这两个值为(1,1)，那么图片左上角的位置就是excel表(1,1)单元格的右下角的点(A,1)右下角的点。
        //col2和row2:图片右下角的位置，以excel单元格为参考,比喻这两个值为(2,2)，那么图片右下角的位置就是excel表(2,2)单元格的右下角的点(B,2)右下角的点。
        private void InsertImgToCell(string imgFullPath, HSSFWorkbook workbook, ISheet sheet, int col1, int row1, int col2, int row2)
        {
            byte[] bytes = File.ReadAllBytes(imgFullPath);
            int pictureIdx = workbook.AddPicture(bytes, PictureType.PNG);
            IDrawing patriarch = sheet.CreateDrawingPatriarch();

            HSSFClientAnchor anchor = new(10, 10, 0, 0, col1, row1, col2, row2);
            _ = patriarch.CreatePicture(anchor, pictureIdx);
        }
    }
}
