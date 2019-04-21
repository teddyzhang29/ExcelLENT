using ExcelParser.Fields;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ExcelParser
{
    public class ExcelSheet
    {
        private Dictionary<int, BaseField> m_fieldMap = new Dictionary<int, BaseField>();
        private Lexer m_lexer = new Lexer();

        //field -> objField:id | listField:id | simpleField:id
        //objField -> obj{field objFieldRemain}
        //objFieldRemain -> ;field objFieldRemain | ε
        //listField -> list{objField} | list{listField} | list{simpleField}
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
            switch (m_lexer.Forward)
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
            field.Name = m_lexer.Forward;
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
            while (m_lexer.Forward == ";")
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
            switch (m_lexer.Forward)
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
            switch (m_lexer.Forward)
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
                    throw new Exception($"{m_lexer.Forward} 不属于SimpleField");
            }
        }

        internal void Serialize()
        {
            for (int rowNum = contentBeginRowNum; rowNum <= contentEndRowNum; rowNum++)
            {
                IRow row = sheet.GetRow(rowNum);
                foreach (var fieldItem in m_fieldMap)
                {
                    string content = row.GetCell(fieldItem.Key).GetStringCellValue();
                    m_lexer.Init(content);
                    fieldItem.Value.ParseContent(m_lexer);
                    break;
                }
            }
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