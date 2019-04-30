using ExcelParser.Fields;
using System.Collections.Generic;
using System.Text;

namespace ExcelParser.Generator
{
    public class TupledCShapGenerator : IGenerator
    {
        public void Generate(ExcelSheet excelSheet, List<BaseField> fields, GenerationParam param)
        {
            CodeBuilder builder = new CodeBuilder();
            builder.AppendLine($"public class {excelSheet.className}")
                       .AppendLine("{").AddIndent();
            {
                //Deserialization
                builder.AppendLine($"public static {excelSheet.className} Deserialize(string serialization)")
                       .AppendLine("{").AddIndent();
                {
                    builder.AppendLine($"{excelSheet.className} data = new {excelSheet.className}();");
                    builder.AppendLine($"data.m_rows = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<Row>>(serialization);");
                    builder.AppendLine($"return data;");
                }
                builder.SubtractIndent().AppendLine("}").AppendLine();

                //Row
                builder.AppendLine("public class Row")
                       .AppendLine("{").AddIndent();
                {
                    foreach (var field in fields)
                    {
                        builder.AppendLine($"public {FieldTypeString(field)} {field.Name};");
                    }
                }
                builder.SubtractIndent().AppendLine("}").AppendLine();

                //Data
                builder.AppendLine($"private System.Collections.Generic.List<Row> m_rows;");
            }
            builder.SubtractIndent().AppendLine("}");

            Utility.SaveToFile(builder.ToString(), $"{param.OutDir}/{excelSheet.className}.cs");
        }

        private string FieldTypeString(BaseField field)
        {
            StringBuilder builder = new StringBuilder();
            if (field is ListField)
            {
                builder.Append("System.Collections.Generic.List");
                if (field.Children.Count > 0)
                {
                    builder.Append("<");
                    builder.Append(FieldTypeString(field.Children[0]));
                    builder.Append(">");
                }
            }
            else if (field is ObjectField)
            {
                builder.Append("(");
                if (field.Children.Count > 0)
                {
                    builder.Append($"{FieldTypeString(field.Children[0])} {field.Children[0].Name}");
                    for (int i = 1; i < field.Children.Count; i++)
                    {
                        builder.Append($", {FieldTypeString(field.Children[i])} {field.Children[i].Name}");
                    }
                }
                builder.Append(")");
            }
            else
            {
                string typeName = field.GetType().Name.Replace("Field", "").ToLower();
                builder.Append(typeName);
            }
            return builder.ToString();
        }
    }
}