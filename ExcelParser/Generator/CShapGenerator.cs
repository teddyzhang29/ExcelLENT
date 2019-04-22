using ExcelParser.Fields;
using System.Collections.Generic;
using System.Text;

namespace ExcelParser.Generator
{
    public class CShapGenerator : IGenerator
    {
        public void Generate(ExcelSheet excelSheet, List<BaseField> fields)
        {
            CodeBuilder builder = new CodeBuilder();
            builder.AppendLine($"public class {excelSheet.className}")
                       .AppendLine("{").AddIndent();
            {
                //Row
                builder.AppendLine("public class Row")
                   .AppendLine("{").AddIndent();
                {
                    foreach (var field in fields)
                    {
                        builder.AppendLine($"public {FieldTypeString(field)} {field.Name};");
                    }
                }
                builder.SubtractIndent().AppendLine("}");
            }
            builder.SubtractIndent().AppendLine("}");

            System.Console.WriteLine(builder.ToString());
        }

        private string FieldTypeString(BaseField field)
        {
            StringBuilder builder = new StringBuilder();
            if (field is ListField)
            {
                builder.Append("List");
                if (field.Children.Count > 0)
                {
                    builder.Append("<");
                    builder.Append(FieldTypeString(field.Children[0]));
                    for (int i = 1; i < field.Children.Count; i++)
                    {
                        builder.Append(",");
                        builder.Append(FieldTypeString(field.Children[i]));
                    }
                    builder.Append(">");
                }
            }
            else if (field is ObjectField)
            {
                builder.Append($"{field.Name}Obj");
            }
            else
            {
                string typeName = field.GetType().Name.Replace("Field", "").ToLower();
                builder.Append(typeName);
            }
            return builder.ToString();
        }
    }

    public class Skill
    {
        public static Skill Deserialize(string json)
        {
            Skill skill = new Skill();
            List<Row> row = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Row>>(json);
            skill.datas = row;
            return skill;
        }

        public List<Row> datas = new List<Row>();

        public class Row
        {
            public List<int> skills;
            public PosObj pos;
            public List<(int id, float level)> skill2;
            public List<bool> success;
            public (string name, string address) desc;
        }

        public class PosObj
        {
            public int x;
            public int z;
        }
    }
}
