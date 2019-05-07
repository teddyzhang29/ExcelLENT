using BBGo.ExcelLENT.Fields;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BBGo.ExcelLENT
{
    public class ExcelSheet
    {
        public string ClassName { get; set; }
        public IRow CustomTypeRow { get; set; }
        public IRow FieldTypeRow { get; set; }
        public IRow FieldNameRow { get; set; }
        public IRow FieldDescriptionRow { get; set; }
        public int ContentBeginRowNum { get; set; }
        public int ContentEndRowNum { get; set; }
        public ISheet Sheet { get; set; }
        public IWorkbook Workbook { get; set; }
        public List<List<BaseField>> PrimaryFieldsList { get; set; } = new List<List<BaseField>>();

        internal IRow m_primaryKeyRow { get; set; }
        private Dictionary<int, BaseField> m_fieldMap = new Dictionary<int, BaseField>();
        private Dictionary<string, BaseField> m_fieldNameMap = new Dictionary<string, BaseField>();
        private Lexer m_lexer = new Lexer();

        internal void ReadPrimaryFields()
        {
            if (m_primaryKeyRow == null)
                return;

            for (int cellNum = m_primaryKeyRow.FirstCellNum; cellNum < m_primaryKeyRow.LastCellNum; cellNum++)
            {
                string fieldString = m_primaryKeyRow.GetCell(cellNum, MissingCellPolicy.CREATE_NULL_AS_BLANK).GetStringCellValue();
                if (string.IsNullOrEmpty(fieldString))
                    break;

                string[] fieldNames = fieldString.Split(',');
                List<BaseField> primaryFields = new List<BaseField>();
                foreach (var fieldName in fieldNames)
                {

                    BaseField primaryField;
                    if (!m_fieldNameMap.TryGetValue(fieldName, out primaryField))
                    {
                        throw new Exception($"Cannot find primary key:`{fieldName}`");
                    }
                    primaryFields.Add(primaryField);
                }
                PrimaryFieldsList.Add(primaryFields);
            }
        }

        //field -> objField:id | listField:id | simpleField:id
        //objField -> obj{field objFieldRemain}
        //objFieldRemain -> ;field objFieldRemain | ε
        //listField -> list{objField} | list{listField} | list{simpleField}
        //simpleField -> int | float | double | bool | string
        internal void ParseFields()
        {
            int first = Math.Min(FieldTypeRow.FirstCellNum, FieldNameRow.FirstCellNum);
            for (int cellNum = first;
                cellNum < Math.Max(FieldTypeRow.LastCellNum, FieldNameRow.LastCellNum);
                cellNum++)
            {
                string fieldType = FieldTypeRow.GetCell(cellNum, MissingCellPolicy.CREATE_NULL_AS_BLANK).GetStringCellValue();
                string fieldName = FieldNameRow.GetCell(cellNum, MissingCellPolicy.CREATE_NULL_AS_BLANK).GetStringCellValue();

                if (string.IsNullOrEmpty(fieldType))
                    continue;

                if (string.IsNullOrEmpty(fieldName))
                    continue;

                fieldType = fieldType + ":" + fieldName;
                m_lexer.Init(fieldType);

                BaseField field = ParseField();
                field.Description = FieldDescriptionRow.GetCell(cellNum, MissingCellPolicy.CREATE_NULL_AS_BLANK).GetStringCellValue();

                if (field != null)
                {
                    m_fieldMap.Add(cellNum, field);
                    m_fieldNameMap.Add(field.Name, field);
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
            field.Children.Add(ParseField());
            field.Children.AddRange(ObjFieldRemain());
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
            switch (m_lexer.Lexical)
            {
                case "obj":
                    field.Children.Add(ObjField());
                    break;
                case "list":
                    field.Children.Add(ListField());
                    break;
                default:
                    field.Children.Add(SimpleField());
                    break;
            }
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
                    throw new Exception($"{m_lexer.Lexical} is not SimpleField。Position:{m_lexer.Position}。");
            }
        }

        internal void Serialize(SerializationParam param)
        {
            int currRowNum = 0;
            BaseField currField = null;
            try
            {
                for (currRowNum = ContentBeginRowNum; currRowNum <= ContentEndRowNum; currRowNum++)
                {
                    IRow row = Sheet.GetRow(currRowNum);
                    param.Serializer.BeginRow();
                    foreach (var fieldItem in m_fieldMap)
                    {
                        currField = fieldItem.Value;
                        string content = row.GetCell(fieldItem.Key).GetStringCellValue();
                        if (currField is ListField || currField is ObjectField)
                        {
                            //对于ListField和ObjectField, 自动在两侧加上{}
                            content = $"{{{content}}}";
                        }
                        m_lexer.Init(content);
                        param.Serializer.BeginField(currField);
                        currField.OnSerialize(param.Serializer, m_lexer);
                        param.Serializer.EndField(currField);
                    }
                    param.Serializer.EndRow();
                }
                Utility.SaveToFile(param.Serializer.Result, $"{param.OutDir}/{ClassName}.json");
            }
            catch (Exception e)
            {
                throw new SerializationException(this, currRowNum, currField, e);
            }
        }

        internal void Generate(GenerationParam param)
        {
            param.Generator.Generate(this, m_fieldMap.Values.ToList(), param);
        }

        public void Close()
        {
            Workbook.Close();
        }
    }
}
