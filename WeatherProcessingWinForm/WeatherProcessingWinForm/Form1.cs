using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeatherProcessingWinForm
{
    public partial class Form1 : Form
    {
        string fileName = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }

        #region Button_Click
        // 选择文件
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = "xlsx",
                Filter = "Excel|*.XLSX"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                fileName = dlg.FileName;
                this.textBox1.Text = fileName;
                Log($"读取文件路径{fileName}");
            }

        }

        //读取文件
        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                fileName = this.textBox1.Text.Trim();
                if (string.IsNullOrWhiteSpace(fileName))
                    MessageBox.Show("文件路径不能为空！");
                Stopwatch sw = new();
                sw.Start();
                Log("开始测试【NPOI读取Excel】性能");
                var dt = await Task.Run(() => ExcelToTable(fileName));
                sw.Stop();
                long ts = sw.ElapsedMilliseconds;
                Log($"【NPOI读取Excel】成功，耗时{ts}毫秒；\r\n文件路径{fileName}");
                Log("==============================================================");

                dataGridView1.BeginInvoke(new Action(() => { dataGridView1.DataSource = dt; }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // 文件解析
        private void button3_Click(object sender, EventArgs e)
        {
            Log("开始解析数据");
            var dt = this.dataGridView1.DataSource as DataTable;
            var list = GenerateWeatherData(dt);
            this.dataGridView2.DataSource = list;
            Log("数据解析结束");
        }

        // 导出为txt文件
        private void button4_Click(object sender, EventArgs e)
        {
            Log("开始导出为txt文件");
            var sfd = new SaveFileDialog { DefaultExt = "txt", Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var dt = this.dataGridView2.DataSource as List<Weather>;
                    if (ExportDataToTxt(dt, sfd.FileName))
                    {
                        if (MessageBox.Show("导出文档成功，是否打开？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            OpenFile(sfd.FileName);
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
            Log("导出为txt文件结束");
        }
        #endregion

        #region Methods
        List<string> strHeaders = new() { "设备名称", "地址号", "模拟量1", "模拟量2", "时间", "报警数据" };
        private Task<DataTable> ExcelToTable(string strFileName)
        {
            DataTable dt = new();
            IWorkbook workbook;
            try
            {
                string fileExt = Path.GetExtension(strFileName).ToLower();
                using (FileStream fs = new(strFileName, FileMode.Open, FileAccess.Read))
                {
                    if (fileExt == ".xlsx")
                        workbook = new XSSFWorkbook(fs);
                    else if (fileExt == ".xls")
                        workbook = new HSSFWorkbook(fs);
                    else
                        workbook = null;

                    if (workbook == null)
                        return null;
                    ISheet sheet = workbook.GetSheetAt(0);

                    IRow headerRow = sheet.GetRow(sheet.FirstRowNum + 1);
                    int cellCount = headerRow.LastCellNum;
                    int count = 0;

                    //表头
                    for (int i = 0; i < cellCount; i++)
                    {
                        ICell cell = headerRow.GetCell(i);
                        object obj = GetValueType(cell);
                        if (obj != null && obj.ToString() != string.Empty)
                        {
                            dt.Columns.Add(obj.ToString());
                            count++;
                        }
                    }

                    //数据
                    for (int i = sheet.FirstRowNum + 2; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null)
                            continue;

                        DataRow dr = dt.NewRow();
                        bool hasValue = false;

                        for (int j = 0, k = 0; j < cellCount && k < count; j++)
                        {
                            var tmp = GetValueType(row.GetCell(j));
                            if (tmp != null && tmp.ToString() != string.Empty && !strHeaders.Contains(tmp.ToString())
                                && !tmp.ToString().Contains("Page"))
                            {
                                dr[k++] = tmp;
                                hasValue = true;
                            }
                        }

                        if (hasValue)
                            dt.Rows.Add(dr);
                    }
                }
                return Task.FromResult(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        StringBuilder data = new StringBuilder();
        private bool ExportDataToTxt(List<Weather> dt, string savePath)
        {
            if (dt == null || string.IsNullOrEmpty(savePath.Trim()))
                return false;
            try
            {
                using var fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
                using var sw = new StreamWriter(fs, Encoding.Default);
                int count = dt.Count;
                string headData = string.Empty;

                //表头
                var heads = new List<string>() { "序号", "设备ID", "时间", "地址ID", "光照", "气压", "风向", "风速", "温度", "湿度" };
                foreach (string item in heads)
                {
                    if (item == "设备ID" || item == "时间")
                        headData += item.PadRight(30) + "\t";
                    else
                        headData += item + "\t";
                }
                sw.WriteLine(headData.Trim());

                //表数据
                foreach (Weather weather in dt)
                {
                    AppendDataToStringBuilder(weather);
                }
                sw.Write(data.ToString());
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private object GetValueType(ICell cell)
        {
            if (cell == null)
                return null;
            return cell.CellType switch
            {
                CellType.Unknown or CellType.Blank => null,
                CellType.Numeric => cell.NumericCellValue,
                CellType.String => cell.StringCellValue,
                CellType.Boolean => cell.BooleanCellValue,
                CellType.Error => cell.ErrorCellValue,
                _ => "=" + cell.CellFormula,
            };
        }

        
        private List<Weather> GenerateWeatherData(DataTable dt)
        {
            if (dt == null)
                return null;
            var weatherlst = new List<Weather>();
            string preTime = string.Empty;
            Weather weather = null;
            int count = 1;
            for (int i = 0; i <= dt.Rows.Count; i++)
            {
                if (i == dt.Rows.Count)
                {
                    SetDefaultValue(weather);
                    weatherlst.Add(weather);
                    break;
                }
                DataRow dr = dt.Rows[i];
                string curTime = dr[4].ToString();

                if (!string.Equals(curTime, preTime))
                {
                    if (weather != null)
                    {
                        SetDefaultValue(weather);
                        weatherlst.Add(weather);
                    }
                   
                    weather = new Weather();
                    preTime = curTime;

                    weather.DeviceId = dr[1].ToString().Split('#')[0];
                    weather.AddressID = "1";
                    weather.Serial = count++;
                    weather.Time = curTime;
                }
                switch (dr[0].ToString())
                {
                    case "气压":
                        weather.AirPressure = dr[3].ToString();
                        break;
                    case "光照":
                        weather.Lighting = dr[2].ToString();
                        break;
                    case "风向":
                        weather.WindDirection = dr[2].ToString();
                        break;
                    case "风力（级）":
                        weather.WindSpeed = dr[3].ToString();
                        break;
                    case "温湿度":
                        weather.Temperature = dr[2].ToString();
                        weather.Humidity = dr[3].ToString();
                        break;
                    default:
                        break;
                }
            }
            return weatherlst;
        }

        private void SetDefaultValue(Weather weather)
        {
            if (weather == null)
                return;
            var propertys = weather.GetType().GetProperties();
            foreach (PropertyInfo pi in propertys)
            {
                if (pi.GetValue(weather) == null)
                    pi.SetValue(weather, "99999.99");
            }
        }

        private void AppendDataToStringBuilder(Weather weather)
        {
            if (weather == null)
                return;
            var propertys = weather.GetType().GetProperties();

            foreach (var item in propertys)
            {
                if (item.Name == "Time" || item.Name == "DeviceId")
                    data.Append(item.GetValue(weather).ToString().PadRight(30) + "\t");
                else
                    data.Append(item.GetValue(weather).ToString() + "\t");
            }
            data.Append("\r\n");
        }

        private void Log(string msg)
        {
            this.textBox2.Text += msg + "\r\n";
        }

        private void OpenFile(string filePath)
        {
            //Process.Start("notepad.exe", filePath);
            Process.Start("explorer.exe", filePath);
        }
        #endregion
    }
}
