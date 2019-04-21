using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExcelParser.Serializer;
using NPOI.SS.UserModel;

namespace ExcelParser
{
    public class Parser
    {
        public static readonly List<string> SUPPORTED_EXTENSIONS = new List<string>() { ".xls", ".xlsx" };

        public void Parse(ParseParam param)
        {
            if (!Directory.Exists(param.ExcelDir))
            {
                param.Logger?.LogError($"找不到Excel目录:{param.ExcelDir}");
                return;
            }

            List<ExcelSheet> sheets = GetAllExcels(param);
            foreach (var sheet in sheets)
            {
                sheet.ParseFields();
            }

            ISerializer serializer = new JsonSerializer();
            foreach (var sheet in sheets)
            {
                sheet.Serialize(serializer, param);
            }
        }

        private List<ExcelSheet> GetAllExcels(ParseParam param)
        {
            List<ExcelSheet> sheets = new List<ExcelSheet>();
            var allConfigs = Directory.GetFiles(param.ExcelDir, "*.*", SearchOption.TopDirectoryOnly).Where(s => !string.IsNullOrEmpty(Path.GetExtension(s)) && SUPPORTED_EXTENSIONS.Contains(Path.GetExtension(s).ToLower()) && !s.Contains("~"));
            foreach (var configPath in allConfigs)
            {
                string workbookName = Path.GetFileNameWithoutExtension(configPath);
                using (FileStream fs = new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    IWorkbook workbook = WorkbookFactory.Create(fs);
                    for (int sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
                    {
                        ISheet sheet = workbook.GetSheetAt(sheetIndex);
                        if (sheet.PhysicalNumberOfRows == 0)
                            continue;

                        for (int rowIndex = sheet.FirstRowNum; rowIndex <= sheet.LastRowNum; rowIndex++)
                        {
                            IRow row = sheet.GetRow(rowIndex++);
                            if (row == null)
                                continue;

                            ICell cell = row.GetCell(0);

                            //寻找以[Config]为标记的首行
                            if (cell == null ||
                                cell.CellType != CellType.String)
                                continue;

                            if (cell.GetStringCellValue().Trim() == "[Config]")
                            {
                                ExcelSheet excelSheet = new ExcelSheet()
                                {
                                    workbook = workbook,
                                    sheet = sheet,
                                    className = row.GetCell(1, MissingCellPolicy.CREATE_NULL_AS_BLANK).GetStringCellValue(),
                                    primaryKeyRow = sheet.GetRow(rowIndex++),
                                    customTypeRow = sheet.GetRow(rowIndex++),
                                    fieldTypeRow = sheet.GetRow(rowIndex++),
                                    fieldNameRow = sheet.GetRow(rowIndex++),
                                    contentBeginRowNum = ++rowIndex,
                                    contentEndRowNum = sheet.LastRowNum,
                                };
                                sheets.Add(excelSheet);
                                excelSheet.Close();
                                break;
                            }
                        }
                    }
                }
            }
            return sheets;
        }
    }

    public class ParseParam
    {
        public string ExcelDir { get; set; }
        public string OutputDir { get; set; }
        public ILogger Logger { get; set; }
    }
}
