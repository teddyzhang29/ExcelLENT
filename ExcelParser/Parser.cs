using ExcelParser.Generator;
using ExcelParser.Serializer;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

            foreach (var sheet in sheets)
            {
                sheet.ReadPrimaryFields();
            }

            for (int i = 0; param.Serializations != null && i < param.Serializations.Length; i++)
            {
                foreach (var sheet in sheets)
                {
                    sheet.Serialize(param.Serializations[i]);
                }
            }

            for (int i = 0; param.Generations != null && i < param.Generations.Length; i++)
            {
                foreach (var sheet in sheets)
                {
                    sheet.Generate(param.Generations[i]);
                }
                param.Generations[i].Generator.OnPostGenerate(sheets, param.Generations[i]);
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
                                    Workbook = workbook,
                                    Sheet = sheet,
                                    ClassName = row.GetCell(1, MissingCellPolicy.CREATE_NULL_AS_BLANK).GetStringCellValue(),
                                    m_primaryKeyRow = sheet.GetRow(rowIndex++),
                                    CustomTypeRow = sheet.GetRow(rowIndex++),
                                    FieldTypeRow = sheet.GetRow(rowIndex++),
                                    FieldNameRow = sheet.GetRow(rowIndex++),
                                    FieldDescriptionRow = sheet.GetRow(rowIndex++),
                                    ContentBeginRowNum = rowIndex,
                                    ContentEndRowNum = sheet.LastRowNum,
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
        public ILogger Logger { get; set; }
        public SerializationParam[] Serializations { get; set; }
        public GenerationParam[] Generations { get; set; }
    }

    public class SerializationParam
    {
        public ISerializer Serializer { get; set; }
        public string OutDir { get; set; }
    }

    public class GenerationParam
    {
        public IGenerator Generator { get; set; }
        public string OutDir { get; set; }
    }
}
