using ExcelParser.Fields;
using ExcelParser.Generator;
using ExcelParser.Serializer;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelParser
{
    public class ExcelSheet
    {
        private Dictionary<int, BaseField> m_fieldMap = new Dictionary<int, BaseField>();
        private Lexer m_lexer = new Lexer();

        //field -> objField:id | listField:id | simpleField:id
        //objField -> obj{field objFieldRemain}
        //objFieldRemain -> ;field objFieldRemain | ε
        //listField -> list{field}
        //simpleField -> int | float | double | bool | string
        internal void ParseFields()
        {
            int first = Math.Min(fieldTypeRow.FirstCellNum, fieldNameRow.FirstCellNum);
            for (int cellNum = first;
                cellNum < Math.Max(fieldTypeRow.LastCellNum, fieldNameRow.LastCellNum);
                cellNum++)
            {
                string fieldType = fieldTypeRow.GetCell(cellNum, MissingCellPolicy.CREATE_NULL_AS_BLANK).GetStringCellValue();
                string fieldName = fieldNameRow.GetCell(cellNum, MissingCellPolicy.CREATE_NULL_AS_BLANK).GetStringCellValue();

                if (string.IsNullOrEmpty(fieldType))
                    continue;

                if (string.IsNullOrEmpty(fieldName))
                    continue;

                fieldType = fieldType + ":" + fieldName;
                m_lexer.Init(fieldType);

                BaseField field = ParseField();
                if (field != null)
                {
                    m_fieldMap.Add(cellNum, field);
                }
            }
        }

        private BaseField ParseField()
        {
            BaseField field = null;
            switch (m_lexer.Lexical)
            {
                case "obj":
                    field = ObjField();
                    break;
                case "list":
                    field = ListField();
                    break;
                default:
                    field = SimpleField();
                    break;
            }
            m_lexer.Match(":");
            field.Name = m_lexer.Lexical;
            m_lexer.Match(field.Name);
            return field;
        }

        private BaseField ObjField()
        {
            BaseField field = new ObjectField();
            m_lexer.Match("obj");
            m_lexer.Match("{");
            field.AddChild(ParseField());
            foreach (var child in ObjFieldRemain())
            {
                field.AddChild(child);
            }
            m_lexer.Match("}");
            return field;
        }

        private List<BaseField> ObjFieldRemain()
        {
            List<BaseField> children = new List<BaseField>();
            while (m_lexer.Lexical == ";")
            {
                m_lexer.Match(";");
                children.Add(ParseField());
            }
            return children;
        }

        private BaseField ListField()
        {
            BaseField field = new ListField();
            m_lexer.Match("list");
            m_lexer.Match("{");
            field.AddChild(ParseField());
            m_lexer.Match("}");
            return field;
        }

        private BaseField SimpleField()
        {
            switch (m_lexer.Lexical)
            {
                case "int":
                    m_lexer.Match("int");
                    return new IntField();
                case "float":
                    m_lexer.Match("float");
                    return new FloatField();
                case "double":
                    m_lexer.Match("double");
                    return new DoubleField();
                case "bool":
                    m_lexer.Match("bool");
                    return new BoolField();
                case "string":
                    m_lexer.Match("string");
                    return new StringField();
                default:
                    throw new Exception($"{m_lexer.Lexical} 不属于SimpleField。位置:{m_lexer.Position}。");
            }
        }

        internal void Serialize(ISerializer serializer, ParseParam param)
        {
            for (int rowNum = contentBeginRowNum; rowNum <= contentEndRowNum; rowNum++)
            {
                IRow row = sheet.GetRow(rowNum);
                serializer.BeginRow();
                foreach (var fieldItem in m_fieldMap)
                {
                    string content = row.GetCell(fieldItem.Key).GetStringCellValue();
                    m_lexer.Init(content);
                    serializer.BeginField(fieldItem.Value);
                    fieldItem.Value.OnSerialize(serializer, m_lexer, param);
                    serializer.EndField(fieldItem.Value);
                }
                serializer.EndRow();
            }

            Skill skill = Skill.Deserialize(serializer.Result);
            //(string s1, int t) t = ("teddy", 3);
            //string tuple = Newtonsoft.Json.JsonConvert.SerializeObject(t);
            //string tuple = "{\"Item1\":\"teddy\",\"Item2\":3}";
            //(string, int) t = Newtonsoft.Json.JsonConvert.DeserializeObject<(string, int)>(tuple);

            //Console.WriteLine(tuple);
            //Console.WriteLine(serializer.Result);
        }

        internal void Generate(IGenerator generator, ParseParam param)
        {
            generator.Generate(this, m_fieldMap.Values.ToList());
        }



















        internal string className;
        internal IRow primaryKeyRow;
        internal IRow customTypeRow;
        internal IRow fieldTypeRow;
        internal IRow fieldNameRow;
        internal int contentBeginRowNum;
        internal int contentEndRowNum;
        internal ISheet sheet;
        internal IWorkbook workbook;

        public void Close()
        {
            workbook.Close();
        }
    }
}
