using BBGo.ExcelLENT.Fields;
using System.Collections.Generic;
using System.Text;

namespace BBGo.ExcelLENT.Generator
{
    public class TupledCSharpGenerator : IGenerator
    {
        public void Generate(ExcelSheet excelSheet, List<BaseField> fields, GenerationParam param)
        {
            CodeBuilder builder = new CodeBuilder();
            if (!string.IsNullOrEmpty(param.Package))
            {
                builder.AppendLine($"namespace {param.Package}")
                       .AppendLine("{").AddIndent();
            }
            {
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

                        if (excelSheet.PrimaryFieldsList.Count > 0)
                        {
                            builder.AppendLine("foreach (var row in s_rows)")
                                   .AppendLine("{").AddIndent();

                            foreach (var primaryFields in excelSheet.PrimaryFieldsList)
                            {
                                if (primaryFields.Count == 0)
                                    continue;

                                builder.AppendLine("{").AddIndent();
                                {
                                    string mapFieldName = $"s_{Utility.ToCamelCase(primaryFields.ConvertAll((v) => v.Name).ToArray())}Map";
                                    for (int i = 0; i < primaryFields.Count - 1; i++)
                                    {
                                        BaseField keyField = primaryFields[i];
                                        string mapValueType = FieldFullMapName(primaryFields, i + 1);
                                        string mapValueFieldName = $"{Utility.ToCamelCase(primaryFields.ConvertAll((v) => v.Name).ToArray(), i + 1)}Map";
                                        builder.AppendLine($"{mapValueType} {mapValueFieldName};");
                                        builder.AppendLine($"if (!{mapFieldName}.TryGetValue(row.{keyField.Name}, out {mapValueFieldName}))")
                                               .AppendLine("{").AddIndent();
                                        builder.AppendLine($"{mapValueFieldName} = new {mapValueType}();");
                                        builder.AppendLine($"{mapFieldName}.Add(row.{keyField.Name}, {mapValueFieldName});");
                                        builder.SubtractIndent().AppendLine("}");
                                        mapFieldName = mapValueFieldName;
                                    }
                                    builder.AppendLine($"{mapFieldName}.Add(row.{primaryFields[primaryFields.Count - 1].Name}, row);");
                                }
                                builder.SubtractIndent().AppendLine("}");
                            }

                            builder.SubtractIndent().AppendLine("}");
                        }
                    }
                    builder.SubtractIndent().AppendLine("}").AppendLine();

                    //PrimaryKey
                    foreach (var primaryFields in excelSheet.PrimaryFieldsList)
                    {
                        if (primaryFields.Count == 0)
                            continue;

                        string mapTypeName = FieldFullMapName(primaryFields);
                        string mapFieldName = $"s_{Utility.ToCamelCase(primaryFields.ConvertAll((v) => v.Name).ToArray())}Map";
                        builder.AppendLine($"private static {mapTypeName} {mapFieldName} = new {mapTypeName}();");
                        builder.Append($"public static Row FindBy{Utility.ToPascalCase(primaryFields.ConvertAll((v) => v.Name).ToArray())}(");
                        for (int i = 0; i < primaryFields.Count; i++)
                        {
                            if (i != 0)
                            {
                                builder.Append(", ", true);
                            }
                            builder.Append($"{FieldFullTypeName(primaryFields[i])} {primaryFields[i].Name.ToLower()}", true);
                        }
                        builder.AppendLine(")", true)
                               .AppendLine("{").AddIndent();
                        {
                            for (int i = 0; i < primaryFields.Count - 1; i++)
                            {
                                BaseField keyField = primaryFields[i];
                                string mapValueType = FieldFullMapName(primaryFields, i + 1);
                                string mapValueFieldName = $"{Utility.ToCamelCase(primaryFields.ConvertAll((v) => v.Name).ToArray(), i + 1)}Map";
                                builder.AppendLine($"{mapValueType} {mapValueFieldName};");
                                builder.AppendLine($"if (!{mapFieldName}.TryGetValue({primaryFields[i].Name.ToLower()}, out {mapValueFieldName}))")
                                       .AppendLine("{").AddIndent();
                                builder.AppendLine($"throw new System.Exception($\"Config Not Found:`{{{keyField.Name.ToLower()}}}`\");");
                                builder.SubtractIndent().AppendLine("}");
                                mapFieldName = mapValueFieldName;
                            }
                            builder.AppendLine("Row retVal;");
                            builder.AppendLine($"if (!{mapFieldName}.TryGetValue({primaryFields[primaryFields.Count - 1].Name.ToLower()}, out retVal))")
                                   .AppendLine("{").AddIndent();
                            builder.AppendLine($"throw new System.Exception($\"Config Not Found:`{{{primaryFields[primaryFields.Count - 1].Name.ToLower()}}}`\");");
                            builder.SubtractIndent().AppendLine("}");
                            builder.AppendLine("return retVal;");
                        }
                        builder.SubtractIndent().AppendLine("}");
                    }

                    //Row
                    builder.AppendLine("public class Row")
                           .AppendLine("{").AddIndent();
                    {
                        foreach (var field in fields)
                        {
                            builder.AppendLine("/// <summary>")
                                   .AppendLine($"/// {field.Description}")
                                   .AppendLine("/// </summary>");
                            builder.AppendLine($"public {FieldFullTypeName(field)} {field.Name} {{ get; set; }}");
                        }
                    }
                    builder.SubtractIndent().AppendLine("}").AppendLine();
                }
                builder.SubtractIndent().AppendLine("}");
            }
            if (!string.IsNullOrEmpty(param.Package))
            {
                builder.SubtractIndent().AppendLine("}");
            }

            Utility.SaveToFile(builder.ToString(), $"{param.OutDir}/{excelSheet.ClassName}.cs");
        }

        private string FieldFullMapName(List<BaseField> fields, int start = 0)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = start; i < fields.Count; i++)
            {
                builder.Append($"System.Collections.Generic.Dictionary<{FieldFullTypeName(fields[i])}, ");
            }
            builder.Append("Row");
            for (int i = start; i < fields.Count; i++)
            {
                builder.Append(">");
            }
            return builder.ToString();
        }

        private string FieldFullTypeName(BaseField field)
        {
            StringBuilder builder = new StringBuilder();
            if (field is ListField)
            {
                builder.Append("System.Collections.Generic.List");
                if (field.Children.Count > 0)
                {
                    builder.Append("<");
                    builder.Append(FieldFullTypeName(field.Children[0]));
                    builder.Append(">");
                }
            }
            else if (field is ObjectField)
            {
                builder.Append("(");
                if (field.Children.Count > 0)
                {
                    builder.Append($"{FieldFullTypeName(field.Children[0])} {field.Children[0].Name}");
                    for (int i = 1; i < field.Children.Count; i++)
                    {
                        builder.Append($", {FieldFullTypeName(field.Children[i])} {field.Children[i].Name}");
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