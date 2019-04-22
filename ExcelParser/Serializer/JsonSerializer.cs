using ExcelParser.Fields;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ExcelParser.Serializer
{
    public class JsonSerializer : ISerializer
    {
        private JArray m_data = new JArray();
        private JObject m_row = null;
        private JToken m_field = null;
        private Stack<char> m_matchStack = new Stack<char>();

        public string Result
        {
            get
            {
                return m_data.ToString();
            }
        }

        public void BeginRow()
        {
            m_row = new JObject();
        }

        public void EndRow()
        {
            m_data.Add(m_row);
            m_row = null;
        }

        public void BeginField(BaseField field)
        {
        }

        public void EndField(BaseField field)
        {
            m_row.Add(field.Name, m_field);
            m_field = null;
        }

        public void BeginList(BaseField field)
        {
            BeginGroup(field, new JArray());
        }
        public void BeginObject(ObjectField field)
        {
            BeginGroup(field, new JObject());
        }

        private void BeginGroup(BaseField field, JToken group)
        {
            m_matchStack.Push('{');
            if (m_field is JObject)
            {
                JObject objField = m_field as JObject;
                objField.Add(field.Name, group);
            }
            else if (m_field is JArray)
            {
                JArray arrayField = m_field as JArray;
                arrayField.Add(group);
            }
            else if (m_field != null)
            {
                throw new Exception($"'{field.GetType()}' 类型无法添加子类型。");
            }
            m_field = group;
        }

        public void EndList(BaseField field)
        {
            EndGroup(field);
        }
        public void EndObject(ObjectField field)
        {
            EndGroup(field);
        }

        private void EndGroup(BaseField field)
        {
            if (m_matchStack.Count == 0)
            {
                throw new Exception("括号不匹配。");
            }
            m_matchStack.Pop();
            if (m_matchStack.Count > 0)
            {
                m_field = m_field.Parent;
            }
        }

        public void FloatField(FloatField field, float value)
        {
            SimpleField(field, value);
        }

        public void DoubleField(DoubleField field, double value)
        {
            SimpleField(field, value);
        }

        public void IntField(IntField field, int value)
        {
            SimpleField(field, value);
        }

        public void BoolField(BoolField field, bool value)
        {
            SimpleField(field, value);
        }

        public void StringField(StringField field, string value)
        {
            SimpleField(field, value);
        }

        private void SimpleField(BaseField field, JToken value)
        {
            if (m_field == null)
            {
                m_field = value;
            }
            else if (m_field is JObject)
            {
                JObject objField = m_field as JObject;
                objField.Add(field.Name, value);
            }
            else if (m_field is JArray)
            {
                JArray arrayField = m_field as JArray;
                arrayField.Add(value);
            }
        }
    }
}
