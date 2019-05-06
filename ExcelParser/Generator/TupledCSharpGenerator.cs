using ExcelParser.Fields;
using System.Collections.Generic;
using System.Text;

namespace ExcelParser.Generator
{
    public class TupledCSharpGenerator : IGenerator
    {
        public void Generate(ExcelSheet excelSheet, List<BaseField> fields, GenerationParam param)
        {
            CodeBuilder builder = new CodeBuilder();
            builder.AppendLine($"public class {excelSheet.ClassName}")
                       .AppendLine("{").AddIndent();
            {
                //Data
                builder.AppendLine($"private static System.Collections.Generic.List<Row> s_rows;");
                //Deserialization
                builder.AppendLine("public static void Deserialize(string serialization)")
                       .AppendLine("{").AddIndent();
                {
                    builder.AppendLine("s_rows = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<Row>>(serialization);");
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
            }
            builder.SubtractIndent().AppendLine("}");

            Utility.SaveToFile(builder.ToString(), $"{param.OutDir}/{excelSheet.ClassName}.cs");
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

        public virtual void OnPostGenerate(List<ExcelSheet> sheets, GenerationParam param)
        {
        }
    }
}