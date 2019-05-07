using BBGo.ExcelLENT.Fields;
using System.Collections.Generic;

namespace BBGo.ExcelLENT.Generator
{
    public interface IGenerator
    {
        void Generate(ExcelSheet excelSheet, List<BaseField> fields, GenerationParam param);
        void OnPostGenerate(List<ExcelSheet> sheets, GenerationParam param);
    }
}
