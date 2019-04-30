using ExcelParser.Fields;
using System.Collections.Generic;

namespace ExcelParser.Generator
{
    public interface IGenerator
    {
        void Generate(ExcelSheet excelSheet, List<BaseField> fields, GenerationParam param);
        void OnPostGenerate(List<ExcelSheet> sheets, GenerationParam param);
    }
}
